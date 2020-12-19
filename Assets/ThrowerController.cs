using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class ThrowerController : MonoBehaviour
{
    [SerializeField] private float height = 5f;
    [SerializeField] private float throwForce = 2f;
    private Vector3 orgLocalPosThrowObj;

    private PathFollow _pathFollow;
    // Start is called before the first frame update
    void Start()
    {
        orgLocalPosThrowObj = transform.localPosition;
        _pathFollow = GetComponent<PathFollow>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            transform.position += gameObject.transform.forward * (throwForce * Time.deltaTime);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            var targetLoc = gameObject.transform.position;
            transform.localPosition = orgLocalPosThrowObj;
            Throw(targetLoc);
        }
    }

    public void Throw(Vector3 target)
    {
        var start = gameObject.transform.position;
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