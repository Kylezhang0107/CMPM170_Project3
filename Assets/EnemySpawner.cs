using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float spawnInterval = 3f;
    public Sprite enemySprite;
    public Color spriteColor = new Color32(255, 0, 255, 255);
    public int enemiesPerWave = 4;
    public float minEnemySize = 0.65f;
    public float maxEnemySize = 1.15f;
    public float baseEnemySpeed = 1.8f;

    private void Start()
    {
        if (enemySprite == null)
        {
            Debug.LogError("EnemySpawner needs an enemy sprite.", this);
            enabled = false;
            return;
        }

        InvokeRepeating(nameof(SpawnWave), spawnInterval, spawnInterval);
    }

    private void SpawnWave()
    {
        float startingAngle = Random.Range(0f, 360f);

        for (int i = 0; i < enemiesPerWave; i++)
        {
            float angle = startingAngle + i * (360f / enemiesPerWave);
            angle += Random.Range(-12f, 12f);
            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad));

            float size = Random.Range(minEnemySize, maxEnemySize);
            SpawnEnemy(direction, size, baseEnemySpeed / size);
        }
    }

    private void SpawnEnemy(Vector2 direction, float size, float speed)
    {
        GameObject enemy = new GameObject("Enemy");
        enemy.transform.position = (Vector2)transform.position + direction * 0.35f;
        enemy.transform.localScale = Vector3.one * size;

        SpriteRenderer spriteRenderer = enemy.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = enemySprite;
        spriteRenderer.color = spriteColor;

        Rigidbody2D body = enemy.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;

        CircleCollider2D collider = enemy.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.45f;

        EnemyMovement movement = enemy.AddComponent<EnemyMovement>();
        movement.Initialize(direction, speed, Camera.main);
        enemy.AddComponent<EnemyCollision>();
    }
}
