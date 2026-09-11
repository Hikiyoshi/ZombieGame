using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private List<ZombiePrefabEntry> zombiePrefabs;

    [Header("Settings")]
    [SerializeField] private List<Wave> waves;
    [SerializeField] private float timePerSpawn = 5f;

    private int _currentWave;
    private int _totalCount;
    private int _aliveCount;
    private Dictionary<ZombieType, GameObject> _prefabLookup;

    private void Awake()
    {
        _prefabLookup = new Dictionary<ZombieType, GameObject>();
        foreach (var entry in zombiePrefabs)
        {
            _prefabLookup[entry.ZombieType] = entry.Prefab;
        }
    }

    private void Start()
    {
        StartCoroutine(RunWaves());
    }

    private IEnumerator RunWaves()
    {
        while (_currentWave < waves.Count)
        {
            Wave wave = waves[_currentWave];

            yield return new WaitForSeconds(wave.TimeSpawn);

            StartCoroutine(SpawnWave(wave));

            _currentWave++;
        }
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        wave.TransferData();

        foreach (var pair in wave.ZombieList)
        {
            ZombieType type = pair.Key;
            int amount = pair.Value;
            _totalCount += amount;

            for (int i = 0; i < amount; i++)
            {
                SpawnZombie(type);
                yield return new WaitForSeconds(timePerSpawn);
            }
        }
    }

    private void SpawnZombie(ZombieType type)
    {
        if (!_prefabLookup.TryGetValue(type, out GameObject prefab) || prefab == null)
        {
            Debug.LogWarning($"Không tìm thấy prefab cho ZombieType: {type}");
            return;
        }

        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning("Chưa gán spawn points!");
            return;
        }

        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];
        GameObject zombie = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

        _aliveCount++;

        if (zombie.TryGetComponent<EnemyAI>(out var zombieAI))
        {
            zombieAI.OnDeath += HandleZombieDeath;
        }
    }

    private void HandleZombieDeath(ZombieType zombieType)
    {
        _aliveCount--;
        SpawnZombie(zombieType);
    }
}

[Serializable]
public class Wave
{
    public float TimeSpawn;
    public List<ZomebieAmount> ZomebieAmountList;
    public Dictionary<ZombieType, int> ZombieList = new Dictionary<ZombieType, int>();

    public void TransferData()
    {
        foreach (var pair in ZomebieAmountList)
        {
            ZombieList.Add(pair.ZomebieType, pair.Amount);
        }
    }
}

[Serializable]
public struct ZomebieAmount
{
    public ZombieType ZomebieType;
    public int Amount;
}

[Serializable]
public struct ZombiePrefabEntry
{
    public ZombieType ZombieType;
    public GameObject Prefab;
}

public enum ZombieType
{
    Normal,
    Mutation,
}