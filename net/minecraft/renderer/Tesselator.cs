using Godot;
using System;

public partial class Tesselator : Node3D
{
    private SurfaceTool surfaceTool;
    private int vertices = 0;
    private bool hasColor = false;
    private bool hasUV = false;
    private Color currentColor = Colors.White;
    private Vector2 currentUV = Vector2.Zero;
    private Vector3 currentNormal = Vector3.Zero;

    private const int MAX_VERTICES = 524288 / 3;
    
    public static Tesselator instance = new Tesselator();

    private Tesselator()
    {
        this.Name = "Tessellator";
        init();
    }

    public void init()
    {
        this.surfaceTool?.Clear();
        this.surfaceTool = new SurfaceTool();
        this.surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
        this.vertices = 0;
        this.hasColor = false;
        this.hasUV = false;
        this.currentColor = Colors.White;
        this.currentUV = Vector2.Zero;
        this.currentNormal = Vector3.Zero;
    }

    public MeshInstance3D flush(Node3D parent = null) {
        if (vertices > 0) {
            surfaceTool.Index();
            ArrayMesh mesh = surfaceTool.Commit();
            
            MeshInstance3D meshInstance = new MeshInstance3D
            {
                Mesh = mesh,
                MaterialOverride = new StandardMaterial3D() {
                    Transparency = BaseMaterial3D.TransparencyEnum.AlphaScissor,
                    DepthDrawMode = BaseMaterial3D.DepthDrawModeEnum.Always,
                    AlbedoTexture = Textures.texture,
                    TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest,
                    CullMode = BaseMaterial3D.CullModeEnum.Disabled,
                    ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
                    VertexColorUseAsAlbedo = true
                }
            };
            if (parent == null)
                parent = Tesselator.instance;
            parent.AddChild(meshInstance);
            
            init();
            return meshInstance;
        }
        return null;
    }

    public void tex(float u, float v)
    {
        if (!hasUV) checkFormatChange(true);
        hasUV = true;
        currentUV = new Vector2(u, v);
    }

    public void color(float r, float g, float b)
    {
        if (!hasColor) checkFormatChange(true);
        hasColor = true;
        currentColor = new Color(r, g, b);
    }

    public void normal(float x, float y, float z)
    {
        currentNormal = new Vector3(x, y, z);
    }

    public void vertex(float x, float y, float z)
    {
        if (hasUV) surfaceTool.SetUV(currentUV);
        if (hasColor) surfaceTool.SetColor(currentColor);
        surfaceTool.SetNormal(currentNormal);
        surfaceTool.AddVertex(new Vector3(x, y, z));
        
        if (++vertices >= MAX_VERTICES)
            flush();
    }

    public void vertexUV(float x, float y, float z, float u, float v)
    {
        tex(u, v);
        vertex(x, y, z);
    }

    public void color(int rgb)
    {
        color(
            (float)((rgb >> 16) & 0xFF) / 255f,
            (float)((rgb >> 8) & 0xFF) / 255f,
            (float)(rgb & 0xFF) / 255f
        );
    }

    public void noColor()
    {
        if (hasColor) checkFormatChange(false);
        hasColor = false;
        currentColor = Colors.White;
    }

    private void checkFormatChange(bool adding)
    {
        if ((adding && vertices > 0) || (!adding && vertices > 0))
        {
            flush();
            surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
            if (hasUV) surfaceTool.SetUV(currentUV);
            if (hasColor) surfaceTool.SetColor(currentColor);
        }
    }
}