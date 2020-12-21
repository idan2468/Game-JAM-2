using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PathCreation;
using UnityEditor;
using UnityEngine.Serialization;

public class ThrowerController : MonoBehaviour
{
    [SerializeField] private float height = 5f;
    [SerializeField] private float throwForce = 2f;
    [SerializeField]private Transform startLocalTransformThrowingObj;
    [SerializeField] private GameObject _target;
    private Vector3 _orgLocalPosTargetObj;
    private List<GameObject> _throwingObjects;
    private GameObject _currThrowingObj;
    // private PathFollow _pathFollow;

   
    // Forward/left/right vectors at time of fire
    private Vector3 _targetOriginalForward; 
    private Vector3 _targetOriginalLeft; 
    private Vector3 _targetOriginalRight;
    private bool _isAiming; // currently aiming _target
    [SerializeField] private float _boundaryDegree = 45f;

    // Start is called before the first frame update
    void Start()
    {
        _isAiming = false;
        _orgLocalPosTargetObj = _target.transform.localPosition;
    }
    
    // Update is called once per frame
    void Update()
    {
        // Only move target if aiming
       

        if (Input.GetKeyDown(KeyCode.Space))
        {
            EnterAimMode();
        }
        if (_isAiming)
        {
            
            HandleAimModePhysics();
        }
       

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (_currThrowingObj == null)
            {
                return;
            }
            _isAiming = false;
            _target.transform.parent = gameObject.transform;
            
            var targetLoc = _target.transform.position;
            _target.transform.localPosition = _orgLocalPosTargetObj;
            
            _currThrowingObj.transform.SetParent(null);
            
            _target.SetActive(false);
            Throw(targetLoc);
            _currThrowingObj = null;
        }
    }

    private void HandleAimModePhysics()
    {
        // If direction is withing boundaries then switch A and D 
        var degree = Mathf.Atan2(-_targetOriginalForward.z, -_targetOriginalForward.x) * Mathf.Rad2Deg;
        if (Input.GetKey(KeyCode.A))
        {
            if (degree >= -(180f - _boundaryDegree) && degree <= -_boundaryDegree)
            {
                _target.transform.position += _targetOriginalRight * (throwForce * Time.deltaTime);
            }
            else
            {
                _target.transform.position += _targetOriginalLeft * (throwForce * Time.deltaTime);
            }
        }
        else if (Input.GetKey(KeyCode.D))
        {
            if (degree >= -135f && degree <= -45f)
            {
                _target.transform.position += _targetOriginalLeft * (throwForce * Time.deltaTime);
            }
            else
            {
                _target.transform.position += _targetOriginalRight * (throwForce * Time.deltaTime);
            }
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (_currThrowingObj == null)
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
        if (_currThrowingObj == null)
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
    public void Throw(Vector3 target)
    {
        target.y = -4.5f / 2; // for now just manual caluculation
        var start = _currThrowingObj.transform.position;
        // Debug.DrawLine(start, target, Color.red, 2f);
        var midPoint = (target + start) / 2;
        midPoint.y += height;
        // Debug.DrawLine(start, midPoint, Color.cyan, 2f);

        BezierPath path = new BezierPath(new List<Vector3>() {start, midPoint, target})
        {
            AutoControlLength = .5f, GlobalNormalsAngle = 90
        };
        var ballisticPathGO = new GameObject();
        var pc = ballisticPathGO.AddComponent<PathCreator>();
        var pathFollow = _currThrowingObj.GetComponent<PathFollow>();
        pc.bezierPath = path;
        
        pathFollow.pathCreator = pc;
        pathFollow.pathObj = ballisticPathGO;
    }

    public void AddObjToThrow(GameObject obj)
    {
        obj.transform.SetParent(transform);
        obj.transform.localPosition = startLocalTransformThrowingObj.localPosition;
        obj.transform.localRotation = startLocalTransformThrowingObj.localRotation;
        _currThrowingObj = obj;
        // _throwingObjects.Insert(0, obj);
        // SetNextThrowingObj();
    }
}