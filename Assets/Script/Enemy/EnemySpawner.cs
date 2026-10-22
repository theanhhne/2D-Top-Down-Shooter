
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private float _minimumSpawnTime = 0.5f;
    [SerializeField] private float _maximumSpawnTime = 1.5f;

    private float _timeUntilSpawn;
    private int _remainingToSpawn;
    private WaveManager _waveManager;
    private bool _isSpawning;

    private bool _isBossSpawn;
    private float _bossHealth;
    private float _bossDamage;

    public bool IsSpawning => _isSpawning;

    public void StartWave(int enemyCount, WaveManager manager)
    {
        _isBossSpawn = false;
        BeginSpawning(enemyCount, manager);
    }

    public void StartBoss(
        WaveManager manager,
        float health,
        float damage)
    {
        _isBossSpawn = true;
        _bossHealth = health;
        _bossDamage = damage;

        BeginSpawning(1, manager);
    }

    private void BeginSpawning(int enemyCount, WaveManager manager)
    {
        _waveManager = manager;
        _remainingToSpawn = enemyCount;
        _timeUntilSpawn = 0f;
        _isSpawning = enemyCount > 0;
    }

    public void StopSpawning()
    {
        _isSpawning = false;
        _remainingToSpawn = 0;
    }

    private void Update()
    {
        if (!_isSpawning) return;

        _timeUntilSpawn -= Time.deltaTime;
        if (_timeUntilSpawn > 0f) return;

        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        if (_enemyPrefab == null)
        {
            Debug.LogError("EnemySpawner chưa được gán Enemy Prefab!", this);
            StopSpawning();
            return;
        }

        GameObject enemy = Instantiate(
            _enemyPrefab,
            transform.position,
            Quaternion.identity
        );

        EnemyWaveMember member = enemy.GetComponent<EnemyWaveMember>();

        if (member == null)
        {
            Debug.LogError("Enemy Prefab cần có EnemyWaveMember!", enemy);
            Destroy(enemy);
            StopSpawning();
            return;
        }

        if (_isBossSpawn)
        {
            HealthController health = enemy.GetComponent<HealthController>();
            EnemyAttack attack = enemy.GetComponent<EnemyAttack>();

            if (health == null || attack == null)
            {
                Debug.LogError(
                    "Boss Prefab cần có HealthController và EnemyAttack!",
                    enemy
                );

                Destroy(enemy);
                StopSpawning();
                return;
            }

            health.SetMaxHealth(_bossHealth);
            attack.SetDamage(_bossDamage);
        }

        _waveManager.RegisterEnemySpawned();
        member.Initialize(_waveManager);
        if (_isBossSpawn)
{
    _waveManager.NotifyBossSpawned();
}

        _remainingToSpawn--;

        if (_remainingToSpawn <= 0)
        {
            _isSpawning = false;
        }
        else
        {
            _timeUntilSpawn = Random.Range(
                _minimumSpawnTime,
                _maximumSpawnTime
            );
        }
    }
}
