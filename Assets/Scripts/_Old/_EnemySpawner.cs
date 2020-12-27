using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private List<GameObject> enemies;

    [SerializeField]private float TimeBetweenSpawns = 2f;
    // Start is called before the first frame update
    void Start()
    {
        enemies = new List<GameObject>();
        foreach (Transform child in gameObject.transform)
        {
            enemies.Add(child.gameObject);
        }
        StartCoroutine(SpawnEnemies());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private IEnumerator SpawnEnemies()
    {
        foreach (var enemy in enemies)
        {
            enemy.SetActive(true);
            yield return new WaitForSeconds(TimeBetweenSpawns);
        }
    }
}
