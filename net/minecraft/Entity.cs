using Godot;
using System;
using System.Collections.Generic;

public partial class Entity : Node3D
{
    protected Level level;
    public float xo;
    public float yo;
    public float zo;
    public float x;
    public float y;
    public float z;
    public float xd;
    public float yd;
    public float zd;
    public float yRot;
    public float xRot;
    public AABB bb;
    public bool onGround = false;
    public bool removed = false;
    public float heightOffset = 0.0F;
    protected float bbWidth = 0.6F;
    protected float bbHeight = 1.8F;
    public Guid entityUUID = Guid.NewGuid();

    public Entity(Level level) {
        this.level = level;
        this.resetPos();
        this.Name = "Entity" + this.entityUUID.ToString();
        this.level.AddChild(this);
    }

    public void resetPos() {
        float x = (float)new Random().NextDouble() * (float)this.level.width;
        float y = (float)(this.level.depth + 10);
        float z = (float)new Random().NextDouble() * (float)this.level.height;
        this.setPos(x, y, z);
    }

    public void remove() {
        this.removed = true;
    }

    protected void setSize(float w, float h) {
        this.bbWidth = w;
        this.bbHeight = h;
    }

    protected void setPos(float x, float y, float z) {
        this.x = x;
        this.y = y;
        this.z = z;
        float w = this.bbWidth / 2.0F;
        float h = this.bbHeight / 2.0F;
        this.bb = new AABB(x - w, y - h, z - w, x + w, y + h, z + w);
    }

    public void turn(float xo, float yo) {
        this.yRot = (float)((double)this.yRot + (double)xo * 0.15);
        this.xRot = (float)((double)this.xRot - (double)yo * 0.15);
        if (this.xRot < -90.0F) {
            this.xRot = -90.0F;
        }

        if (this.xRot > 90.0F) {
            this.xRot = 90.0F;
        }
    }

    public virtual void tick() {
        this.xo = this.x;
        this.yo = this.y;
        this.zo = this.z;
    }

    public void move(float xa, float ya, float za) {
        float xaOrg = xa;
        float yaOrg = ya;
        float zaOrg = za;
        List<AABB> aABBs = this.level.getCubes(this.bb.expand(xa, ya, za));

        for(int i = 0; i < aABBs.Count; ++i) {
            ya = ((AABB)aABBs[i]).clipYCollide(this.bb, ya);
        }

        this.bb.move(0.0F, ya, 0.0F);

        for(int i = 0; i < aABBs.Count; ++i) {
            xa = ((AABB)aABBs[i]).clipXCollide(this.bb, xa);
        }

        this.bb.move(xa, 0.0F, 0.0F);

        for(int i = 0; i < aABBs.Count; ++i) {
            za = ((AABB)aABBs[i]).clipZCollide(this.bb, za);
        }

        this.bb.move(0.0F, 0.0F, za);
        this.onGround = yaOrg != ya && yaOrg < 0.0F;
        if (xaOrg != xa) {
            this.xd = 0.0F;
        }

        if (yaOrg != ya) {
            this.yd = 0.0F;
        }

        if (zaOrg != za) {
            this.zd = 0.0F;
        }

        this.x = (this.bb.x0 + this.bb.x1) / 2.0F;
        this.y = this.bb.y0 + this.heightOffset;
        this.z = (this.bb.z0 + this.bb.z1) / 2.0F;
    }

    public void moveRelative(float xa, float za, float speed) {
        float dist = xa * xa + za * za;
        if (!(dist < 0.01F)) {
            dist = speed / (float)Math.Sqrt((double)dist);
            xa *= dist;
            za *= dist;
            float sin = (float)Math.Sin((double)this.yRot * Math.PI / (double)180.0F);
            float cos = (float)Math.Cos((double)this.yRot * Math.PI / (double)180.0F);
            this.xd += xa * cos + za * sin;
            this.zd += za * cos - xa * sin;
        }
    }

    public bool isLit() {
        int xTile = (int)this.x;
        int yTile = (int)this.y;
        int zTile = (int)this.z;
        return this.level.isLit(xTile, yTile, zTile);
    }

    public virtual void render(float a) {
    }

    Vector3[] verts = new Vector3[8];
    public void debugRender(float a)
    {
        var debugMesh = GetNodeOrNull<MeshInstance3D>("DebugAABBMesh");
        if (debugMesh == null)
        {
            debugMesh = new MeshInstance3D();
            debugMesh.Name = "DebugAABBMesh";
            debugMesh.TopLevel = true;
            AddChild(debugMesh);
        }

        var immediateMesh = new ImmediateMesh();
        var material = new StandardMaterial3D 
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            AlbedoColor = new Color(1, 0, 0, 1),
            NoDepthTest = false
        };

        immediateMesh.SurfaceBegin(Mesh.PrimitiveType.Lines);
        
        verts[0] = new Vector3(bb.x0 + (this.x - this.xo) * a, bb.y0 + (this.y - this.yo) * a, bb.z0 + (this.z - this.zo) * a);
        verts[1] = new Vector3(bb.x1 + (this.x - this.xo) * a, bb.y0 + (this.y - this.yo) * a, bb.z0 + (this.z - this.zo) * a);
        verts[2] = new Vector3(bb.x1 + (this.x - this.xo) * a, bb.y0 + (this.y - this.yo) * a, bb.z1 + (this.z - this.zo) * a);
        verts[3] = new Vector3(bb.x0 + (this.x - this.xo) * a, bb.y0 + (this.y - this.yo) * a, bb.z1 + (this.z - this.zo) * a);
        verts[4] = new Vector3(bb.x0 + (this.x - this.xo) * a, bb.y1 + (this.y - this.yo) * a, bb.z0 + (this.z - this.zo) * a);
        verts[5] = new Vector3(bb.x1 + (this.x - this.xo) * a, bb.y1 + (this.y - this.yo) * a, bb.z0 + (this.z - this.zo) * a);
        verts[6] = new Vector3(bb.x1 + (this.x - this.xo) * a, bb.y1 + (this.y - this.yo) * a, bb.z1 + (this.z - this.zo) * a);
        verts[7] = new Vector3(bb.x0 + (this.x - this.xo) * a, bb.y1 + (this.y - this.yo) * a, bb.z1 + (this.z - this.zo) * a);

        drawLineStrip(immediateMesh, new int[] {0,1,2,3,0});
        drawLineStrip(immediateMesh, new int[] {4,5,6,7,4});
        for(int i=0; i<4; i++) 
        {
            drawLine(immediateMesh, verts[i], verts[i+4]);
        }

        immediateMesh.SurfaceEnd();

        debugMesh.Mesh = immediateMesh;
        debugMesh.MaterialOverride = material;
        
        debugMesh.GlobalPosition = Vector3.Zero;
    }

    private void drawLine(ImmediateMesh mesh, Vector3 a, Vector3 b)
    {
        mesh.SurfaceAddVertex(a);
        mesh.SurfaceAddVertex(b);
    }

    private void drawLineStrip(ImmediateMesh mesh, int[] indices)
    {
        for(int i=0; i<indices.Length-1; i++)
        {
            drawLine(mesh, verts[indices[i]], verts[indices[i+1]]);
        }
    }
}