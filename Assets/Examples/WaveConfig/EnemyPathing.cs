using System.Collections.Generic;
using UnityEngine;

public class EnemyPathing : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] List<Transform> wayPoints;
    WaveConfig waveConfig;
    float speed;
    [SerializeField] Vector3 padding;
    int currWayPointIndex = 0;
    void Start()
    {
        wayPoints = waveConfig.getWayPoints();
        gameObject.transform.position = wayPoints[currWayPointIndex].position + padding;
        speed = waveConfig.getWaveSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    public void setWaveConfig(WaveConfig waveConfig) { this.waveConfig = waveConfig; }
    private void Move()
    {
        updateNextWayPoint();
        if (currWayPointIndex < wayPoints.Count)
        {
            var step = speed * Time.deltaTime;
            var nextPos = wayPoints[currWayPointIndex].position;
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, nextPos, step);
        }
        else
        {
            //Destroy(gameObject);
            currWayPointIndex = 0;
            gameObject.transform.position = wayPoints[0].position;
        }
    }

    private void updateNextWayPoint()
    {
        if (gameObject.transform.position == wayPoints[currWayPointIndex].position)
        {
            currWayPointIndex++;
        }

    }
}
