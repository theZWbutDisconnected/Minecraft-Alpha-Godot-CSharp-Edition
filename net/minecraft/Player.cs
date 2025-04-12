using Godot;
using System;

public partial class Player : Entity
{
    public Camera3D camera;
    public Player(Level level) : base(level) {
        this.heightOffset = 1.62F;
        this.camera = new Camera3D();
        this.camera.Fov = 70.0f;
        this.camera.Near = 0.05f;
        this.camera.Far = 1000.0f;

        FogVolume fog = new FogVolume();
        
        AddChild(this.camera);
        this.camera.AddChild(fog);
    }

    public void tick() {
        this.xo = this.x;
        this.yo = this.y;
        this.zo = this.z;
        float xa = 0.0F;
        float ya = 0.0F;
        if (Input.IsKeyPressed(Key.R)) {
            this.resetPos();
        }

        if (Input.IsKeyPressed(Key.Up) || Input.IsKeyPressed(Key.W)) {
            --ya;
        }

        if (Input.IsKeyPressed(Key.Down) || Input.IsKeyPressed(Key.S)) {
            ++ya;
        }

        if (Input.IsKeyPressed(Key.Left) || Input.IsKeyPressed(Key.A)) {
            --xa;
        }

        if (Input.IsKeyPressed(Key.Right) || Input.IsKeyPressed(Key.D)) {
            ++xa;
        }

        if ((Input.IsKeyPressed(Key.Space) || Input.IsKeyPressed(Key.Bracketright)) && this.onGround) {
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
}