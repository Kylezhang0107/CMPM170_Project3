
using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        PixelProjectile projectile = other.GetComponent<PixelProjectile>();

        if (projectile != null)
        {
            FindAnyObjectByType<ScoreManager>().AddScore(50);
            Destroy(projectile.gameObject);
            Destroy(gameObject);
        }
        if(player != null)
        {
            FindAnyObjectByType<ScoreManager>().AddScore(-100);
            Destroy(gameObject);
        }
    }
}
