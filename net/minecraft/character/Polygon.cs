
using Godot;

public class Polygon {
    public Vertex[] vertices;
    public int vertexCount;

    public Polygon(Vertex[] vertices) {
        this.vertexCount = 0;
        this.vertices = vertices;
        this.vertexCount = vertices.Length;
    }

    public Polygon(Vertex[] vertices, int u0, int v0, int u1, int v1): this(vertices) {
        vertices[0] = vertices[0].remap((float)u1, (float)v0);
        vertices[1] = vertices[1].remap((float)u0, (float)v0);
        vertices[2] = vertices[2].remap((float)u0, (float)v1);
        vertices[3] = vertices[3].remap((float)u1, (float)v1);
    }

    public void render(SurfaceTool st) {
        st.SetColor(new Color(1.0F, 1.0F, 1.0F));
        for(int i = 3; i >= 0; --i) {
            Vertex v = this.vertices[i];
            st.SetUV(new Vector2(v.u / 63.999F, v.v / 31.999F));
            st.SetNormal(new Vector3(v.pos.X, v.pos.Y, v.pos.Z));
            st.AddVertex(new Vector3(v.pos.X, v.pos.Y, v.pos.Z));
        }
    }
}
