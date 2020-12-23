using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private Bounds spawnBounds;

    [SerializeField] private List<GameObject> _enemies;
    [SerializeField] private List<GameObject> _jews;

    [SerializeField] private float minDistanceBetweenObjects;

    // Start is called before the first frame update
    void Start()
    {
        _enemies = new List<GameObject>();
        _jews = new List<GameObject>();
        TestSpawn();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void TestSpawn()
    {
        var seq = DOTween.Sequence();
        var numOfObjects = 5;
        for (int i = 0; i < numOfObjects; i++)
        {
            if (Random.Range(0, 2) == 0)
            {
                seq.AppendCallback(SpawnJew);
            }
            else
            {
                seq.AppendCallback(SpawnEnemy);
            }

            seq.AppendInterval(1);
        }

        seq.Play();
    }

    public Vector3 RandomPointInBounds()
    {
        return new Vector3(
            Random.Range(spawnBounds.min.x, spawnBounds.max.x),
            Random.Range(spawnBounds.min.y, spawnBounds.max.y),
            Random.Range(spawnBounds.min.z, spawnBounds.max.z)
        );
    }

    public GameObject GetClosestJew(Vector3 pos)
    {
        var bestDist = float.PositiveInfinity;
        GameObject closestJew = null;
        foreach (var jew in _jews)
        {
            var currDist = Vector3.Distance(pos, jew.transform.position);
            if (currDist < bestDist)
            {
                bestDist = currDist;
                closestJew = jew;
            }
        }

        return closestJew;
    }

    private GameObject SpawnObject(string objName)
    {
        var newObj = Resources.Load(objName) as GameObject;
        var extent = new Vector3(minDistanceBetweenObjects, 0, minDistanceBetweenObjects);
        var pos = RandomPointInBounds();
        while (Physics.OverlapBox(pos, extent).Length > 0)
        {
            pos = RandomPointInBounds();
        }

        newObj.transform.position = pos;
        Instantiate(newObj, pos, Quaternion.identity);
        return newObj;
    }

    public void SpawnJew()
    {
        var enemy = SpawnObject("Enemy");
        _enemies.Add(enemy);
    }

    public void SpawnEnemy()
    {
        var jew = SpawnObject("Jew");
        _jews.Add(jew);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(spawnBounds.center, spawnBounds.size);
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(transform.position, new Vector3(minDistanceBetweenObjects, 0, minDistanceBetweenObjects));
    }
}