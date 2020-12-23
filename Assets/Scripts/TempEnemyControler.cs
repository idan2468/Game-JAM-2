using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TempEnemyControler : MonoBehaviour
{
    public ObjectSpawner myObjectSpawner;

    private Vector3 _currentTargetPosition;
    [SerializeField] private float _speed = 2f;

    private bool _isTainting = false; // Enemy is in tainting state
    [SerializeField] private float _taintDuration = 5f; // Time it takes to taint a Jew
    private GameObject _jew;

    private Animator _enemyAnimator;

    // Start is called before the first frame update
    void Start()
    {
        _enemyAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // If tainting do nothing
        if (_isTainting)
        {
            if (_jew == null) StopTainting();
        }

        // Otherwise move towards closest Jew
        if(!_isTainting)
        {
            var closestJew = myObjectSpawner.GetClosestJew(transform.position);
            _currentTargetPosition = closestJew == null ? transform.position : closestJew.transform.position;
            EnemyMove();
        }
    }

    public void RemoveJew()
    {
        _jew = null;
    }

    // Move enemy in direction of _currentTargetPosition
    private void EnemyMove()
    {
        transform.position += (_currentTargetPosition - transform.position) * Time.deltaTime * _speed;
    }

    // TODO: WATCH OUT FOR _jew BEING NULL BEFORE OnUpdate() (SHOULDN'T BE A PROBLEM)
    // Switch to tainting mode, add jew to enemy, start fade
    private void StartTainting(GameObject jew)
    {
        _isTainting = true;
        _enemyAnimator.SetBool("isTainting", _isTainting);
        bool successfullyTainted = true;

        _jew = jew;
        var jewMaterial = _jew.GetComponent<Material>();
        var fadeTweener = jewMaterial.DOFade(0, _taintDuration);
        fadeTweener.OnUpdate(() =>
        {
            // Tainting was interrupted, set alpha back to 1
            if (_jew == null)
            {
                // TODO: TRY TO REFORMAT
                var currentColor = jewMaterial.color;
                currentColor.a = 1;
                jewMaterial.SetColor("Color", currentColor);
                fadeTweener.Kill();
                successfullyTainted = false;
            }
        });

        if (successfullyTainted) myObjectSpawner.KillJew(_jew);
    }

    private void StopTainting()
    {
        _isTainting = false;
        _enemyAnimator.SetBool("isTainting", _isTainting);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Handle Jew trigger
        // TODO: ADD CONDITION PREVENTING TAKING JEW FROM GOLEM
        if (other.tag.Equals("Jew"))
        {
            StartTainting(other.gameObject);
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    // Handle Jew trigger
    //    if (other.tag.Equals("Jew"))
    //    {
    //        StopTainting();
    //    }
    //}
}
