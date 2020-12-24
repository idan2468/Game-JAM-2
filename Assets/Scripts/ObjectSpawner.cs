using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectSpawner : MonoBehaviour
{

    /// <summary>
    /// The instance.
    /// </summary>
    private static ObjectSpawner _instance;

    #region Properties

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
    [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeNullComparison")]
    public static ObjectSpawner Instance
    {
        get
        {
            if ( _instance == null )
            {
                _instance = FindObjectOfType<ObjectSpawner> ();
                if (_instance == null)
                {
                    Debug.LogWarning("There is no ObjectSpawner GameObject in the scene");
                }
            }
            return _instance;
        }
    }

    #endregion
    
    
    private Queue<GameObject> _enemiesPool;
    private Queue<GameObject> _jewsPool;
    [Header("Config params")]
    [SerializeField] private Bounds spawnBounds;
    [SerializeField] private int maxJews = 20;
    [SerializeField] private int maxEnemies = 20;
    [SerializeField] private float minDistanceBetweenObjects;
    [Header("Objects in game")]
    [SerializeField] private List<GameObject> _jewsInGame;
    [SerializeField] private List<GameObject> _enemiesInGame;
    [Header("Containers")]
    [SerializeField] private Transform jewsPoolContainer;
    [SerializeField] private Transform enemiesPoolContainer;
    [SerializeField] private Transform jewsInGameContainer;
    [SerializeField] private Transform enemiesInGameContainer;

    enum ObjectType
    {
        Jew,
        Enemy
    }

    // Start is called before the first frame update
    void Start()
    {
        _instance = this;
        _enemiesPool = new Queue<GameObject>();
        _jewsPool = new Queue<GameObject>();
        _jewsInGame = new List<GameObject>();
        _enemiesInGame = new List<GameObject>();
        InitializeQueue(maxJews, ObjectType.Jew);
        InitializeQueue(maxEnemies, ObjectType.Enemy);
        TestSpawn();
    }

    private void InitializeQueue(int numOfObject, ObjectType objNameToInitialize)
    {
        var queue = objNameToInitialize == ObjectType.Jew ? _jewsPool : _enemiesPool;
        var container = objNameToInitialize == ObjectType.Jew ? jewsPoolContainer : enemiesPoolContainer;
        var newObj = Resources.Load(Enum.GetName(typeof(ObjectType), objNameToInitialize)) as GameObject;
        for (int i = 0; i < numOfObject; i++)
        {
            var obj = Instantiate(newObj, container, true);
            obj.SetActive(false);
            queue.Enqueue(obj);
        }
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

        seq.Play().OnComplete(() => KillJew(_jewsInGame[0]));
    }

    private Vector3 RandomPointInBounds()
    {
        return new Vector3(
            Random.Range(spawnBounds.min.x, spawnBounds.max.x),
            Random.Range(spawnBounds.min.y, spawnBounds.max.y),
            Random.Range(spawnBounds.min.z, spawnBounds.max.z)
        );
    }
    
    public static Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    public GameObject GetClosestJew(Vector3 pos)
    {
        var bestDist = float.PositiveInfinity;
        GameObject closestJew = null;
        foreach (var jew in _jewsInGame)
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

    private GameObject SpawnObject(ObjectType objType)
    {
        var objPoolQueue = objType == ObjectType.Jew ? _jewsPool : _enemiesPool;
        var container = objType == ObjectType.Jew ? jewsInGameContainer : enemiesInGameContainer;
        var extent = new Vector3(minDistanceBetweenObjects, 0, minDistanceBetweenObjects);
        var pos = RandomPointInBounds();
        while (Physics.OverlapBox(pos, extent / 2).Length > 0)
        {
            pos = RandomPointInBounds();
        }

        var newObj = objPoolQueue.Dequeue();
        newObj.transform.position = pos;
        newObj.transform.SetParent(container);
        newObj.SetActive(true);
        return newObj;
    }

    public void SpawnJew()
    {
        var jew = SpawnObject(ObjectType.Jew);
        _jewsInGame.Add(jew);
    }

    public void SpawnEnemy()
    {
        var jew = SpawnObject(ObjectType.Enemy);
        _enemiesInGame.Add(jew);
    }

    public void KillJew(GameObject jew)
    {
        Debug.Log("Killing Jew");
        _jewsInGame.Remove(jew);
        jew.SetActive(false);
        jew.transform.SetParent(jewsPoolContainer);
        _jewsPool.Enqueue(jew);
    }

    public void KillEnemy(GameObject enemy)
    {
        Debug.Log("Killing Enemy");
        _enemiesInGame.Remove(enemy);
        enemy.SetActive(false);
        enemy.transform.SetParent(jewsPoolContainer);
        _enemiesPool.Enqueue(enemy);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(spawnBounds.center, spawnBounds.size);
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(transform.position, new Vector3(minDistanceBetweenObjects, 0, minDistanceBetweenObjects));
    }
}