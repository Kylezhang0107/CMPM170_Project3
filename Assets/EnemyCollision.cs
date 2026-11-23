
using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        PixelProjectile projectile = collision.gameObject.GetComponent<PixelProjectile>();
        EnemyMovement enemy = GetComponent<EnemyMovement>();
        if (projectile != null && enemy != null)
        {
            enemy.TakeDamage();
            Destroy(projectile.gameObject);
        }
    }
}
