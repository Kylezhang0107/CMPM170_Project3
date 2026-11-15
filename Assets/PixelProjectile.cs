using UnityEngine;

public class PixelProjectile : MonoBehaviour
{
    private const float PixelsPerUnit = 32f;
    private static Sprite pixelSprite;

    private Vector2 direction;
    private float speed;
    private float lifetime = 5f;
    private float elapsedTime;
    private Camera targetCamera;

    public static Sprite CreateSprite()
    {
        if (pixelSprite == null)
        {
            Texture2D texture = new Texture2D(3, 5, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.HideAndDontSave
            };

            Color32 clear = new Color32(0, 0, 0, 0);
            Color32 yellow = new Color32(255, 255, 0, 255);
            Color32[] pixels = new Color32[15];
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    bool isTip = y == 0 || y == 4;
                    pixels[y * 3 + x] = isTip && x != 1 ? clear : yellow;
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply(false, true);

            pixelSprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                PixelsPerUnit);
            pixelSprite.hideFlags = HideFlags.HideAndDontSave;
        }

        return pixelSprite;
    }

    public void Initialize(Vector2 travelDirection, float travelSpeed, Camera cameraToTrack)
    {
        direction = travelDirection.normalized;
        speed = travelSpeed;
        targetCamera = cameraToTrack;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        transform.localScale = Vector3.one * 1.5f;
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= lifetime || IsOutsideCamera())
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
