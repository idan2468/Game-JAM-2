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
        _isTainting = _jew != null;

        // Otherwise move towards closest Jew
        if(!_isTainting)
        {
            var closestJew = myObjectSpawner.GetClosestJew(transform.position);
            _currentTargetPosition = closestJew == null ? transform.position : closestJew.transform.position;
            EnemyMove();
        }
    }

    // Move enemy in direction of _currentTargetPosition
    private void EnemyMove()
    {
        // TODO: ANIMATION
        transform.position += (_currentTargetPosition - transform.position) * Time.deltaTime * _speed;
    }

    // Switch to tainting mode, add jew, start fade
    private void StartTainting(GameObject jew, bool success)
    {
        // TODO: ANIMATION

        _jew = jew;

        var jewMaterial = _jew.GetComponent<Material>();
        var fadeTweener = jewMaterial.DOFade(0, _taintDuration);
        fadeTweener.OnUpdate(() =>
        {
            // Tainting was interrupted, set alpha back to 1
            if (!_jew.GetComponent<TempJewControler>().IsCaughtByEnemy())
            {
                // TODO: TRY TO REFORMAT
                var currentColor = jewMaterial.color;
                currentColor.a = 1;
                jewMaterial.SetColor("Color", currentColor);
                fadeTweener.Kill();
                success = false;
            }
        });
    }

    private void StopTainting(bool success)
    {
        // TODO: ANIMATION

        if (success) // Destroy Jew if tainting uninterrupted
        {
            myObjectSpawner.KillJew(_jew);
        }
        _jew = null;
    }

    private void TaintingProcess(GameObject jew)
    {
        bool successfullyTainted = true;
        StartTainting(jew, successfullyTainted);
        StopTainting(successfullyTainted);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Catch Jew if he's free
        // TODO: ADD CONDITION PREVENTING TAKING JEW FROM GOLEM
        if (other.tag.Equals("Jew"))
        {
            if (other.GetComponent<TempJewControler>().IsFree())
            {
                TaintingProcess(other.gameObject);
            }
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
