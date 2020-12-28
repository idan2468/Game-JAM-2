using System;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class ThrowerController : MonoBehaviour
{
    [SerializeField] private float height = 5f;
    [SerializeField] private float throwForce = 2f;
    [SerializeField] private Transform startLocalTransformThrowingObj;
    [SerializeField] private GameObject _target;
    private Vector3 _orgLocalPosTargetObj;
    private GameObject _currThrowingObj;

    private bool _caughtJew;

    // private PathFollow _pathFollow;
    [SerializeField] private Transform jewsInGame;


    // Forward/left/right vectors at time of fire
    private Vector3 _targetOriginalForward;
    private Vector3 _targetOriginalLeft;
    private Vector3 _targetOriginalRight;
    private bool _isAiming; // currently aiming _target
    //[SerializeField] private float _boundaryDegree = 45f; NOT BEING USED

    [SerializeField] private Animator _parentAnimator;
    public bool CanCatchJew => _currThrowingObj == null;

    // Start is called before the first frame update
    void Start()
    {
        _parentAnimator = GetComponentInParent<Animator>();
        _isAiming = false;
        _caughtJew = false;
        _orgLocalPosTargetObj = _target.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Throw"))
        {
            EnterAimMode();
        }

        // Only move target if aiming
        if (_isAiming)
        {
            HandleAimModePhysics();
        }

        if (Input.GetButtonUp("Throw"))
        {
            Debug.Log("Throw");
            if (!_caughtJew)
            {
                return;
            }

            var normDirection = (_target.transform.position - transform.position).normalized;
            transform.parent.rotation = Quaternion.LookRotation(normDirection);
            _parentAnimator.SetTrigger("Throw");
        }
    }

    /**
     * Exit Aim mode and returns the aim word position to throw the object.
     */
    private Vector3 ExitAimMode()
    {
        _isAiming = false;
        _target.transform.parent = gameObject.transform;

        var targetLoc = _target.transform.position;
        _target.transform.localPosition = _orgLocalPosTargetObj;

        _currThrowingObj.transform.SetParent(null);

        _target.SetActive(false);
        return targetLoc;
    }

    private void HandleAimModePhysics()
    {
        // If direction is withing boundaries then switch A and D 
        var cameraForwardVector = new Vector3(Camera.main.transform.localEulerAngles.x, 0f, Camera.main.transform.localEulerAngles.z);
        var degree = Vector3.SignedAngle(cameraForwardVector, _targetOriginalForward, Vector3.up);

        if (Input.GetButton("LeftAim"))
        {
            if (degree >= 45f && degree <= 225f)
            {
                _target.transform.position += _targetOriginalRight * (throwForce * Time.deltaTime);
            }
            else
            {
                _target.transform.position += _targetOriginalLeft * (throwForce * Time.deltaTime);
            }
        }
        else if (Input.GetButton("RightAim"))
        {
            if (degree >= 45f && degree <= 225f)
            {
                _target.transform.position += _targetOriginalLeft * (throwForce * Time.deltaTime);
            }
            else
            {
                _target.transform.position += _targetOriginalRight * (throwForce * Time.deltaTime);
            }
        }

        if (Input.GetButton("Throw"))
        {
            if (!_caughtJew)
            {
                return;
            }

            _target.transform.position += _targetOriginalForward * (throwForce * Time.deltaTime);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void EnterAimMode()
    {
        // Decouple _target, save directions if only started aiming
        if (!_caughtJew)
        {
            Debug.LogWarning("No Object to throw");
            return;
        }

        _target.transform.parent = null;
        _isAiming = true;
        _targetOriginalForward = transform.forward;
        _targetOriginalLeft = -transform.right;
        _targetOriginalRight = transform.right;
        _target.SetActive(true);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    // Maybe change to DOTween
    public void Throw()
    {
        _parentAnimator.speed = 0;
        var targetLoc = ExitAimMode();
        var start = _currThrowingObj.transform.position;
        // Debug.DrawLine(start, target, Color.red, 2f);
        var midPoint = (targetLoc + start) / 2;
        midPoint.y += height;
        // Debug.DrawLine(start, midPoint, Color.cyan, 2f);

        BezierPath path = new BezierPath(new List<Vector3>() {start, midPoint, targetLoc})
        {
            AutoControlLength = .5f, GlobalNormalsAngle = 90
        };
        var ballisticPathGO = new GameObject();
        var pc = ballisticPathGO.AddComponent<PathCreator>();
        var pathFollow = _currThrowingObj.GetComponent<PathFollow>();
        pc.bezierPath = path;
        pathFollow.pathCreator = pc;
        pathFollow.pathObj = ballisticPathGO;
        _currThrowingObj.GetComponent<JewController>().EnterThrownState();
        _currThrowingObj = null;
        _caughtJew = false;
        _parentAnimator.speed = 1;
    }

    public void AddObjToThrow(GameObject obj)
    {
        if (_caughtJew) return;
        _caughtJew = true;
        jewsInGame = obj.transform.parent;
        obj.transform.SetParent(startLocalTransformThrowingObj);
        obj.transform.localPosition = startLocalTransformThrowingObj.localPosition;
        obj.transform.localRotation = startLocalTransformThrowingObj.localRotation;
        _currThrowingObj = obj;
    }
    
    public void ReleaseJew(GameObject jew)
    {
        if (jew == _currThrowingObj)
        {
            _currThrowingObj = null;
            _caughtJew = false;    
        }
    }
}