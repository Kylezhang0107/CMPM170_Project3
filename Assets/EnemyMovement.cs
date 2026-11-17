using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private const float MaxLifetime = 20f;

    private Vector2 direction;
    private float speed;
    private float elapsedTime;
    private Camera targetCamera;
    private Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 travelDirection, float travelSpeed, Camera cameraToTrack)
    {
        direction = travelDirection.normalized;
        speed = travelSpeed;
        targetCamera = cameraToTrack;
    }

    private void FixedUpdate()
    {
        if (body != null)
        {
            body.MovePosition(body.position + direction * speed * Time.fixedDeltaTime);
        }
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= MaxLifetime || IsOutsideCamera())
        {
            Destroy(gameObject);
        }
    }

    private bool IsOutsideCamera()
    {
        if (targetCamera == null)
        {
            return false;
        }

        Vector3 viewportPosition = targetCamera.WorldToViewportPoint(transform.position);
        return viewportPosition.z <= 0f ||
               viewportPosition.x < -0.05f ||
               viewportPosition.x > 1.05f ||
               viewportPosition.y < -0.05f ||
               viewportPosition.y > 1.05f;
    }
}
