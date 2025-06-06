using Godot;

public class Vertex {
    public Vector3 pos;
    public float u;
    public float v;

    public Vertex(float x, float y, float z, float u, float v) : this(new Vector3(x, y, z), u, v) {}

    public Vertex remap(float u, float v) {
        return new Vertex(this, u, v);
    }

    public Vertex(Vertex vertex, float u, float v) {
        this.pos = vertex.pos;
        this.u = u;
        this.v = v;
    }

    public Vertex(Vector3 pos, float u, float v) {
        this.pos = pos;
        this.u = u;
        this.v = v;
    }
}
