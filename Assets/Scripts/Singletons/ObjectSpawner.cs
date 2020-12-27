using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DG.Tweening;
using Singletons;
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
            if (_instance == null)
            {
                _instance = FindObjectOfType<ObjectSpawner>();
                if (_instance == null)
                {
                    Debug.LogWarning("There is no ObjectSpawner GameObject in the scene");
                }
            }

            return _instance;
        }
    }

    public Bounds GameBounds => spawnBounds;

    #endregion


    private Queue<EnemyController> _enemiesPool;
    private Queue<JewController> _jewsPool;

    [Header("Config params")] [SerializeField]
    private Bounds spawnBounds;

    [SerializeField] private int maxJews = 20;
    [SerializeField] private int maxEnemies = 20;
    [SerializeField] private float minDistanceBetweenObjects;

    [Header("Containers")] [SerializeField]
    private Transform jewsPoolContainer;

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
        Debug.Log(spawnBounds.max);
        Debug.Log(spawnBounds.min);
        _instance = this;
        _enemiesPool = new Queue<EnemyController>();
        _jewsPool = new Queue<JewController>();
        InitializeQueue(maxJews, ObjectType.Jew);
        InitializeQueue(maxEnemies, ObjectType.Enemy);
    }

    private void InitializeQueue(int numOfObject, ObjectType objNameToInitialize)
    {
        var container = objNameToInitialize == ObjectType.Jew ? jewsPoolContainer : enemiesPoolContainer;
        var newObj = Resources.Load(Enum.GetName(typeof(ObjectType), objNameToInitialize)) as GameObject;
        for (int i = 0; i < numOfObject; i++)
        {
            var obj = Instantiate(newObj, container, true);
            obj.SetActive(false);
            if (objNameToInitialize == ObjectType.Jew)
            {
                _jewsPool.Enqueue(obj.GetComponent<JewController>());
            }
            else
            {
                _enemiesPool.Enqueue(obj.GetComponent<EnemyController>());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }


    public JewController SpawnJew()
    {
        if (_jewsPool.Count == 0)
        {
            Debug.LogWarning("No jews in the pool");
            return null;
        }

        var jewController = _jewsPool.Dequeue();
        SpawnObject(jewController.gameObject, jewsInGameContainer);
        return jewController;
    }
    
    public EnemyController SpawnEnemy()
    {
        if (_enemiesPool.Count == 0)
        {
            Debug.LogWarning("No enemies in the pool");
            return null;
        }
        var enemyController = _enemiesPool.Dequeue();
        SpawnObject(enemyController.gameObject, enemiesInGameContainer);
        return enemyController;
    }

    private void SpawnObject(GameObject obj, Transform inGameContainer)
    {
        var extent = new Vector3(minDistanceBetweenObjects, 0, minDistanceBetweenObjects);
        var pos = GameManager.Instance.RandomPointInBounds(spawnBounds);
        while (Physics.OverlapBox(pos, extent / 2).Length > 0)
        {
            pos = GameManager.Instance.RandomPointInBounds(spawnBounds);
        }
        obj.transform.position = pos;
        obj.transform.SetParent(inGameContainer);
        obj.SetActive(true);
    }

    public void RemoveObject(JewController jew)
    {
        jew.gameObject.SetActive(false);
        jew.transform.SetParent(jewsPoolContainer);
        _jewsPool.Enqueue(jew);
    }
    public void RemoveObject(EnemyController enemy)
    {
        enemy.gameObject.SetActive(false);
        enemy.transform.SetParent(enemiesPoolContainer);
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