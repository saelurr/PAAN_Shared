using UnityEngine;

public class LeafPainter : MonoBehaviour
{
    public RenderTexture paintCanvas;
    public Material brushMaterial;

    float lastStampTime;
    const float StampInterval = 0.01f;

    void Start()
    {
        RenderTexture.active = paintCanvas;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = null;

        GetComponent<Renderer>().material.SetTexture("_ChunnaMask", paintCanvas);
    }

    public void StampAt(Vector2 uv)
    {
        if (Time.time - lastStampTime < StampInterval) return;
        lastStampTime = Time.time;

        RenderTexture.active = paintCanvas;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, paintCanvas.width, paintCanvas.height, 0);

        Vector2 pixelPos = new Vector2(uv.x * paintCanvas.width, (1 - uv.y) * paintCanvas.height);
        float brushSize = 40f;

        Graphics.DrawTexture(
            new Rect(pixelPos.x - brushSize / 2, pixelPos.y - brushSize / 2, brushSize, brushSize),
            brushMaterial.mainTexture,
            brushMaterial
        );

        GL.PopMatrix();
        RenderTexture.active = null;
    }
}