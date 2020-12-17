using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEngine.Serialization;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private PathCreator pathCreated;
    [SerializeField] private float speed = 5f;
    private float distanceTraveled;

    // Start is called before the first frame update
    void Start()
    {
        distanceTraveled = 0;
    }

    // Update is called once per frame
    void Update()
    {
        distanceTraveled += speed * Time.deltaTime;
        transform.position = pathCreated.path.GetPointAtDistance(distanceTraveled);
        transform.rotation = pathCreated.path.GetRotationAtDistance(distanceTraveled);
        var x = transform.eulerAngles.x;
        var y = transform.eulerAngles.y;
        var z = transform.eulerAngles.z;
        transform.rotation = Quaternion.Euler(x, y + 180, z);
    }
}