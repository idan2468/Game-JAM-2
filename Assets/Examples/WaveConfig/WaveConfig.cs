using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wave Config")]
public class WaveConfig : ScriptableObject
{
    [Header("Game Obj Prefabs")]
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject pathPrefab;
    [Header("Wave Settings")]
    [SerializeField] float waveSpeed = 5f;
    [SerializeField] float spawnRandomFactor = 0.5f;
    [SerializeField] float timeBetweenSpawns = 2f;
    [SerializeField] int numOfEnemies = 5;
    public float getWaveSpeed() { return waveSpeed; }
    public GameObject getEnemyPrefab() { return enemyPrefab; }
    public List<Transform> getWayPoints()
    {
        List<Transform> wayPoints = new List<Transform>();
        foreach (Transform wayPoint in pathPrefab.transform)
        {
            wayPoints.Add(wayPoint);
        }
        return wayPoints;
    }
    public float getSpawnRandomFactor() { return spawnRandomFactor; }
    public float gettimeBetweenSpawns() { return timeBetweenSpawns; }
    public int getNumOfenemies() { return numOfEnemies; }


}
