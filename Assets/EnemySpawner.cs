using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float spawnInterval = 2f;
    public Sprite enemySprite;
    public Color spriteColor = new Color32(255,0,255,255);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy),spawnInterval,spawnInterval);
    }

    void SpawnEnemy(){

        Debug.Log("Enemy Spawned");

        GameObject enemy = new GameObject("Enemy");

        SpriteRenderer sr = enemy.AddComponent<SpriteRenderer>();
        sr.sprite = enemySprite;
        sr.color = spriteColor;

        enemy.transform.position = new Vector2(0,0);
        
        // Add collider
        CircleCollider2D collider = enemy.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        enemy.AddComponent<EnemyCollision>();
    }
}
