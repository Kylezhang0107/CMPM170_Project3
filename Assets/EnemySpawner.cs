using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyMovement enemyPrefab;
    [SerializeField] private float arenaRadius = 6.8f;
    [SerializeField] private float enemySpeed = 1.4f;
    [SerializeField] private float spawnDelay = 0.8f;
    [SerializeField] private float emergenceDuration = 2.4f;

    private static readonly int[] EnemiesPerLevel = { 5, 7, 9 };

    private readonly List<GameObject> enemies = new List<GameObject>();
    private ScoreManager scoreManager;
    private PlayerController player;
    private LineRenderer arenaLine;
    private int aliveEnemies;

    private void Awake()
    {
        PolygonPath.Configure(5, Vector2.zero, arenaRadius);
    }

    private void Start()
    {
        scoreManager = FindAnyObjectByType<ScoreManager>();
        if (scoreManager == null)
        {
            Debug.LogError("EnemySpawner needs a ScoreManager in the scene.", this);
            enabled = false;
            return;
        }

        player = FindAnyObjectByType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("EnemySpawner needs a PlayerController in the scene.", this);
            enabled = false;
            return;
        }

        if (enemyPrefab == null)
        {
            Debug.LogError("EnemySpawner needs an enemy prefab assigned.", this);
            enabled = false;
            return;
        }

        CreateArenaLine();
        StartCoroutine(PlayLevels());
    }

    public void EnemyRemoved(EnemyMovement enemy, bool defeated)
    {
        if (enemies.Remove(enemy.gameObject))
        {
            aliveEnemies--;
            if (defeated)
            {
                scoreManager.AddScore(50);
            }
        }
    }

    private IEnumerator PlayLevels()
    {
        for (int level = 1; level <= EnemiesPerLevel.Length && !scoreManager.IsGameOver; level++)
        {
            PolygonPath.Configure(4 + level, Vector2.zero, arenaRadius);
            UpdateArenaLine();
            scoreManager.SetLevel(level);

            int enemyCount = EnemiesPerLevel[level - 1];
            aliveEnemies = 0;
            enemies.Clear();

            for (int i = 0; i < enemyCount && !scoreManager.IsGameOver; i++)
            {
                SpawnEnemy(i);
                yield return new WaitForSeconds(spawnDelay);
            }

            while (aliveEnemies > 0 && !scoreManager.IsGameOver)
            {
                yield return null;
            }

            if (scoreManager.IsGameOver)
            {
                yield break;
            }

            if (level < EnemiesPerLevel.Length)
            {
                scoreManager.ShowStatus("Garden cleared!\nNext polygon...");
                yield return new WaitForSeconds(1.5f);
            }
        }

        if (!scoreManager.IsGameOver)
        {
            scoreManager.FinishGame();
        }
    }

    private void SpawnEnemy(int index)
    {
        bool isLarge = index % 3 == 0;
        float size = isLarge ? 1.05f : 0.68f;

        EnemyMovement movement = Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity);
        GameObject enemy = movement.gameObject;
        enemy.name = isLarge ? "Large Centipede" : "Small Centipede";
        enemy.transform.localScale = Vector3.one * size;

        movement.Initialize(
            isLarge ? enemySpeed * 0.72f : enemySpeed * 1.12f,
            isLarge ? 2 : 1,
            isLarge ? 2 : 1,
            this,
            player,
            scoreManager,
            emergenceDuration);

        enemies.Add(enemy);
        aliveEnemies++;
    }

    private void CreateArenaLine()
    {
        GameObject lineObject = new GameObject("Polygon Arena");
        arenaLine = lineObject.AddComponent<LineRenderer>();
        arenaLine.useWorldSpace = true;
        arenaLine.loop = true;
        arenaLine.widthMultiplier = 0.08f;
        arenaLine.positionCount = PolygonPath.Sides;
        arenaLine.startColor = new Color(0.3f, 1f, 0.85f, 1f);
        arenaLine.endColor = arenaLine.startColor;
        arenaLine.sortingOrder = -1;

        Shader shader = Shader.Find("Sprites/Default");
        if (shader == null)
        {
            Debug.LogError("Could not find the Sprites/Default shader for the polygon outline.", this);
            return;
        }

        arenaLine.material = new Material(shader);
        UpdateArenaLine();
    }

    private void UpdateArenaLine()
    {
        if (arenaLine == null)
        {
            return;
        }

        arenaLine.positionCount = PolygonPath.Sides;
        for (int i = 0; i < PolygonPath.Sides; i++)
        {
            arenaLine.SetPosition(i, PolygonPath.VertexAt(i));
        }
    }
}

public static class PolygonPath
{
    private static Vector2 center;
    private static Vector2[] vertices = new Vector2[0];
    private static float[] segmentLengths = new float[0];
    private static float[] cumulativeLengths = new float[0];

    public static int Sides { get; private set; }
    public static float Radius { get; private set; }
    public static float Perimeter { get; private set; }

    public static void Configure(int sides, Vector2 polygonCenter, float radius)
    {
        Sides = Mathf.Max(3, sides);
        Radius = Mathf.Max(0.1f, radius);
        center = polygonCenter;
        vertices = new Vector2[Sides];
        segmentLengths = new float[Sides];
        cumulativeLengths = new float[Sides + 1];
        Perimeter = 0f;

        for (int i = 0; i < Sides; i++)
        {
            float angle = (-90f + i * 360f / Sides) * Mathf.Deg2Rad;
            vertices[i] = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Radius;
        }

        for (int i = 0; i < Sides; i++)
        {
            segmentLengths[i] = Vector2.Distance(vertices[i], vertices[(i + 1) % Sides]);
            Perimeter += segmentLengths[i];
            cumulativeLengths[i + 1] = Perimeter;
        }
    }

    public static Vector2 PositionAt(float distance)
    {
        GetSegment(distance, out int segment, out float fraction);
        return Vector2.Lerp(vertices[segment], vertices[(segment + 1) % Sides], fraction);
    }

    public static Vector2 TangentAt(float distance)
    {
        GetSegment(distance, out int segment, out _);
        return (vertices[(segment + 1) % Sides] - vertices[segment]).normalized;
    }

    public static Vector2 InwardAt(float distance)
    {
        Vector2 tangent = TangentAt(distance);
        return new Vector2(-tangent.y, tangent.x);
    }

    public static Vector2 VertexAt(int index)
    {
        return vertices[index % Sides];
    }

    public static bool ContainsPoint(Vector2 point)
    {
        bool inside = false;
        for (int i = 0, j = Sides - 1; i < Sides; j = i++)
        {
            Vector2 a = vertices[i];
            Vector2 b = vertices[j];
            bool crosses = (a.y > point.y) != (b.y > point.y);
            if (crosses &&
                point.x < (b.x - a.x) * (point.y - a.y) / (b.y - a.y) + a.x)
            {
                inside = !inside;
            }
        }

        return inside;
    }

    private static void GetSegment(float distance, out int segment, out float fraction)
    {
        float wrappedDistance = Mathf.Repeat(distance, Perimeter);
        segment = 0;
        while (segment < Sides - 1 && wrappedDistance >= cumulativeLengths[segment + 1])
        {
            segment++;
        }

        float segmentDistance = wrappedDistance - cumulativeLengths[segment];
        fraction = segmentDistance / segmentLengths[segment];
    }
}
