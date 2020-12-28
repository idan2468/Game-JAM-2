using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Singletons
{
    public class GameManager : Singleton<GameManager>
    {
        [Header("Spawn Timing")] [SerializeField]
        private float spawnJewMinTime;

        [SerializeField] private float spawnJewMaxTime;
        [SerializeField] private float spawnEnemyMinTime;
        [SerializeField] private float spawnEnemyMaxTime;

        [Header("Objects in game")] [SerializeField]
        private List<JewController> _jewsInGame;

        [SerializeField] private List<EnemyController> _enemiesInGame;
        [SerializeField] private ThrowerController _throwerController;
        [SerializeField] private float spawnOffset = 5f;

        [Header("Points")] [SerializeField] private int _playerScore;
        [SerializeField] private int _playerLives;

        public int PlayerLives => _playerLives;

        public Bounds GameBounds => ObjectSpawner.Instance.GameBounds;

        void Start()
        {
            ResetGameManager();
        }

        private IEnumerator SpawnJews()
        {
            while (true)
            {
                SpawnJew();
                yield return new WaitForSeconds(Random.Range(spawnJewMinTime, spawnJewMaxTime));
            }
        }

        private IEnumerator SpawnEnemies()
        {
            while (true)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(Random.Range(spawnEnemyMinTime, spawnEnemyMaxTime));
            }
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
            z = Mathf.Clamp(z, ObjectSpawner.Instance.GameBounds.min.x + spawnOffset,
                ObjectSpawner.Instance.GameBounds.max.x - spawnOffset);
            return new Vector3(x, y, z);
        }

        public JewController GetClosestFreeJew(Vector3 pos)
        {
            var bestDist = float.PositiveInfinity;
            JewController closestJew = null;
            var foundJew = false;
            foreach (var jew in _jewsInGame)
            {
                if (jew.CurrentState != JewController.State.Free)
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

            return foundJew ? closestJew : null;
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
            _throwerController.ReleaseJew(jew);
            var jewController = _jewsInGame.Find((item) => item.gameObject == jew);
            _jewsInGame.Remove(jewController);
            ObjectSpawner.Instance.RemoveObject(jewController);
        }

        public void KillEnemy(GameObject enemy)
        {
            Debug.Log("Killing Enemy");
            var enemyController = _enemiesInGame.Find((item) => item.gameObject == enemy);
            enemyController.ReleaseJew();
            _enemiesInGame.Remove(enemyController);
            ObjectSpawner.Instance.RemoveObject(enemyController);
        }

        public void AddScore()
        {
            _playerScore += 1;
            UIController.Instance.UpdateScoreUI(_playerScore);
        }

        private void EndGame()
        {
            UIController.Instance.SwitchToEndGameUI();
            UIController.Instance.SetEndGameScore(_playerScore);
        }

        public void LoseLife()
        {
            _playerLives--;
            UIController.Instance.LoseLife();
            if (_playerLives <= 0)
            {
                EndGame();
            }
        }

        public void ResetGameManager()
        {
            _playerScore = 0;
            _jewsInGame = new List<JewController>();
            _enemiesInGame = new List<EnemyController>();
            _throwerController = FindObjectOfType<ThrowerController>();
            UIController.Instance.UpdateScoreUI(_playerScore);

            // TestSpawn();
            StartCoroutine(SpawnJews());
            StartCoroutine(SpawnEnemies());
        }
    }
}