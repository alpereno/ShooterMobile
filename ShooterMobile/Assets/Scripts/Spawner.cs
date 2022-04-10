using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    //public event System.Action onNewWave;
    public event System.Action<int> onNewWave;
    public bool devMode;
    public Wave[] waves;
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private Transform playerT;

    [Header("Map Spawn Points")]
    [SerializeField] Transform playerSpawnPoint;
    [SerializeField] Transform[] enemySpawnPoints;

    Wave currentWave;
    int currentWaveNumber;
    int enemiesRemainingToSpawn;                //keep track how many enemies are remaining to spawn
    float nextSpawnTime;
    float enemiesRemainingAlive;


    private void Start()
    {
        nextWave();
    }

    private void Update()
    {
        //basicly on a timer spawn however many enemies are in our current wave
        if ((enemiesRemainingToSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime)
        {
            enemiesRemainingToSpawn--;
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

            int randomSpawnPointNumber = Random.Range(0, enemySpawnPoints.Length);

            Vector3 spawnPoint = enemySpawnPoints[randomSpawnPointNumber].position;

            Enemy spawnedEnemy = Instantiate(enemyPrefab, spawnPoint, Quaternion.identity) as Enemy;
            spawnedEnemy.onDeath += onEnemyDeath;
            spawnedEnemy.setEnemyProperties(currentWave.enemyHealth, currentWave.enemyMovementSpeed, currentWave.hitsNumberToKillPlayer,
                currentWave.enemySkinColor);
        }

        if (devMode)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    GameObject.Destroy(enemy.gameObject);
                }
                nextWave();
            }
        }
    }

    void onEnemyDeath() {
        enemiesRemainingAlive--;

        if (enemiesRemainingAlive == 0)
        {
            nextWave();
        }
    }

    void nextWave() {
        currentWaveNumber++;
        if (currentWaveNumber-1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;

            resetPlayerPosition();

            if (onNewWave != null)
            {
                onNewWave(currentWaveNumber);                   // u can use without parameter but remember delete <int> part up there
            }                                                   // arrange the player health with this event and change map

            //if (onNewWave != null)
            //{
            //    onNewWave();
            //}
        }
    }

    void resetPlayerPosition() {
        playerT.position = playerSpawnPoint.position;
    }

    [System.Serializable]
    public class Wave {
        // last wave should be infinite so the game never finish
        public bool infinite;
        //information about each wave... like how many enemies are in a wave, spawn rate each wave...
        public int enemyCount;
        public float timeBetweenSpawns;
        public float enemyHealth;
        public float enemyMovementSpeed;
        public float hitsNumberToKillPlayer;
        public Color enemySkinColor;
     // public float angularSpeed;              // turning speed
     // public float acceleration;              // if u open this comment line you have to arrange the setEnemyProperties method which is inside
                                                // of Enemy script.
    }
}
