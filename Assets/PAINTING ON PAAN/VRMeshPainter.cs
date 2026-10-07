using UnityEngine;

public class VRMeshPainter : MonoBehaviour
{
    [Header("Assign these in Inspector")]
    public Transform controllerTip; 
    public RenderTexture paintTexture; 
    public int brushSize = 8; 

    private Texture2D tempTexture;

    void Start()
    {
        if (paintTexture == null)
        {
            Debug.LogError("Please assign the PaintCanvas RenderTexture to the script!");
            return;
        }
        tempTexture = new Texture2D(paintTexture.width, paintTexture.height, TextureFormat.RGBA32, false);
        ClearCanvas();
    }

    void Update()
    {
        if (controllerTip == null) return;

        // Checks for VR trigger button or Left-Mouse-Click on PC for testing
        if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.JoystickButton14)) 
        {
            Ray ray = new Ray(controllerTip.position, controllerTip.forward);
            
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector2 uv = hit.textureCoord;
                
                // If we didn't hit a valid unwrapped UV space, skip
                if (uv == Vector2.zero) return;

                int pixelX = (int)(uv.x * paintTexture.width);
                int pixelY = (int)(uv.y * paintTexture.height);

                RenderTexture.active = paintTexture;
                tempTexture.ReadPixels(new Rect(0, 0, paintTexture.width, paintTexture.height), 0, 0);

                // Draw a small square brush stroke
                for (int x = -brushSize; x < brushSize; x++)
                {
                    for (int y = -brushSize; y < brushSize; y++)
                    {
                        int targetX = Mathf.Clamp(pixelX + x, 0, paintTexture.width - 1);
                        int targetY = Mathf.Clamp(pixelY + y, 0, paintTexture.height - 1);
                        tempTexture.SetPixel(targetX, targetY, Color.red);
                    }
                }

                tempTexture.Apply();
                Graphics.Blit(tempTexture, paintTexture);
                RenderTexture.active = null;
            }
        }
    }

    void ClearCanvas()
    {
        RenderTexture.active = paintTexture;
        GL.Clear(true, true, Color.white); // Starts canvas as pure white
        RenderTexture.active = null;
    }
}