using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScreenDraw : MonoBehaviour
{

    private static ScreenDraw _instance;
    public static ScreenDraw instance
    {
        get { return _instance; }
        protected set { _instance = value; }
    }

    private Camera cam;

    public Shader drawShader;
    Material drawMaterial;

    public Shader cleanShader;
    Material cleanMaterial;

    public Shader compositeShader;
    Material compositeMaterial;

    RenderTexture screenRT;

    public Texture2D drawTexture;
    public Texture2D cleanTexture;

    bool clear = false;

    Color color;

    void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(this);
    }

    void Start()
    {
        cam = GetComponent<Camera>();

        color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        drawMaterial = new Material(drawShader);
        cleanMaterial = new Material(cleanShader);
        compositeMaterial = new Material(compositeShader);

        screenRT = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
        screenRT.Create();

        Clear();
    }

    void Update()
    {
        if (clear)
        {
            Clear();

            clear = false;
        }

        if (Input.mouseScrollDelta.y != 0.0f)
        {
            float x = Input.mouseScrollDelta.y * 0.1f;
            SetColor(new Color(color.r + x, color.g + x, color.b + x, 1.0f));
        }

        if (Input.GetMouseButton(0))
        {
            Draw(new Vector3(Input.mousePosition.x / Screen.width, 1.0f - (Input.mousePosition.y / Screen.height), Input.mousePosition.z));
        }

        if (Input.GetMouseButton(1))
        {
            Clean(new Vector3(Input.mousePosition.x / Screen.width, 1.0f - (Input.mousePosition.y / Screen.height), Input.mousePosition.z));
        }
    }

    public void Clear()
    {
        screenRT.DiscardContents();

        Graphics.SetRenderTarget(screenRT);
        GL.Clear(false, true, new Color32(0, 0, 0, 0));
    }

    public void Draw(Vector3 viewPos)
    {
        Vector3 drawScale = new Vector3(0.1f, 0.1f, 1.0f);
        Matrix4x4 mat = Matrix4x4.TRS(new Vector3(viewPos.x - drawScale.x * 0.5f, viewPos.y - drawScale.y * 0.5f, 0.0f), Quaternion.identity, drawScale);

        screenRT.MarkRestoreExpected();

        Graphics.SetRenderTarget(screenRT);
        GL.PushMatrix();
        GL.LoadOrtho();
        drawMaterial.SetTexture("_MainTex", drawTexture);
        drawMaterial.SetMatrix("_Matrix", mat);
        drawMaterial.SetColor("_Color", color);
        drawMaterial.SetPass(0);
        GL.Begin(GL.QUADS);
        GL.TexCoord2(0.0f, 0.0f);
        GL.Vertex3(0, 0, 0.1f);
        GL.TexCoord2(1.0f, 0.0f);
        GL.Vertex3(1, 0, 0.1f);
        GL.TexCoord2(1.0f, 1.0f);
        GL.Vertex3(1, 1, 0.1f);
        GL.TexCoord2(0.0f, 1.0f);
        GL.Vertex3(0, 1, 0.1f);
        GL.End();
        GL.PopMatrix();
    }

    void Clean(Vector3 viewPos)
    {
        Vector3 cleanScale = new Vector3(0.1f, 0.1f, 1.0f);
        Matrix4x4 mat = Matrix4x4.TRS(new Vector3(viewPos.x - cleanScale.x * 0.5f, viewPos.y - cleanScale.y * 0.5f, 0.0f), Quaternion.identity, cleanScale);

        screenRT.MarkRestoreExpected();

        Graphics.SetRenderTarget(screenRT);
        GL.PushMatrix();
        GL.LoadOrtho();
        cleanMaterial.SetTexture("_MainTex", cleanTexture);
        cleanMaterial.SetMatrix("_Matrix", mat);
        cleanMaterial.SetPass(0);
        GL.Begin(GL.QUADS);
        GL.TexCoord2(0.0f, 0.0f);
        GL.Vertex3(0, 0, 0.1f);
        GL.TexCoord2(1.0f, 0.0f);
        GL.Vertex3(1, 0, 0.1f);
        GL.TexCoord2(1.0f, 1.0f);
        GL.Vertex3(1, 1, 0.1f);
        GL.TexCoord2(0.0f, 1.0f);
        GL.Vertex3(0, 1, 0.1f);
        GL.End();
        GL.PopMatrix();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        compositeMaterial.SetTexture("_ScreenTex", screenRT);
        Graphics.Blit(source, destination, compositeMaterial);
    }

    void OnDestroy()
    {
        screenRT.Release();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        clear = true;
    }

    public void SetColor(Color c)
    {
        color = c;
    }
}
