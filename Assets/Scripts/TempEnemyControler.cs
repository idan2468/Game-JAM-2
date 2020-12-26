using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Singletons;

public class TempEnemyControler : MonoBehaviour
{
    [SerializeField]private Vector3 _currentTargetPosition;
    [SerializeField] private float _movementSpeed = 2f;
    [SerializeField] private float _rotationSpeed = 2f;

    [SerializeField] private bool _isTainting = false; // Enemy is in tainting state
    [SerializeField] private float _taintDuration; // Time it takes to taint a Jew
    private Tweener _fadeTweener;

    private Animator _enemyAnimator;
    [SerializeField] private bool _isUsingAnimator = false;

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
        if(!_isTainting)
        {
            EnemyMove();
        }
    }

    private IEnumerator FindTarget()
    {
        while (true)
        {
            if(!_isTainting)
            {
                var closestJew = GameManager.Instance.GetClosestFreeJew(transform.position);
                _currentTargetPosition = closestJew == null ? transform.position : closestJew.transform.position;
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
        transform.position = Vector3.MoveTowards(transform.position, _currentTargetPosition, _movementSpeed * Time.deltaTime);
    }

    // Freeze jew and enemy
    private IEnumerator TaintJew(GameObject jew)
    {
        var jewScript = jew.GetComponent<TempJewControler>();
        jewScript.EnterTaintState(gameObject);
        var success = false;

        yield return new WaitForSeconds(_taintDuration);
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
