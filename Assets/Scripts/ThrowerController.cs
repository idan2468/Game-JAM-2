using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PathCreation;

public class ThrowerController : MonoBehaviour
{
    [SerializeField] private float height = 5f;
    [SerializeField] private float throwForce = 2f;
    private Vector3 _orgLocalPosThrowObj;
    private Vector3 _orgLocalPosTargetObj;
    private List<GameObject> _throwingObjects;
    private GameObject _currThrowingObj;
    private PathFollow _pathFollow;

    [SerializeField] private GameObject _target;
    // Forward/left/right vectors at time of fire
    private Vector3 _targetOriginalForward; 
    private Vector3 _targetOriginalLeft; 
    private Vector3 _targetOriginalRight;
    private bool _isTargeting; // currently aiming _target

    // Start is called before the first frame update
    void Start()
    {
        _throwingObjects = new List<GameObject>();
        foreach (Transform child in gameObject.transform)
        {
            _throwingObjects.Add(child.gameObject);
        }

        _isTargeting = false;
        _orgLocalPosTargetObj = _target.transform.localPosition;
        SetNextThrowingObj();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void SetNextThrowingObj()
    {
        if (_throwingObjects.Count == 0)
        {
            Debug.LogWarning("Finished all objects");
            return;
        }

        if (_throwingObjects.Count == 1)
        {
            _throwingObjects.Add(Instantiate(_throwingObjects.First(), transform));
        }

        _currThrowingObj = _throwingObjects.First();
        _throwingObjects.RemoveAt(0);
        _orgLocalPosThrowObj = _currThrowingObj.transform.localPosition;
        _pathFollow = _currThrowingObj.GetComponent<PathFollow>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            _target.transform.position += _targetOriginalLeft * throwForce * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            _target.transform.position += _targetOriginalRight * throwForce * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            _target.transform.parent = null;
            // _currThrowingObj.SetActive(true);
            // _currThrowingObj.transform.position += transform.forward * (throwForce * Time.deltaTime);
            if (_isTargeting == false)
            {
                _isTargeting = true;
                _targetOriginalForward = transform.forward;
                _targetOriginalLeft = -transform.right;
                _targetOriginalRight = transform.right;
            }

            _target.SetActive(true);
            _target.transform.position += _targetOriginalForward * (throwForce * Time.deltaTime);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            _isTargeting = false;
            _target.transform.parent = gameObject.transform;
            
            var targetLoc = _target.transform.position;
            _target.transform.localPosition = _orgLocalPosTargetObj;
            
            // For temporary brown spheres
            _currThrowingObj.SetActive(true);
            _currThrowingObj.transform.SetParent(null);
            
            _target.SetActive(false);
            Throw(targetLoc);
            SetNextThrowingObj();
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void Throw(Vector3 target)
    {
        target.y = -4.5f / 2;
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
        pc.bezierPath = path;

        _pathFollow.pathCreator = pc;
        _pathFollow.pathObj = ballisticPathGO;
    }

    public void AddObjToThrow(GameObject obj)
    {
        obj.transform.SetParent(transform);
        obj.transform.localPosition = _orgLocalPosThrowObj;
        _throwingObjects.Insert(0, obj);
        SetNextThrowingObj();
    }
}