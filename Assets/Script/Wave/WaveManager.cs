
using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Normal Enemy Spawners")]
    [SerializeField] private EnemySpawner[] normalSpawners;

    [Header("Strong Mob Spawners")]
    [SerializeField] private EnemySpawner[] strongMobSpawners;

    [Header("Boss Spawner")]
    [SerializeField] private EnemySpawner bossSpawner;

    [Header("Normal Enemy Counts")]
    [SerializeField, Min(0)] private int wave1EnemyCount = 15;
    [SerializeField, Min(0)] private int wave2EnemyCount = 30;
    [SerializeField, Min(0)] private int wave3EnemyCount = 50;

    [Header("Difficulty Scaling")]
    [SerializeField, Min(0f)] private float normalEnemyGrowthPercent = 25f;
    [SerializeField, Min(1)] private int strongMobStartCount = 1;
    [SerializeField, Min(1)] private int strongMobEveryWaves = 2;

    [Header("Boss Scaling")]
    [SerializeField, Min(1)] private int bossEveryWaves = 5;
    [SerializeField, Min(1)] private float bossHealthPerMilestone = 500f;
    [SerializeField, Min(0)] private float bossDamagePerMilestone = 10f;

    [Header("Wave Settings")]
    [SerializeField, Min(0f)] private float timeBetweenWaves = 3f;
    [SerializeField, Min(1)] private int startingWave = 1;
    [Header("Wave UI")]
    [SerializeField] private WaveUI waveUI;

    public int CurrentWave => currentWave;

    private int currentWave;
    private int aliveEnemies;
    private bool waitingForNextWave;
    private bool bossSpawned;

    private void Start()
    {
        if (!ValidateSpawners()) return;
        StartNextWave();
    }

    private bool ValidateSpawners()
    {
        if (normalSpawners == null || normalSpawners.Length == 0)
        {
            Debug.LogError("Chưa gán Normal Enemy Spawners!", this);
            return false;
        }

        if (strongMobSpawners == null || strongMobSpawners.Length == 0)
        {
            Debug.LogError("Chưa gán Strong Mob Spawners!", this);
            return false;
        }

        if (bossSpawner == null)
        {
            Debug.LogError("Chưa gán Boss Spawner!", this);
            return false;
        }

        foreach (EnemySpawner spawner in normalSpawners)
        {
            if (spawner == null)
            {
                Debug.LogError("Normal Spawner bị thiếu!", this);
                return false;
            }
        }

        foreach (EnemySpawner spawner in strongMobSpawners)
        {
            if (spawner == null)
            {
                Debug.LogError("Strong Mob Spawner bị thiếu!", this);
                return false;
            }
        }

        return true;
    }

    private int GetNormalEnemyCount(int wave)
    {
        if (wave == 1) return wave1EnemyCount;
        if (wave == 2) return wave2EnemyCount;
        if (wave == 3) return wave3EnemyCount;

        float growth = 1f + normalEnemyGrowthPercent / 100f;
        float count = wave3EnemyCount;

        for (int i = 4; i <= wave; i++)
            count = Mathf.Ceil(count * growth);

        return Mathf.CeilToInt(count);
    }

    private int GetStrongMobCount(int wave)
    {
        return strongMobStartCount
            + (wave - 1) / strongMobEveryWaves;
    }

    private bool IsBossWave(int wave)
    {
        return bossEveryWaves > 0 && wave % bossEveryWaves == 0;
    }

   private void StartNextWave()
{
    if (currentWave == 0)
        currentWave = startingWave - 1;

    currentWave++;
    if (waveUI != null)
{
    waveUI.ShowWave(currentWave, IsBossWave(currentWave));
}
    aliveEnemies = 0;
    bossSpawned = false;

    int normalCount = GetNormalEnemyCount(currentWave);
    int strongCount = GetStrongMobCount(currentWave);

    Debug.Log(
        $"Wave {currentWave} started | Normal: {normalCount} | " +
        $"Strong: {strongCount} | Boss wave: {IsBossWave(currentWave)}"
    );

    StartSpawners(normalSpawners, normalCount);
    StartSpawners(strongMobSpawners, strongCount);
}

    private void StartSpawners(EnemySpawner[] spawners, int totalEnemies)
    {
        int baseCount = totalEnemies / spawners.Length;
        int remainder = totalEnemies % spawners.Length;

        for (int i = 0; i < spawners.Length; i++)
        {
            int count = baseCount + (i < remainder ? 1 : 0);
            spawners[i].StartWave(count, this);
        }
    }

    public void RegisterEnemySpawned()
    {
        aliveEnemies++;
    }

    public void OnEnemyDied(EnemyWaveMember enemy)
    {
        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);
        CheckWaveComplete();
    }

    private void Update()
    {
        CheckWaveComplete();
    }

    private void CheckWaveComplete()
    {
        if (currentWave == 0 || waitingForNextWave) return;

        foreach (EnemySpawner spawner in normalSpawners)
            if (spawner.IsSpawning) return;

        foreach (EnemySpawner spawner in strongMobSpawners)
            if (spawner.IsSpawning) return;

        if (aliveEnemies > 0) return;

        if (IsBossWave(currentWave) && !bossSpawned)
        {
            int milestone = currentWave / bossEveryWaves;
            float bossHealth = milestone * bossHealthPerMilestone;
            float bossDamage = milestone * bossDamagePerMilestone;

            bossSpawned = true;

            Debug.Log(
                $"Boss Round {currentWave} | HP: {bossHealth} | " +
                $"Damage: {bossDamage}"
            );

            bossSpawner.StartBoss(this, bossHealth, bossDamage);
            return;
        }

        waitingForNextWave = true;
        StartCoroutine(NextWaveAfterDelay());
    }
    public void NotifyBossSpawned()
{
    if (waveUI != null)
    {
        waveUI.ShowBossWarning();
        Debug.Log("BOSS WARNING CALLED");
    }
    else
    {
        Debug.LogError("WaveUI chưa được gán trong WaveManager!");
    }
}
    private IEnumerator NextWaveAfterDelay()
    {
        waitingForNextWave = true;

        Debug.Log($"Wave {currentWave} completed!");

        yield return new WaitForSeconds(timeBetweenWaves);

        waitingForNextWave = false;
        StartNextWave();
    }
}
