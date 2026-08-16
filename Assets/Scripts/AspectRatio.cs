using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FixedAspectRatio : MonoBehaviour
{
    private Camera cam;

    private int lastWidth;
    private int lastHeight;

    private const float targetAspect = 16f / 10f;

    void Awake()
    {
        cam = GetComponent<Camera>();
        UpdateAspect();
    }

    void Update()
    {
        // WebGL/browser can change size after startup
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            UpdateAspect();
        }
    }

    void UpdateAspect()
    {
        lastWidth = Screen.width;
        lastHeight = Screen.height;

        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        Rect rect = cam.rect;

        if (scaleHeight < 1f)
        {
            // Screen is too tall -> black bars top/bottom
            rect.x = 0;
            rect.width = 1;
            rect.height = scaleHeight;
            rect.y = (1f - scaleHeight) / 2f;
        }
        else
        {
            // Screen is too wide -> black bars left/right
            float scaleWidth = 1f / scaleHeight;

            rect.y = 0;
            rect.height = 1;
            rect.width = scaleWidth;
            rect.x = (1f - scaleWidth) / 2f;
        }

        cam.rect = rect;
    }
}