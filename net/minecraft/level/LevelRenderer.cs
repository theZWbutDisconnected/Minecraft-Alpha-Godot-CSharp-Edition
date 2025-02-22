using Godot;
using System;
using System.Collections.Generic;

public class LevelRenderer : ILevelListener {
    public static readonly int MAX_REBUILDS_PER_FRAME = 8;
    public static readonly int CHUNK_SIZE = 16;
    private Level level;
    private Chunk[] chunks;
    private static Tesselator t;
    private int xChunks;
    private int yChunks;
    private int zChunks;
    private Textures textures;

    public static void _static() {
        t = Tesselator.instance;
    }

    public LevelRenderer(Level level, Textures textures) {
        this.level = level;
        this.textures = textures;
        level.addListener(this);
        this.xChunks = level.width / 16;
        this.yChunks = level.depth / 16;
        this.zChunks = level.height / 16;
        this.chunks = new Chunk[this.xChunks * this.yChunks * this.zChunks];

        for(int x = 0; x < this.xChunks; ++x) {
            for(int y = 0; y < this.yChunks; ++y) {
                for(int z = 0; z < this.zChunks; ++z) {
                    int x0 = x * 16;
                    int y0 = y * 16;
                    int z0 = z * 16;
                    int x1 = (x + 1) * 16;
                    int y1 = (y + 1) * 16;
                    int z1 = (z + 1) * 16;
                    if (x1 > level.width) {
                        x1 = level.width;
                    }

                    if (y1 > level.depth) {
                        y1 = level.depth;
                    }

                    if (z1 > level.height) {
                        z1 = level.height;
                    }

                    this.chunks[(x + y * this.xChunks) * this.zChunks + z] = new Chunk(level, x0, y0, z0, x1, y1, z1);
                }
            }
        }

    }

    public List<Chunk> getAllDirtyChunks() {
        List<Chunk> dirty = null;

        for(int i = 0; i < this.chunks.Length; ++i) {
            Chunk chunk = this.chunks[i];
            if (chunk.isDirty()) {
                if (dirty == null) {
                    dirty = new List<Chunk>();
                }

                dirty.Add(chunk);
            }
        }

        return dirty;
    }

    public void render(Player player, int layer) {
        this.textures.loadTexture("/terrain.png", 9728);
        Frustum frustum = Frustum.getFrustum();

        for(int i = 0; i < this.chunks.Length; ++i) {
            if (frustum.isVisible(this.chunks[i].aabb)) {
                this.chunks[i].render(layer);
            }
        }
    }

    public void setDirty(int x0, int y0, int z0, int x1, int y1, int z1) {
        x0 /= 16;
        x1 /= 16;
        y0 /= 16;
        y1 /= 16;
        z0 /= 16;
        z1 /= 16;
        if (x0 < 0) {
            x0 = 0;
        }

        if (y0 < 0) {
            y0 = 0;
        }

        if (z0 < 0) {
            z0 = 0;
        }

        if (x1 >= this.xChunks) {
            x1 = this.xChunks - 1;
        }

        if (y1 >= this.yChunks) {
            y1 = this.yChunks - 1;
        }

        if (z1 >= this.zChunks) {
            z1 = this.zChunks - 1;
        }

        for(int x = x0; x <= x1; ++x) {
            for(int y = y0; y <= y1; ++y) {
                for(int z = z0; z <= z1; ++z) {
                    this.chunks[(x + y * this.xChunks) * this.zChunks + z].setDirty();
                }
            }
        }

    }

    public void flushAround(int x, int y, int z) {
        this.setDirty(x - 1, y - 1, z - 1, x + 1, y + 1, z + 1);
    }

    public void flushLight(int x, int z, int y0, int y1) {
        this.setDirty(x - 1, y0 - 1, z - 1, x + 1, y1 + 1, z + 1);
    }

    public void flushAll() {
        this.setDirty(0, 0, 0, this.level.width, this.level.depth, this.level.height);
    }

    MeshInstance3D[] hits = new MeshInstance3D[1];
    bool[] free = new bool[1];

    public void renderHit(HitResult h, int editMode, int tileType)
    {
        if (this.hits[0] != null && !this.free[0]) {
            this.free[0] = true;
            this.hits[0].QueueFree();
        }
        if (h != null) {
            if (editMode == 0) {
                t.init();

                for(int i = 0; i < 6; ++i) {
                    Tile.rock.renderFaceNoTexture(t, h.x, h.y, h.z, i);
                }

                this.hits[0] = t.flush();
                StandardMaterial3D mat = (StandardMaterial3D)this.hits[0].MaterialOverride;
                mat.AlbedoColor = new Color(0.0F, 0.0F, 0.0F, ((float)Math.Sin((double)Time.GetTicksMsec() / (double)100.0F) * 0.2F + 0.4F) * 0.5F);
                this.free[0] = false;
            } else {
                float br = (float)Math.Sin((double)Time.GetTicksMsec() / (double)100.0F) * 0.2F + 0.8F;
                int x = h.x;
                int y = h.y;
                int z = h.z;
                if (h.f == 0) {
                    --y;
                }

                if (h.f == 1) {
                    ++y;
                }

                if (h.f == 2) {
                    --z;
                }

                if (h.f == 3) {
                    ++z;
                }

                if (h.f == 4) {
                    --x;
                }

                if (h.f == 5) {
                    ++x;
                }

                t.init();
                t.noColor();

                for(int i = 0; i < 6; ++i) {
                    Tile.tiles[tileType].renderFace(t, x, y, z, i);
                }

                this.hits[0] = t.flush();
                StandardMaterial3D mat = (StandardMaterial3D)this.hits[0].MaterialOverride;
                mat.AlbedoColor = new Color(br, br, br, (float)Math.Sin((double)Time.GetTicksMsec() / (double)200.0F) * 0.2F + 0.5F);
                this.free[0] = false;
            }
        }
    }

    public void updateDirtyChunks(Player player)
    {
        List<Chunk> dirty = this.getAllDirtyChunks();
        if (dirty != null) {
            dirty.Sort(new DirtyChunkSorter(player, Frustum.getFrustum()));

            for(int i = 0; i < 8 && i < dirty.Count; ++i) {
                ((Chunk)dirty[i]).rebuild();
            }

        }
    }
}