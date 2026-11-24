using UnityEngine;

public class PixelProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    private Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        if (body == null)
        {
            Debug.LogError("PixelProjectile prefab needs a Rigidbody2D.", this);
            enabled = false;
        }
    }

    public void Initialize(Vector2 travelDirection)
    {
        if (body == null)
        {
            return;
        }

        Vector2 direction = travelDirection.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        body.linearVelocity = direction * speed;
    }

    private void Update()
    {
        if (!PolygonPath.ContainsPoint(transform.position))
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Obstacle"))
    {
        Destroy(gameObject);
    }
}
}
