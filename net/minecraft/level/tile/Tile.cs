using System;

public class Tile {
    public static readonly Tile[] tiles = new Tile[256];
    public static readonly Tile empty = null;
    public static readonly Tile rock = new Tile(1, 1);
    public static readonly Tile grass = new GrassTile(2);
    public static readonly Tile dirt = new DirtTile(3, 2);
    public static readonly Tile stoneBrick = new Tile(4, 16);
    public static readonly Tile wood = new Tile(5, 4);
    public static readonly Tile bush = new Bush(6);
    public int tex;
    public readonly int id;

    protected Tile(int id) {
        tiles[id] = this;
        this.id = id;
    }

    protected Tile(int id, int tex) : this(id) {
        this.tex = tex;
    }

    public virtual void render(Tesselator t, Level level, int layer, int x, int y, int z) {
        float c1;
        float c2;
        float c3;

        if (this.shouldRenderFace(level, x, y - 1, z, layer)) {
            c1 = 1.0F;
            if (level.isLit(x, y - 1, z) ^ layer == 0)
                c1 *= 0.6F;
            t.color(c1, c1, c1);
            t.normal(0, -1, 0);
            this.renderFace(t, x, y, z, 0);
        }

        if (this.shouldRenderFace(level, x, y + 1, z, layer)) {
            c1 = 1.0F;
            if (level.isLit(x, y, z) ^ layer == 0)
                c1 *= 0.6F;
            t.color(c1, c1, c1);
            t.normal(0, 1, 0);
            this.renderFace(t, x, y, z, 1);
        }

        if (this.shouldRenderFace(level, x, y, z - 1, layer)) {
            c2 = 0.8F;
            if (level.isLit(x, y, z - 1) ^ layer == 0)
                c2 *= 0.6F;
            t.color(c2, c2, c2);
            t.normal(0, 0, -1);
            this.renderFace(t, x, y, z, 2);
        }

        if (this.shouldRenderFace(level, x, y, z + 1, layer)) {
            c2 = 0.8F;
            if (level.isLit(x, y, z + 1) ^ layer == 0)
                c2 *= 0.6F;
            t.color(c2, c2, c2);
            t.normal(0, 0, 1);
            this.renderFace(t, x, y, z, 3);
        }

        if (this.shouldRenderFace(level, x - 1, y, z, layer)) {
            c3 = 0.6F;
            if (level.isLit(x - 1, y, z) ^ layer == 0)
                c3 *= 0.6F;
            t.color(c3, c3, c3);
            t.normal(-1, 0, 0);
            this.renderFace(t, x, y, z, 4);
        }

        if (this.shouldRenderFace(level, x + 1, y, z, layer)) {
            c3 = 0.6F;
            if (level.isLit(x + 1, y, z) ^ layer == 0)
                c3 *= 0.6F;
            t.color(c3, c3, c3);
            t.normal(1, 0, 0);
            this.renderFace(t, x, y, z, 5);
        }

    }

    private bool shouldRenderFace(Level level, int x, int y, int z, int layer) {
        return !level.isSolidTile(x, y, z);
    }

    protected virtual int getTexture(int face) {
        return this.tex;
    }

    public void renderFace(Tesselator t, int x, int y, int z, int face) {
        int tex = this.getTexture(face);
        float u0 = (float)(tex % 16) / 16.0F;
        float u1 = u0 + 0.0624375F;
        float v0 = (float)(tex / 16) / 16.0F;
        float v1 = v0 + 0.0624375F;
        float x0 = (float)x + 0.0F;
        float x1 = (float)x + 1.0F;
        float y0 = (float)y + 0.0F;
        float y1 = (float)y + 1.0F;
        float z0 = (float)z + 0.0F;
        float z1 = (float)z + 1.0F;
        if (face == 0) {
            t.vertexUV(x0, y0, z0, u0, v0);
            t.vertexUV(x1, y0, z0, u1, v0);
            t.vertexUV(x1, y0, z1, u1, v1);
            t.vertexUV(x1, y0, z1, u1, v1);
            t.vertexUV(x0, y0, z1, u0, v1);
            t.vertexUV(x0, y0, z0, u0, v0);
        }

        if (face == 1) {
            t.vertexUV(x0, y1, z0, u0, v0);
            t.vertexUV(x1, y1, z0, u1, v0);
            t.vertexUV(x1, y1, z1, u1, v1);
            t.vertexUV(x1, y1, z1, u1, v1);
            t.vertexUV(x0, y1, z1, u0, v1);
            t.vertexUV(x0, y1, z0, u0, v0);
        }

        if (face == 2) {
            t.vertexUV(x1, y0, z0, u0, v1);
            t.vertexUV(x0, y0, z0, u1, v1);
            t.vertexUV(x0, y1, z0, u1, v0);
            t.vertexUV(x0, y1, z0, u1, v0);
            t.vertexUV(x1, y1, z0, u0, v0);
            t.vertexUV(x1, y0, z0, u0, v1);
        }

        if (face == 3) {
            t.vertexUV(x1, y0, z1, u0, v1);
            t.vertexUV(x0, y0, z1, u1, v1);
            t.vertexUV(x0, y1, z1, u1, v0);
            t.vertexUV(x0, y1, z1, u1, v0);
            t.vertexUV(x1, y1, z1, u0, v0);
            t.vertexUV(x1, y0, z1, u0, v1);
        }

        if (face == 4) {
            t.vertexUV(x0, y0, z1, u1, v1);
            t.vertexUV(x0, y0, z0, u0, v1);
            t.vertexUV(x0, y1, z0, u0, v0);
            t.vertexUV(x0, y1, z0, u0, v0);
            t.vertexUV(x0, y1, z1, u1, v0);
            t.vertexUV(x0, y0, z1, u1, v1);
        }

        if (face == 5) {
            t.vertexUV(x1, y0, z1, u1, v1);
            t.vertexUV(x1, y0, z0, u0, v1);
            t.vertexUV(x1, y1, z0, u0, v0);
            t.vertexUV(x1, y1, z0, u0, v0);
            t.vertexUV(x1, y1, z1, u1, v0);
            t.vertexUV(x1, y0, z1, u1, v1);
        }
    }

    public void renderFaceNoTexture(Tesselator t, int x, int y, int z, int face) {
        float x0 = (float)x + 0.0F;
        float x1 = (float)x + 1.0F;
        float y0 = (float)y + 0.0F;
        float y1 = (float)y + 1.0F;
        float z0 = (float)z + 0.0F;
        float z1 = (float)z + 1.0F;
        if (face == 0) {
            t.vertex(x0, y0, z0);
            t.vertex(x1, y0, z0);
            t.vertex(x1, y0, z1);
            t.vertex(x1, y0, z1);
            t.vertex(x0, y0, z1);
            t.vertex(x0, y0, z0);
        }

        if (face == 1) {
            t.vertex(x0, y1, z0);
            t.vertex(x1, y1, z0);
            t.vertex(x1, y1, z1);
            t.vertex(x1, y1, z1);
            t.vertex(x0, y1, z1);
            t.vertex(x0, y1, z0);
        }

        if (face == 2) {
            t.vertex(x1, y0, z0);
            t.vertex(x0, y0, z0);
            t.vertex(x0, y1, z0);
            t.vertex(x0, y1, z0);
            t.vertex(x1, y1, z0);
            t.vertex(x1, y0, z0);
        }

        if (face == 3) {
            t.vertex(x1, y0, z1);
            t.vertex(x0, y0, z1);
            t.vertex(x0, y1, z1);
            t.vertex(x0, y1, z1);
            t.vertex(x1, y1, z1);
            t.vertex(x1, y0, z1);
        }

        if (face == 4) {
            t.vertex(x0, y0, z1);
            t.vertex(x0, y0, z0);
            t.vertex(x0, y1, z0);
            t.vertex(x0, y1, z0);
            t.vertex(x0, y1, z1);
            t.vertex(x0, y0, z1);
        }

        if (face == 5) {
            t.vertex(x1, y0, z1);
            t.vertex(x1, y0, z0);
            t.vertex(x1, y1, z0);
            t.vertex(x1, y1, z0);
            t.vertex(x1, y1, z1);
            t.vertex(x1, y0, z1);
        }

    }

    public virtual AABB getTileAABB(int x, int y, int z) {
        return new AABB((float)x, (float)y, (float)z, (float)(x + 1), (float)(y + 1), (float)(z + 1));
    }

    public virtual AABB getAABB(int x, int y, int z) {
        return new AABB((float)x, (float)y, (float)z, (float)(x + 1), (float)(y + 1), (float)(z + 1));
    }

    public virtual bool blocksLight() {
        return true;
    }

    public virtual bool isSolid() {
        return true;
    }

    public virtual void tick(Level level, int x, int y, int z, Random random) {
    }

    public virtual void destroy(Level level, int x, int y, int z, ParticleEngine particleEngine) {
        int SD = 4;

        for(int xx = 0; xx < SD; ++xx) {
            for(int yy = 0; yy < SD; ++yy) {
                for(int zz = 0; zz < SD; ++zz) {
                    float xp = (float)x + ((float)xx + 0.5F) / (float)SD;
                    float yp = (float)y + ((float)yy + 0.5F) / (float)SD;
                    float zp = (float)z + ((float)zz + 0.5F) / (float)SD;
                    particleEngine.add(new Particle(level, xp, yp, zp, xp - (float)x - 0.5F, yp - (float)y - 0.5F, zp - (float)z - 0.5F, this.tex));
                }
            }
        }

    }
}