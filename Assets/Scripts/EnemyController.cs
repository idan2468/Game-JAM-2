﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Singletons;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 2f;
    [SerializeField] private float _rotationSpeed = 2f;

    [SerializeField] private bool _isTainting = false; // Enemy is in tainting state
    [SerializeField] private float _taintDuration; // Time it takes to taint a Jew
    [SerializeField]private float _fadeDuration;

    private Animator _enemyAnimator;
    [SerializeField] private bool _isUsingAnimator = false;
    [SerializeField] private Image tainBar;

    private float _findJewInterval = .25f;

    [SerializeField] private float lifeTime;
    [SerializeField] private JewController _taintedJew;
    [SerializeField] private GameObject _currentTargetObject;
    private JewController _currentTargetObjectController;

    private Material[] myMaterials;

    

    // Start is called before the first frame update
    void Start()
    {
        _currentTargetObject = gameObject;
        _enemyAnimator = GetComponentInChildren<Animator>();
        myMaterials = GetComponentsInChildren<MeshRenderer>().Select(meshRenderer => meshRenderer.material).ToArray();
    }

    private void OnEnable()
    {
        //case 1
        StartCoroutine(FindTarget());
        StartCoroutine(KillMe());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator KillMe()
    {
        yield return new WaitForSeconds(lifeTime);
        if (_isTainting)
        {
            _taintedJew.EnterFreeState();
        }

        var seq = DOTween.Sequence();
        foreach (var myMaterial in myMaterials)
        {
            seq.Join(myMaterial.DOFade(0, _fadeDuration));
        }
        seq.AppendCallback(() => GameManager.Instance.KillEnemy(gameObject));
        seq.Play();
        yield return seq.WaitForCompletion();
        // GameManager.Instance.KillEnemy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // Move towards closest Jew
        if (!_isTainting)
        {
            EnemyMove();
        }
    }

    private IEnumerator FindTarget()
    {
        while (true)
        {
            if (!_isTainting)
            {
                var closestJewController = GameManager.Instance.GetClosestFreeJew(transform.position);

                //if (closestJewController != null && (closestJewController.GetChase() == null || closestJewController.GetChase() == gameObject))
                //{
                //    if (_currentTargetObject.tag.Equals("Jew") && _currentTargetObject != closestJewController.gameObject)
                //    {
                //        _currentTargetObjectController.SetChase(null);
                //    }

                //    closestJewController.SetChase(gameObject);
                //    _currentTargetObject = closestJewController.gameObject;
                //    _currentTargetObjectController = closestJewController;
                //}

                // Found Jew
                if (closestJewController != null)
                {
                    // Already chasing Jew
                    if (_currentTargetObject.gameObject.tag.Equals("Jew"))
                    {
                        yield return new WaitForSeconds(_findJewInterval);
                    }

                    // Jew is not chased by anyone else
                    if (closestJewController.GetChase() == null)
                    {
                        // Release previous Jew
                        if (_currentTargetObject.tag.Equals("Jew"))
                        {
                            _currentTargetObjectController.SetChase(null);
                        }

                        _currentTargetObject = closestJewController.gameObject;
                        closestJewController.SetChase(gameObject);
                        _currentTargetObjectController = closestJewController;
                    }
                }
                else // No Jew
                {
                    _currentTargetObject = gameObject;
                }
            }
            yield return new WaitForSeconds(_findJewInterval);
        }
    }

    // Move enemy in direction of _currentTargetPosition
    private void EnemyMove()
    {
        var angleToTarget = Vector3.Angle(transform.forward, _currentTargetObject.transform.position - transform.position);
        if (angleToTarget > 0)
        {
            var normDirection = (_currentTargetObject.transform.position - transform.position).normalized;
            var lookRotation = Quaternion.LookRotation(normDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _rotationSpeed);
        }

        transform.position =
            Vector3.MoveTowards(transform.position, _currentTargetObject.transform.position, _movementSpeed * Time.deltaTime);
    }

    private Sequence GetSequenceForTaintBar()
    {
        var seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            tainBar.gameObject.SetActive(true);
            tainBar.fillAmount = 1;
        });
        seq.Append(tainBar.DOFillAmount(0, _taintDuration));
        seq.AppendCallback(() => tainBar.gameObject.SetActive(false));
        seq.OnKill(() => tainBar.gameObject.SetActive(false));
        return seq;
    }

    // Freeze jew and enemy
    private IEnumerator TaintJew(GameObject jew)
    {
        var jewScript = jew.GetComponent<JewController>();
        jewScript.EnterTaintState(gameObject);
        var success = true;
        var seq = GetSequenceForTaintBar();
        seq.Play().OnUpdate(() =>
        {
            if (jewScript != null)
            {
                if (jewScript.CurrentState != JewController.State.CaughtByEnemy)
                {
                    seq.Kill();
                    success = false;
                }
            }   
        });
        yield return seq.WaitForCompletion();
        StopTainting(jew, success);
    }

    // Switch to tainting mode, add jew, start fade
    private void Taint(GameObject jew)
    {
        Debug.Log("Start Tainting");
        if (_isUsingAnimator) _enemyAnimator.SetBool("isTainting", true);

        _isTainting = true;
        _taintedJew = jew.GetComponent<JewController>();
        StartCoroutine(TaintJew(jew));
    }

    private void StopTainting(GameObject jew, bool success)
    {
        if (_isUsingAnimator) _enemyAnimator.SetBool("isTainting", false);

        _isTainting = false;

        if (success) // Destroy Jew if tainting uninterrupted
        {
            GameManager.Instance.LoseLife();
            GameManager.Instance.KillJew(jew);
        }

        _currentTargetObject = gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Catch Jew if he's free
        if (other.tag.Equals("Jew"))
        {
            Debug.Log("Collided with jew");
            if (!_isTainting && other.gameObject.GetComponent<JewController>().CurrentState == JewController.State.Free)
            {
                Taint(other.gameObject);
            }
        }
    }

    public void ReleaseJew()
    {
        if (_isTainting)
        {
            //release jew 
            _taintedJew.EnterFreeState();
            _isTainting = false;
        }
    }
}