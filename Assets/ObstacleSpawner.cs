using System;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject obstaclePrefab;

    private float arenaRadius = 6.8f;

    void Start()
    {
        InvokeRepeating("SpawnObstacle", 1, 2);
    }

    void SpawnObstacle()
    {   

        Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * (arenaRadius - 2);
        Instantiate(obstaclePrefab, Vector2.zero + randomOffset, Quaternion.identity);
    }
}
