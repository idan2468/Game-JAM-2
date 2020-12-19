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
    private List<GameObject> _throwingObjects;
    private GameObject _currThrowingObj;

    private PathFollow _pathFollow;

    // Start is called before the first frame update
    void Start()
    {
        _throwingObjects = new List<GameObject>();
        foreach (Transform child in gameObject.transform)
        {
            _throwingObjects.Add(child.gameObject);
        }

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
            _throwingObjects.Add(Instantiate(_throwingObjects.First(),transform));
        }

        _currThrowingObj = _throwingObjects.First();
        _throwingObjects.RemoveAt(0);
        _orgLocalPosThrowObj = _currThrowingObj.transform.localPosition;
        _pathFollow = _currThrowingObj.GetComponent<PathFollow>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            _currThrowingObj.SetActive(true);
            _currThrowingObj.transform.position += _currThrowingObj.transform.forward * (throwForce * Time.deltaTime);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            var targetLoc = _currThrowingObj.transform.position;
            _currThrowingObj.transform.localPosition = _orgLocalPosThrowObj;
            _currThrowingObj.transform.SetParent(null);
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

        BezierPath path = new BezierPath(new List<Vector3>() {start, midPoint, target});
        var ballisticPathGO = Instantiate(new GameObject());
        var pc = ballisticPathGO.AddComponent<PathCreator>();
        pc.bezierPath = path;
        pc.bezierPath.AutoControlLength = 0.5f;

        _pathFollow.pathCreator = pc;
    }
}