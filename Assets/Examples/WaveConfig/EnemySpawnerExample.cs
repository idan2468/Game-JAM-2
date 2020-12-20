using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Examples
{
    public class EnemySpawnerExample : MonoBehaviour
    {
        private const int TIME_BETWEEN_WAVES_CYCLES = 3;

        // Start is called before the first frame update
        [SerializeField] List<WaveConfig> waveConfigs;
        int currWaveIndex = 0;
        [SerializeField] private bool looping = false;
        private float timeBetweenWaves = 0.5f;
        [SerializeField]int numOfEnemies = 0;
        IEnumerator Start()
        {
            do { yield return StartCoroutine(spawnAllWaves()); }
            while (looping);
        }

        IEnumerator spawnAllWaves()
        {
            foreach (WaveConfig waveConfig in waveConfigs)
            {
                yield return StartCoroutine(spawnAllWaveEnemies(waveConfigs[currWaveIndex]));
                yield return new WaitForSeconds(timeBetweenWaves);
                currWaveIndex = currWaveIndex + 1 == waveConfigs.Count ? 0 : currWaveIndex + 1;
            }
        }

        IEnumerator spawnAllWaveEnemies(WaveConfig waveConfig)
        {
            var numOfEnemies = waveConfig.getNumOfenemies();
            for (int i = 0; i < numOfEnemies; ++i)
            {
                List<Transform> wayPoints = waveConfig.getWayPoints();
                var newEnemy = Instantiate(waveConfig.getEnemyPrefab(),
                    new Vector3(),
                    Quaternion.Euler(0, 0, 270));
                newEnemy.GetComponent<EnemyPathing>().setWaveConfig(waveConfig);
                yield return new WaitForSeconds(waveConfig.gettimeBetweenSpawns());
            }
        }
        // Update is called once per frame
        void Update()
        {
        }
        public void addEnemy()
        {
            numOfEnemies += 1;
        }
        public void removeEnemy()
        {
            numOfEnemies -= 1;
            if (numOfEnemies == 0)
            {
                StartCoroutine(startOver());
            }
        }
        IEnumerator startOver()
        {
            yield return new WaitForSeconds(TIME_BETWEEN_WAVES_CYCLES);
            StartCoroutine(spawnAllWaves());
        }
    }
}
