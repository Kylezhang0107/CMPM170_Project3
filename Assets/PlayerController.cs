using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PixelProjectile projectilePrefab;
    [SerializeField] private float moveSpeed = 5.5f;
    [SerializeField] private float fireInterval = 0.25f;

    private Rigidbody2D body;
    private ScoreManager scoreManager;
    private float trackDistance;
    private float currentPerimeter;
    private float nextFireTime;
    private float movementInput;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (body == null || collider == null)
        {
            Debug.LogError("Player needs a Rigidbody2D and CircleCollider2D in the scene.", this);
            enabled = false;
            return;
        }

        if (projectilePrefab == null)
        {
            Debug.LogError("PlayerController needs a projectile prefab assigned.", this);
            enabled = false;
            return;
        }

        scoreManager = FindAnyObjectByType<ScoreManager>();
        currentPerimeter = PolygonPath.Perimeter;
        trackDistance = currentPerimeter * 0.75f;
        UpdatePose();
    }

    private void Update()
    {
        if (scoreManager != null && scoreManager.IsGameOver)
        {
            return;
        }

        movementInput = GetHorizontalInput();

        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.isPressed &&
            Time.time >= nextFireTime)
        {
            FireProjectile(PolygonPath.InwardAt(trackDistance));
            nextFireTime = Time.time + fireInterval;
        }
    }

    private void FixedUpdate()
    {
        if (scoreManager != null && scoreManager.IsGameOver)
        {
            return;
        }

        if (currentPerimeter > 0f && !Mathf.Approximately(currentPerimeter, PolygonPath.Perimeter))
        {
            trackDistance = trackDistance / currentPerimeter * PolygonPath.Perimeter;
            currentPerimeter = PolygonPath.Perimeter;
        }

        trackDistance = Mathf.Repeat(
            trackDistance + movementInput * moveSpeed * Time.fixedDeltaTime,
            PolygonPath.Perimeter);
        UpdatePose();
    }

    public void TakeDamage(int amount)
    {
        if (scoreManager != null)
        {
            scoreManager.TakeDamage(amount);
        }
    }

    private void UpdatePose()
    {
        if (PolygonPath.Perimeter <= 0f)
        {
            return;
        }

        Vector2 position = PolygonPath.PositionAt(trackDistance);
        Vector2 inward = PolygonPath.InwardAt(trackDistance);

        if (body != null)
        {
            body.MovePosition(position);
        }
        else
        {
            transform.position = position;
        }

        float angle = Mathf.Atan2(inward.y, inward.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void FireProjectile(Vector2 direction)
    {
        Vector3 spawnPosition = transform.position + (Vector3)(direction * 0.55f);
        PixelProjectile projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        projectile.Initialize(direction);
    }

    private float GetHorizontalInput()
    {
        if (Keyboard.current == null)
        {
            return 0f;
        }

        bool left = Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed;
        bool right = Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed;
        return left == right ? 0f : right ? 1f : -1f;
    }
}
