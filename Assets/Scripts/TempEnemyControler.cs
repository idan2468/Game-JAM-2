using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Singletons;
using UnityEngine.UI;

public class TempEnemyControler : MonoBehaviour
{
    [SerializeField] private Vector3 _currentTargetPosition;
    [SerializeField] private float _movementSpeed = 2f;
    [SerializeField] private float _rotationSpeed = 2f;

    [SerializeField] private bool _isTainting = false; // Enemy is in tainting state
    [SerializeField] private float _taintDuration; // Time it takes to taint a Jew
    private Tweener _fadeTweener;

    private Animator _enemyAnimator;
    [SerializeField] private bool _isUsingAnimator = false;
    [SerializeField] private Image tainBar;

    private float _findJewInterval = .25f;

    // Start is called before the first frame update
    void Start()
    {
        _taintDuration = 5f;
        _enemyAnimator = GetComponent<Animator>();
        //case 1
        StartCoroutine(FindTarget());
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
                var closestJew = GameManager.Instance.GetClosestFreeJew(transform.position);
                if (closestJew != null && (closestJew.GetComponent<TempJewControler>().GetChase() == null ||
                    closestJew.GetComponent<TempJewControler>().GetChase() == gameObject))
                {
                    closestJew.GetComponent<TempJewControler>().SetChase(gameObject);
                    _currentTargetPosition = closestJew.transform.position;
                }
                else
                {
                    _currentTargetPosition = transform.position;
                }
            }

            yield return new WaitForSeconds(_findJewInterval);
        }
    }

    // Move enemy in direction of _currentTargetPosition
    private void EnemyMove()
    {
        var angleToTarget = Vector3.Angle(transform.forward, _currentTargetPosition - transform.position);
        if (angleToTarget > 0)
        {
            var normDirection = (_currentTargetPosition - transform.position).normalized;
            var lookRotation = Quaternion.LookRotation(normDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _rotationSpeed);
        }

        transform.position =
            Vector3.MoveTowards(transform.position, _currentTargetPosition, _movementSpeed * Time.deltaTime);
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
        return seq;
    }

    // Freeze jew and enemy
    private IEnumerator TaintJew(GameObject jew)
    {
        var jewScript = jew.GetComponent<TempJewControler>();
        jewScript.EnterTaintState(gameObject);
        var success = false;
        var seq = GetSequenceForTaintBar();
        seq.Play();
        yield return seq.WaitForCompletion();

        if (jewScript != null)
        {
            if (jewScript.CurrentState == TempJewControler.State.CaughtByEnemy)
            {
                success = true;
            }
        }

        StopTainting(jew, success);
    }

    // Switch to tainting mode, add jew, start fade
    private void Taint(GameObject jew)
    {
        Debug.Log("Start Tainting");
        if (_isUsingAnimator) _enemyAnimator.SetBool("isTainting", true);

        _isTainting = true;
        StartCoroutine(TaintJew(jew));
    }

    private void StopTainting(GameObject jew, bool success)
    {
        if (_isUsingAnimator) _enemyAnimator.SetBool("isTainting", false);

        _isTainting = false;

        if (success) // Destroy Jew if tainting uninterrupted
        {
            GameManager.Instance.KillJew(jew);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Catch Jew if he's free
        if (other.tag.Equals("Jew"))
        {
            Debug.Log("Collided with jew");
            if (other.gameObject.GetComponent<TempJewControler>().CurrentState == TempJewControler.State.Free)
            {
                Taint(other.gameObject);
            }
        }
    }
}