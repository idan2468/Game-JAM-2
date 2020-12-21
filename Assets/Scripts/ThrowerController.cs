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
    private bool _isAiming; // currently aiming _target
    [SerializeField] private float _degreeBoundary = 45f; // boundary for switching A and D when facing down

    // Start is called before the first frame update
    void Start()
    {
        _throwingObjects = new List<GameObject>();
        foreach (Transform child in gameObject.transform)
        {
            _throwingObjects.Add(child.gameObject);
        }

        _isAiming = false;
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
<<<<<<< HEAD
        // If -135 <= degree <= -45 (i.e. facing down) switch A and D 
        var degree = Mathf.Atan2(-_targetOriginalForward.z, -_targetOriginalForward.x) * Mathf.Rad2Deg;
        if (Input.GetKey(KeyCode.A))
        {
            if (degree >= -135f && degree <= -45f)
            {
                _target.transform.position += _targetOriginalRight * (throwForce * Time.deltaTime);
                if (degree >= -(180f - _degreeBoundary) && degree <= -_degreeBoundary)
                {
                    _target.transform.position += _targetOriginalRight * throwForce * Time.deltaTime;
                }
                else
                {
                    _target.transform.position += _targetOriginalLeft * throwForce * Time.deltaTime;
                }
            }
            else
            {
                _target.transform.position += _targetOriginalLeft * (throwForce * Time.deltaTime);
            }
        }
        else if (Input.GetKey(KeyCode.D))
=======
        // Only move target if aiming
        if (_isAiming)
>>>>>>> parent of b729120... Removed fake throwing objects and reformated code
        {
            // If -135 <= degree <= -45 (i.e. facing down) switch A and D 
            var degree = Mathf.Atan2(-_targetOriginalForward.z, -_targetOriginalForward.x) * Mathf.Rad2Deg;
            if (Input.GetKey(KeyCode.A))
            {
                if (degree >= -135f && degree <= -45f)
                {
                    _target.transform.position += _targetOriginalRight * throwForce * Time.deltaTime;
                }
                else
                {
                    _target.transform.position += _targetOriginalLeft * throwForce * Time.deltaTime;
                }
            }
            else if (Input.GetKey(KeyCode.D))
            {
                if (degree >= -135f && degree <= -45f)
                {
                    _target.transform.position += _targetOriginalLeft * throwForce * Time.deltaTime;
                }
                else
                {
                    _target.transform.position += _targetOriginalRight * throwForce * Time.deltaTime;
                }
            }
        }

        if (Input.GetKey(KeyCode.Space))
        {
            // _currThrowingObj.SetActive(true);
            // _currThrowingObj.transform.position += transform.forward * (throwForce * Time.deltaTime);

            // Decouple _target, save directions if only started aiming
            _target.transform.parent = null;
            if (_isAiming == false)
            {
                _isAiming = true;
                _targetOriginalForward = transform.forward;
                _targetOriginalLeft = -transform.right;
                _targetOriginalRight = transform.right;
            }

            _target.SetActive(true);
            _target.transform.position += _targetOriginalForward * (throwForce * Time.deltaTime);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            _isAiming = false;
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