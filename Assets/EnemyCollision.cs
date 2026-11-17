
using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        PixelProjectile projectile = other.GetComponent<PixelProjectile>();
        if (projectile != null)
        {
            Destroy(projectile.gameObject);
            Destroy(gameObject);
        }
    }
}
