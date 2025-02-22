using System;

public class GrassTile : Tile
{
    public GrassTile(int id) : base(id) {
        this.tex = 3;
    }

    protected override int getTexture(int face) {
        if (face == 1) {
            return 0;
        } else {
            return face == 0 ? 2 : 3;
        }
    }

    public void tick(Level level, int x, int y, int z, Random random) {
        if (!level.isLit(x, y, z)) {
            level.setTile(x, y, z, Tile.dirt.id);
        } else {
            for(int i = 0; i < 4; ++i) {
                int xt = (int)(x + random.NextInt64(3) - 1);
                int yt = (int)(y + random.NextInt64(5) - 3);
                int zt = (int)(z + random.NextInt64(3) - 1);
                if (level.getTile(xt, yt, zt) == Tile.dirt.id && level.isLit(xt, yt, zt)) {
                    level.setTile(xt, yt, zt, Tile.grass.id);
                }
            }
        }

    }
}