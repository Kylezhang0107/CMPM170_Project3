using System;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private float spawnInterval = 0.5f; //in secs
    private float spawnRadius = 4.8f;

    void Start()
    {
        InvokeRepeating("SpawnObstacle", 1, spawnInterval);
    }

    void SpawnObstacle()
    {   

        Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * (spawnRadius);
        Instantiate(obstaclePrefab, Vector2.zero + randomOffset, Quaternion.identity);
    }
}
