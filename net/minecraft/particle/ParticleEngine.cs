using System;
using System.Collections.Generic;
using Godot;

public class ParticleEngine
{
    protected Level level;
    private List<Particle> particles = new List<Particle>();
    private Textures textures;

    public ParticleEngine(Level level, Textures textures)
    {
        this.level = level;
        this.textures = textures;
    }

    public void add(Particle particle)
    {
        this.particles.Add(particle);
    }
    
    MeshInstance3D[] particleTemp = new MeshInstance3D[1];

    public void render(Player player, float a, int layer) {
        if (this.particleTemp[0] != null) {
            this.particleTemp[0].QueueFree();
            this.particleTemp[0] = null;
        }
        if (this.particles.Count != 0) {
            int id = this.textures.loadTexture("/terrain.png", 9728);
            float xa = -(float)Math.Cos((double)player.yRot * Math.PI / (double)180.0F);
            float za = -(float)Math.Sin((double)player.yRot * Math.PI / (double)180.0F);
            float xa2 = -za * (float)Math.Sin((double)player.xRot * Math.PI / (double)180.0F);
            float za2 = xa * (float)Math.Sin((double)player.xRot * Math.PI / (double)180.0F);
            float ya = (float)Math.Cos((double)player.xRot * Math.PI / (double)180.0F);
            Tesselator t = Tesselator.instance;
            t.init();

            for(int i = 0; i < this.particles.Count; ++i) {
                Particle p = (Particle)this.particles[i];
                p.render(t, a, xa, ya, za, xa2, za2);
            }

            this.particleTemp[0] = t.flush();
            StandardMaterial3D mat = this.particleTemp[0].MaterialOverride as StandardMaterial3D;
            mat.AlbedoColor = new Color(0.8F, 0.8F, 0.8F, 1.0F);
        }
    }

    public void tick()
    {
        for(int i = 0; i < this.particles.Count; ++i) {
            Particle p = (Particle)this.particles[i];
            p.tick();
            if (p.removed) {
                this.particles.Remove(this.particles[i--]);
            }
        }
    }
}