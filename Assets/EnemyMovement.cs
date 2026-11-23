using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private const int ContactScorePenalty = 100;

    private float moveSpeed;
    private int hitPoints;
    private int contactDamage;
    private EnemySpawner spawner;
    private PlayerController player;
    private ScoreManager scoreManager;
    private Rigidbody2D body;
    private Collider2D enemyCollider;
    private Vector3 finalScale;
    private float emergenceDuration;
    private float emergenceElapsed;
    private bool emerging;
    private bool removed;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    public void Initialize(
        float speed,
        int health,
        int damageOnContact,
        EnemySpawner owner,
        PlayerController target,
        ScoreManager score,
        float emergenceTime)
    {
        moveSpeed = speed;
        hitPoints = health;
        contactDamage = damageOnContact;
        spawner = owner;
        player = target;
        scoreManager = score;
        enemyCollider = GetComponent<Collider2D>();
        finalScale = transform.localScale;
        emergenceDuration = Mathf.Max(0.01f, emergenceTime);
        emergenceElapsed = 0f;
        emerging = true;

        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        transform.localScale = finalScale * 0.12f;
    }

    public void TakeDamage()
    {
        if (removed)
        {
            return;
        }

        hitPoints--;
        if (hitPoints <= 0)
        {
            RemoveEnemy(true);
        }
    }

    private void FixedUpdate()
    {
        if (removed || player == null)
        {
            return;
        }

        Vector2 targetPosition = player.transform.position;
        Vector2 currentPosition = body != null ? body.position : (Vector2)transform.position;
        Vector2 direction = targetPosition - currentPosition;

        if (emerging)
        {
            emergenceElapsed = Mathf.Min(
                emergenceElapsed + Time.fixedDeltaTime,
                emergenceDuration);
            float progress = emergenceElapsed / emergenceDuration;
            float easedProgress = progress * progress * (3f - 2f * progress);
            transform.localScale = Vector3.Lerp(finalScale * 0.12f, finalScale, easedProgress);
            if (progress >= 1f)
            {
                emerging = false;
                if (enemyCollider != null)
                {
                    enemyCollider.enabled = true;
                }
            }
        }

        if (direction.sqrMagnitude > 0.0001f)
        {
            Vector2 nextPosition = Vector2.MoveTowards(
                currentPosition,
                targetPosition,
                moveSpeed * Time.fixedDeltaTime);
            if (body != null)
            {
                body.MovePosition(nextPosition);
            }
            else
            {
                transform.position = nextPosition;
            }

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (removed)
        {
            return;
        }

        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            scoreManager.AddScore(-ContactScorePenalty);
            player.TakeDamage(contactDamage);
            RemoveEnemy(false);
        }
    }

    private void RemoveEnemy(bool defeated)
    {
        removed = true;
        if (spawner != null)
        {
            spawner.EnemyRemoved(this, defeated);
        }

        Destroy(gameObject);
    }
}
