using System;
using Godot;

public partial class Particle : Entity
{
    private float xd;
    private float yd;
    private float zd;
    public int tex;
    private float uo;
    private float vo;
    private int age = 0;
    private int lifetime = 0;
    private float size;
    public Particle(Level level, float x, float y, float z, float xa, float ya, float za, int tex) : base(level)
    {
        this.tex = tex;
        this.setSize(0.2F, 0.2F);
        this.heightOffset = this.bbHeight / 2.0F;
        this.setPos(x, y, z);
        this.xd = xa + (float)(new Random().NextDouble() * (double)2.0F - (double)1.0F) * 0.4F;
        this.yd = ya + (float)(new Random().NextDouble() * (double)2.0F - (double)1.0F) * 0.4F;
        this.zd = za + (float)(new Random().NextDouble() * (double)2.0F - (double)1.0F) * 0.4F;
        float speed = (float)(new Random().NextDouble() + new Random().NextDouble() + (double)1.0F) * 0.15F;
        float dd = (float)Math.Sqrt((double)(this.xd * this.xd + this.yd * this.yd + this.zd * this.zd));
        this.xd = this.xd / dd * speed * 0.4F;
        this.yd = this.yd / dd * speed * 0.4F + 0.1F;
        this.zd = this.zd / dd * speed * 0.4F;
        this.uo = (float)new Random().NextDouble() * 3.0F;
        this.vo = (float)new Random().NextDouble() * 3.0F;
        this.size = (float)(new Random().NextDouble() * (double)0.5F + (double)0.5F);
        this.lifetime = (int)((double)4.0F / (new Random().NextDouble() * 0.9 + 0.1));
        this.age = 0;
    }

    public override void tick() {
        this.xo = this.x;
        this.yo = this.y;
        this.zo = this.z;
        if (this.age++ >= this.lifetime) {
            this.remove();
        }

        this.yd = (float)((double)this.yd - 0.04);
        this.move(this.xd, this.yd, this.zd);
        this.xd *= 0.98F;
        this.yd *= 0.98F;
        this.zd *= 0.98F;
        if (this.onGround) {
            this.xd *= 0.7F;
            this.zd *= 0.7F;
        }

    }

    public void render(Tesselator t, float a, float xa, float ya, float za, float xa2, float za2) {
        float u0 = ((float)(this.tex % 16) + this.uo / 4.0F) / 16.0F;
        float u1 = u0 + 0.015609375F;
        float v0 = ((float)(this.tex / 16) + this.vo / 4.0F) / 16.0F;
        float v1 = v0 + 0.015609375F;
        float x = this.xo + (this.x - this.xo) * a;
        float y = this.yo + (this.y - this.yo) * a;
        float z = this.zo + (this.z - this.zo) * a;
        Vector3 particlePos = new Vector3(x, y, z);
        Transform3D cameraTransform = Minecraft.instance.player.camera.Transform;
        Basis inverseCameraBasis = cameraTransform.Basis;
        Vector3 localRight = inverseCameraBasis * Vector3.Right;
        Vector3 localUp = inverseCameraBasis * Vector3.Up;
        
        float r = 0.1F * this.size;
        Vector3 rightScaled = localRight * r;
        Vector3 upScaled = localUp * r;

        Vector3 A = particlePos - rightScaled - upScaled;
        Vector3 B = particlePos - rightScaled + upScaled;
        Vector3 C = particlePos + rightScaled + upScaled;
        Vector3 D = particlePos + rightScaled - upScaled;

        t.vertexUV(A.X, A.Y, A.Z, u0, v1);
        t.vertexUV(B.X, B.Y, B.Z, u0, v0);
        t.vertexUV(C.X, C.Y, C.Z, u1, v0);
        t.vertexUV(C.X, C.Y, C.Z, u1, v0);
        t.vertexUV(D.X, D.Y, D.Z, u1, v1);
        t.vertexUV(A.X, A.Y, A.Z, u0, v1);
    }
}