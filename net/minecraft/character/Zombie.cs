using System;
using System.Collections.Generic;
using Godot;

public partial class Zombie : Entity
{
    public float rot;
    public float timeOffs;
    public float speed;
    public float rotA;
    private static ZombieModel zombieModel = new ZombieModel();
    private Textures textures;

    public Zombie(Level level, Textures textures, float v1, float v2, float v3) : base(level)
    {
        this.textures = textures;
        this.rotA = (float)(new Random().NextDouble() + (double)1.0F) * 0.01F;
        this.setPos(x, y, z);
        this.timeOffs = (float)new Random().NextDouble() * 1239813.0F;
        this.rot = (float)(new Random().NextDouble() * Math.PI * (double)2.0F);
        this.speed = 1.0F;
        this.level.AddChild(this);
    }

    public override void tick() {
        this.xo = this.x;
        this.yo = this.y;
        this.zo = this.z;
        float xa = 0.0F;
        float ya = 0.0F;
        if (this.y < -100.0F) {
            this.remove();
        }

        this.rot += this.rotA;
        this.rotA = (float)((double)this.rotA * 0.99);
        this.rotA = (float)((double)this.rotA + (new Random().NextDouble() - new Random().NextDouble()) * new Random().NextDouble() * new Random().NextDouble() * (double)0.08F);
        xa = (float)Math.Sin((double)this.rot);
        ya = (float)Math.Cos((double)this.rot);
        if (this.onGround && new Random().NextDouble() < 0.08) {
            this.yd = 0.5F;
        }

        this.moveRelative(xa, ya, this.onGround ? 0.1F : 0.02F);
        this.yd = (float)((double)this.yd - 0.08);
        this.move(this.xd, this.yd, this.zd);
        this.xd *= 0.91F;
        this.yd *= 0.98F;
        this.zd *= 0.91F;
        if (this.onGround) {
            this.xd *= 0.7F;
            this.zd *= 0.7F;
        }

    }

    public override void render(float a) {
        // int id = this.textures.loadTexture("/char.png", 9728);
        double time = (double)Time.GetTicksMsec() / 1000.0 * 10.0 * (double)this.speed + (double)this.timeOffs;
        float size = 0.058333334F;
        float yy = (float)(-Math.Abs(Math.Sin(time * 0.6662)) * (double)5.0F - (double)23.0F);
        Translate(new Vector3(this.xo + (this.x - this.xo) * a, this.yo + (this.y - this.yo) * a, this.zo + (this.z - this.zo) * a));
        SetScale(new Vector3(1.0F, -1.0F, 1.0F));
        SetScale(new Vector3(size, size, size));
        Translate(new Vector3(0.0F, yy, 0.0F));
        float c = 57.29578F;
        RotateY(this.rot * c + 180.0F);
        zombieModel.render((float)time, this);
    }
}