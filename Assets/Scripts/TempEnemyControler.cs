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
    [SerializeField] private float _taintDuration = 5f; // Time it takes to taint a Jew

    private Animator _enemyAnimator;

    private float _findJewInterval = 1f;

    // Start is called before the first frame update
    void Start()
    {
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

    // Switch to tainting mode, add jew, start fade
    private bool StartTainting(GameObject jew)
    {
        Debug.Log("Start Tainting");
        _enemyAnimator.SetBool("isTainting", true);

        _isTainting = true;
        bool success = true;
        var jewScript = jew.GetComponent<TempJewControler>();
        jewScript.EnterTaintState();

        var jewMaterial = jew.GetComponent<Material>();
        var fadeTweener = jewMaterial.DOFade(0, _taintDuration);
        fadeTweener.OnUpdate(() =>
        {
            // Tainting was interrupted, set alpha back to 1
            if (jewScript.CurrentState != TempJewControler.State.CaughtByEnemy)
            {
                var currentColor = jewMaterial.color;
                currentColor.a = 1;
                jewMaterial.SetColor("Color", currentColor);
                fadeTweener.Kill();
                success = false;
            }
        });

        return success;
    }

    private void StopTainting(GameObject jew, bool success)
    {
        _enemyAnimator.SetBool("isTainting", false);

        _isTainting = false;

        if (success) // Destroy Jew if tainting uninterrupted
        {
            GameManager.Instance.KillJew(jew);
        }
    }

    private void TaintingProcess(GameObject jew)
    {
        // case 2
        bool successfullyTainted = StartTainting(jew);
        StopTainting(jew, successfullyTainted);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Catch Jew if he's free
        if (other.tag.Equals("Jew"))
        {
            Debug.Log("Collided with jew");
            if (other.gameObject.GetComponent<TempJewControler>().CurrentState == TempJewControler.State.Free)
            {
                TaintingProcess(other.gameObject);
            }
        }
    }
}
