using System;

public class Bush : Tile
{
    public Bush(int id) : base(id)
    {
        this.tex = 15;
    }

    public void tick(Level level, int x, int y, int z, Random random) {
        int below = level.getTile(x, y - 1, z);
        if (!level.isLit(x, y, z) || below != Tile.dirt.id && below != Tile.grass.id) {
            level.setTile(x, y, z, 0);
        }
    }

    public override void render(Tesselator t, Level level, int layer, int x, int y, int z) {
        if (!(level.isLit(x, y, z) ^ layer != 1)) {
            int tex = this.getTexture(15);
            float u0 = (float)(tex % 16) / 16.0F;
            float u1 = u0 + 0.0624375F;
            float v0 = (float)(tex / 16) / 16.0F;
            float v1 = v0 + 0.0624375F;
            int rots = 2;
            t.color(1.0F, 1.0F, 1.0F);

            for(int r = 0; r < rots; ++r) {
                float xa = (float)(Math.Sin((double)r * Math.PI / (double)rots + (Math.PI / 4D)) * (double)0.5F);
                float za = (float)(Math.Cos((double)r * Math.PI / (double)rots + (Math.PI / 4D)) * (double)0.5F);
                float x0 = (float)x + 0.5F - xa;
                float x1 = (float)x + 0.5F + xa;
                float y0 = (float)y + 0.0F;
                float y1 = (float)y + 1.0F;
                float z0 = (float)z + 0.5F - za;
                float z1 = (float)z + 0.5F + za;
                t.vertexUV(x0, y1, z0, u1, v0);
                t.vertexUV(x1, y1, z1, u0, v0);
                t.vertexUV(x1, y0, z1, u0, v1);
                t.vertexUV(x0, y1, z0, u1, v0);
                t.vertexUV(x1, y0, z1, u0, v1);
                t.vertexUV(x0, y0, z0, u1, v1);
                t.vertexUV(x1, y1, z1, u0, v0);
                t.vertexUV(x0, y1, z0, u1, v0);
                t.vertexUV(x0, y0, z0, u1, v1);
                t.vertexUV(x1, y1, z1, u0, v0);
                t.vertexUV(x0, y0, z0, u1, v1);
                t.vertexUV(x1, y0, z1, u0, v1);
            }

        }
    }

    public override AABB getAABB(int x, int y, int z) {
        return null;
    }

    public override bool blocksLight() {
        return false;
    }

    public override bool isSolid() {
        return false;
    }
}