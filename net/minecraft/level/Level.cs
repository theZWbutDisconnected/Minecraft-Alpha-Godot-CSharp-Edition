using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

public partial class Level : Node
{
    private static readonly int TILE_UPDATE_INTERVAL = 400;
    public readonly int width;
    public readonly int height;
    public readonly int depth;
    private byte[] blocks;
    private int[] lightDepths;
    private List<ILevelListener> levelListeners = new List<ILevelListener>();
    private Random random = new Random();
    int unprocessed = 0;
    public Level(int w, int h, int d) {
        this.width = w;
        this.height = h;
        this.depth = d;
        this.blocks = new byte[w * h * d];
        this.lightDepths = new int[w * h];
        bool mapLoaded = this.load();
        if (!mapLoaded) {
            this.blocks = new LevelGen(w, h, d).generateMap();
        }

        this.calcLightDepths(0, 0, w, h);
        this.Name = "Level";
    }

    public bool load()
    {
        String path = Path.Combine(ProjectSettings.GlobalizePath("level.dat"));
        
        try
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            using (GZipStream gz = new GZipStream(fs, CompressionMode.Decompress))
            {
                int totalRead = 0;
                while (totalRead < this.blocks.Length)
                {
                    int bytesRead = gz.Read(this.blocks, totalRead, this.blocks.Length - totalRead);
                    if (bytesRead == 0) break;
                    totalRead += bytesRead;
                }
            }

            this.calcLightDepths(0, 0, width, height);
            for(int i = 0; i < this.levelListeners.Count; ++i) {
                ((ILevelListener)this.levelListeners[i]).flushAll();
            }
            return true;
        }
        catch (Exception e)
        {
            GD.PrintErr($"Load failed: {e}");
            return false;
        }
    }

    public void save()
    {
        String path = Path.Combine(ProjectSettings.GlobalizePath("level.dat"));
        
        try
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            using (GZipStream gz = new GZipStream(fs, CompressionMode.Compress))
            {
                gz.Write(blocks, 0, blocks.Length);
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"Save failed: {e}");
        }
    }
    public void calcLightDepths(int x0, int y0, int x1, int y1) {
        for(int x = x0; x < x0 + x1; ++x) {
            for(int z = y0; z < y0 + y1; ++z) {
                int oldDepth = this.lightDepths[x + z * this.width];

                int y;
                for(y = this.depth - 1; y > 0 && !this.isLightBlocker(x, y, z); --y) {
                }

                this.lightDepths[x + z * this.width] = y;
                if (oldDepth != y) {
                    int yl0 = oldDepth < y ? oldDepth : y;
                    int yl1 = oldDepth > y ? oldDepth : y;

                    for(int i = 0; i < this.levelListeners.Count; ++i) {
                        ((ILevelListener)this.levelListeners[i]).flushLight(x, z, yl0, yl1);
                    }
                }
            }
        }

    }

    public void addListener(ILevelListener levelListener) {
        this.levelListeners.Add(levelListener);
    }

    public void removeListener(ILevelListener levelListener) {
        this.levelListeners.Remove(levelListener);
    }

    private bool isLightBlocker(int x, int y, int z)
    {
        Tile tile = Tile.tiles[this.getTile(x, y, z)];
        return tile == null ? false : tile.blocksLight();
    }

    public List<AABB> getCubes(AABB aABB) {
        List<AABB> aABBs = new List<AABB>();
        int x0 = (int)aABB.x0;
        int x1 = (int)(aABB.x1 + 1.0F);
        int y0 = (int)aABB.y0;
        int y1 = (int)(aABB.y1 + 1.0F);
        int z0 = (int)aABB.z0;
        int z1 = (int)(aABB.z1 + 1.0F);
        if (x0 < 0) {
            x0 = 0;
        }

        if (y0 < 0) {
            y0 = 0;
        }

        if (z0 < 0) {
            z0 = 0;
        }

        if (x1 > this.width) {
            x1 = this.width;
        }

        if (y1 > this.depth) {
            y1 = this.depth;
        }

        if (z1 > this.height) {
            z1 = this.height;
        }

        for(int x = x0; x < x1; ++x) {
            for(int y = y0; y < y1; ++y) {
                for(int z = z0; z < z1; ++z) {
                    Tile tile = Tile.tiles[this.getTile(x, y, z)];
                    if (tile != null) {
                        AABB aabb = tile.getAABB(x, y, z);
                        if (aabb != null) {
                            aABBs.Add(aabb);
                        }
                    }
                }
            }
        }

        return aABBs;
    }

    public void tick() {
        this.unprocessed += this.width * this.height * this.depth;
        int ticks = this.unprocessed / 400;
        this.unprocessed -= ticks * 400;

        for(int i = 0; i < ticks; ++i) {
            int x = (int)this.random.NextInt64(this.width);
            int y = (int)this.random.NextInt64(this.depth);
            int z = (int)this.random.NextInt64(this.height);
            Tile tile = Tile.tiles[this.getTile(x, y, z)];
            if (tile != null) {
                tile.tick(this, x, y, z, this.random);
            }
        }

    }

    public bool isLit(int x, int y, int z)
    {
        if (x >= 0 && y >= 0 && z >= 0 && x < this.width && y < this.depth && z < this.height) {
            return y >= this.lightDepths[x + z * this.width];
        } else {
            return true;
        }
    }

    public bool setTile(int x, int y, int z, int type) {
        if (x >= 0 && y >= 0 && z >= 0 && x < this.width && y < this.depth && z < this.height) {
            if (type == this.blocks[(y * this.height + z) * this.width + x]) {
                return false;
            } else {
                this.blocks[(y * this.height + z) * this.width + x] = (byte)type;
                this.calcLightDepths(x, z, 1, 1);

                for(int i = 0; i < this.levelListeners.Count; ++i) {
                    ((ILevelListener)this.levelListeners[i]).flushAround(x, y, z);
                }

                return true;
            }
        } else {
            return false;
        }
    }

    public int getTile(int x, int y, int z) {
        return x >= 0 && y >= 0 && z >= 0 && x < this.width && y < this.depth && z < this.height ? this.blocks[(y * this.height + z) * this.width + x] : 0;
    }

    public bool isSolidTile(int x, int y, int z) {
        Tile tile = Tile.tiles[this.getTile(x, y, z)];
        return tile == null ? false : tile.isSolid();
    }
}