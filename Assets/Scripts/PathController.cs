using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

public class PathController : MonoBehaviour
{
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos;
    private List<Transform> _wayPoints;
    private VertexPath _myPath;

    // Start is called before the first frame update
    void Start()
    {
        _wayPoints = new List<Transform>();
        _wayPoints.AddRange(new []{startPos,endPos});
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateBezierPath()
    {
        var myBzPath = new BezierPath(_wayPoints, false, PathSpace.xyz);
        _myPath = new VertexPath(myBzPath, transform);
        
    }
}
