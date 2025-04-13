
using System;
using Godot;

public class ZombieModel {
    public Cube head = new Cube(0, 0);
    public Cube body;
    public Cube arm0;
    public Cube arm1;
    public Cube leg0;
    public Cube leg1;

    public ZombieModel() {
        this.head.addBox(-4.0F, -8.0F, -4.0F, 8, 8, 8);
        this.body = new Cube(16, 16);
        this.body.addBox(-4.0F, 0.0F, -2.0F, 8, 12, 4);
        this.arm0 = new Cube(40, 16);
        this.arm0.addBox(-3.0F, -2.0F, -2.0F, 4, 12, 4);
        this.arm0.setPos(-5.0F, 2.0F, 0.0F);
        this.arm1 = new Cube(40, 16);
        this.arm1.addBox(-1.0F, -2.0F, -2.0F, 4, 12, 4);
        this.arm1.setPos(5.0F, 2.0F, 0.0F);
        this.leg0 = new Cube(0, 16);
        this.leg0.addBox(-2.0F, 0.0F, -2.0F, 4, 12, 4);
        this.leg0.setPos(-2.0F, 12.0F, 0.0F);
        this.leg1 = new Cube(0, 16);
        this.leg1.addBox(-2.0F, 0.0F, -2.0F, 4, 12, 4);
        this.leg1.setPos(2.0F, 12.0F, 0.0F);
    }

    public void render(float time, Node3D parent) {
        this.head.yRot = (float)Math.Sin((double)time * 0.83) * 1.0F;
        this.head.xRot = (float)Math.Sin((double)time) * 0.8F;
        this.arm0.xRot = (float)Math.Sin((double)time * 0.6662 + Math.PI) * 2.0F;
        this.arm0.zRot = (float)(Math.Sin((double)time * 0.2312) + (double)1.0F) * 1.0F;
        this.arm1.xRot = (float)Math.Sin((double)time * 0.6662) * 2.0F;
        this.arm1.zRot = (float)(Math.Sin((double)time * 0.2812) - (double)1.0F) * 1.0F;
        this.leg0.xRot = (float)Math.Sin((double)time * 0.6662) * 1.4F;
        this.leg1.xRot = (float)Math.Sin((double)time * 0.6662 + Math.PI) * 1.4F;
        this.head.render(parent);
        this.body.render(parent);
        this.arm0.render(parent);
        this.arm1.render(parent);
        this.leg0.render(parent);
        this.leg1.render(parent);
    }
}
