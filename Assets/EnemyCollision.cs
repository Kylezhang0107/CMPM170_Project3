
using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collided with object");
        if (other.GetComponent<PixelProjectile>() != null)
        {
            Debug.Log("Projectile has hit Enemy!");

            // Destroy the enemy
            Destroy(gameObject);
        }
    }
}
