using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Singletons
{
    public class GameManager : Singleton<GameManager>
    {
        [Header("Objects in game")] [SerializeField]
        private List<TempJewControler> _jewsInGame;

        [SerializeField] private List<TempEnemyControler> _enemiesInGame;
        [SerializeField] private float spawnOffset = 5f;

        // Start is called before the first frame update
        public Bounds GameBounds => ObjectSpawner.Instance.GameBounds;

        void Start()
        {
            _jewsInGame = new List<TempJewControler>();
            _enemiesInGame = new List<TempEnemyControler>();
            TestSpawn();
        }

        private void TestSpawn()
        {
            var seq = DOTween.Sequence();
            // var numOfObjects = 10;
            // for (int i = 0; i < numOfObjects; i++)
            // {
            //     if (Random.Range(0, 2) == 0)
            //     {
            //         seq.AppendCallback(SpawnJew);
            //     }
            //     else
            //     {
            //         seq.AppendCallback(SpawnEnemy);
            //     }
            //
            //     seq.AppendInterval(1);
            // }
            seq.AppendCallback(SpawnJew);
            // seq.AppendCallback(SpawnEnemy);
            seq.Play();
        }

        public Vector3 RandomPointInBounds(Bounds bounds)
        {
            var x = Random.Range(bounds.min.x, bounds.max.x);
            var y = Random.Range(bounds.min.y, bounds.max.y);
            var z = Random.Range(bounds.min.z, bounds.max.z);
            x = Mathf.Clamp(x, ObjectSpawner.Instance.GameBounds.min.x + spawnOffset,
                ObjectSpawner.Instance.GameBounds.max.x - spawnOffset);
            y = Mathf.Clamp(y, ObjectSpawner.Instance.GameBounds.min.y,
                ObjectSpawner.Instance.GameBounds.max.y);
            z = Mathf.Clamp(z, ObjectSpawner.Instance.GameBounds.min.x+spawnOffset,
                ObjectSpawner.Instance.GameBounds.max.x-spawnOffset);
            return new Vector3(x, y, z);
        }

        public GameObject GetClosestFreeJew(Vector3 pos)
        {
            var bestDist = float.PositiveInfinity;
            TempJewControler closestJew = null;
            var foundJew = false;
            foreach (var jew in _jewsInGame)
            {
                if (jew.CurrentState != TempJewControler.State.Free)
                {
                    continue;
                }

                var currDist = Vector3.Distance(pos, jew.transform.position);
                if (currDist < bestDist)
                {
                    bestDist = currDist;
                    closestJew = jew;
                    foundJew = true;
                }
            }

            return foundJew ? closestJew.gameObject : null;
        }

        public void SpawnJew()
        {
            _jewsInGame.Add(ObjectSpawner.Instance.SpawnJew());
        }

        public void SpawnEnemy()
        {
            _enemiesInGame.Add(ObjectSpawner.Instance.SpawnEnemy());
        }

        public void KillJew(GameObject jew)
        {
            Debug.Log("Killing Jew");
            var jewController = _jewsInGame.Find((item) => item.gameObject == jew);
            _jewsInGame.Remove(jewController);
            ObjectSpawner.Instance.RemoveObject(jewController);
        }

        public void KillEnemy(GameObject enemy)
        {
            Debug.Log("Killing Enemy");
            var enemyController = _enemiesInGame.Find((item) => item.gameObject == enemy);
            _enemiesInGame.Remove(enemyController);
            ObjectSpawner.Instance.RemoveObject(enemyController);
        }
    }
}