
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

    public void render(Tesselator t) {
        if (this.vertices.Length >= 3) {
            Vertex v0 = this.vertices[0];
            Vertex v1 = this.vertices[1];
            Vertex v2 = this.vertices[2];
            t.vertexUV(v0.pos.X, v0.pos.Y, v0.pos.Z, v0.u / 63.999F, v0.v / 31.999F);
            t.vertexUV(v1.pos.X, v1.pos.Y, v1.pos.Z, v1.u / 63.999F, v1.v / 31.999F);
            t.vertexUV(v2.pos.X, v2.pos.Y, v2.pos.Z, v2.u / 63.999F, v2.v / 31.999F);

            if (this.vertices.Length >= 4) {
                Vertex v3 = this.vertices[3];
                t.vertexUV(v0.pos.X, v0.pos.Y, v0.pos.Z, v0.u / 63.999F, v0.v / 31.999F);
                t.vertexUV(v2.pos.X, v2.pos.Y, v2.pos.Z, v2.u / 63.999F, v2.v / 31.999F);
                t.vertexUV(v3.pos.X, v3.pos.Y, v3.pos.Z, v3.u / 63.999F, v3.v / 31.999F);
            }
        }
    }
}
