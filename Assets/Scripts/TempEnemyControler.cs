using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TempEnemyControler : MonoBehaviour
{
    private Vector3 _currentTargetPosition;
    [SerializeField] private float _movementSpeed = 2f;
    [SerializeField] private float _rotationSpeed = 2f;

    private bool _isTainting = false; // Enemy is in tainting state
    [SerializeField] private float _taintDuration = 5f; // Time it takes to taint a Jew

    private Animator _enemyAnimator;

    // Start is called before the first frame update
    void Start()
    {
        _enemyAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move towards closest Jew
        if(!_isTainting)
        {
            var closestJew = ObjectSpawner.Instance.GetClosestJew(transform.position);
            _currentTargetPosition = closestJew == null ? transform.position : closestJew.transform.position;
            EnemyMove();
        }
    }

    // Move enemy in direction of _currentTargetPosition
    private void EnemyMove()
    {
        // TODO: ANIMATION
        var angleToTarget = Vector3.Angle(transform.forward, _currentTargetPosition - transform.position);
        if (angleToTarget > 0)
        {
            transform.DORotate(_currentTargetPosition, angleToTarget / _rotationSpeed);
        }

        transform.DOMove(_currentTargetPosition, Vector3.Distance(_currentTargetPosition, transform.position) / _movementSpeed);
    }

    // Switch to tainting mode, add jew, start fade
    private bool StartTainting(GameObject jew)
    {
        // TODO: ANIMATION

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
                // TODO: TRY TO REFORMAT
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
        // TODO: ANIMATION

        _isTainting = false;

        if (success) // Destroy Jew if tainting uninterrupted
        {
            ObjectSpawner.Instance.KillJew(jew);
        }
    }

    private void TaintingProcess(GameObject jew)
    {
        bool successfullyTainted = StartTainting(jew);
        StopTainting(jew, successfullyTainted);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Catch Jew if he's free
        if (other.tag.Equals("Jew"))
        {
            if (other.gameObject.GetComponent<TempJewControler>().CurrentState == TempJewControler.State.Free)
            {
                TaintingProcess(other.gameObject);
            }
        }
    }
}
