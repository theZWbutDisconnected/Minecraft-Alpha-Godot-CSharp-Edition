using Godot;
using System;
using System.Collections.Generic;
using System.Threading;

public partial class Minecraft : Node3D
{
    public static readonly String VERSION_STRING = "0.0.11a";
    public static Minecraft instance;
    private bool fullscreen = false;
    public int width;
    public int height;
    private Timer timer = new Timer(20.0F);
    public Level level;
    public LevelRenderer levelRenderer;
    public Player player;
    public int paintTexture = 1;
    private ParticleEngine particleEngine;
    private List<Entity> entities = new List<Entity>();
    public bool appletMode = false;
    public volatile bool pause = false;
    private int yMouseAxis = 1;
    public Textures textures;
    private int editMode = 0;
    private volatile bool running = false;
    public String fpsString = "";
    private bool mouseGrabbed = false;
    private HitResult hitResult = null;
    private SubViewport viewport;
    private TextureRect tileShower;
    private Label font;

    public Minecraft() {
        Minecraft.instance = this;
        Chunk._static();
        LevelRenderer._static();
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventMouseMotion @motion) {
            if (this.mouseGrabbed) {
                float xo = 0.0F;
                float yo = 0.0F;
                xo = (float)-@motion.Relative.X;
                yo = (float)@motion.Relative.Y;

                this.player.turn(xo, yo * (float)this.yMouseAxis);
            }
        }
        if (@event is InputEventMouseButton @mouse) {
            if (!this.mouseGrabbed && @mouse.Pressed) {
                this.grabMouse();
            } else {
                if (@mouse.ButtonIndex == MouseButton.Left && @mouse.Pressed) {
                    this.handleMouseClick();
                }

                if (@mouse.ButtonIndex == MouseButton.Right && @mouse.Pressed) {
                    this.editMode = (this.editMode + 1) % 2;
                }
            }
        }
        if (@event is InputEventKey @key) {
            if (@key.KeyLabel == Key.Escape) {
                this.level.save();
                this.GetTree().Quit();
            }

            if (@key.KeyLabel == Key.Escape && (this.appletMode || !this.fullscreen)) {
                this.releaseMouse();
            }

            if (@key.KeyLabel == Key.Enter) {
                this.level.save();
            }

            if (@key.KeyLabel == Key.Key1) {
                this.paintTexture = 1;
            }

            if (@key.KeyLabel == Key.Key2) {
                this.paintTexture = 3;
            }

            if (@key.KeyLabel == Key.Key3) {
                this.paintTexture = 4;
            }

            if (@key.KeyLabel == Key.Key4) {
                this.paintTexture = 5;
            }

            if (@key.KeyLabel == Key.Key6) {
                this.paintTexture = 6;
            }

            if (@key.KeyLabel == Key.Y) {
                this.yMouseAxis *= -1;
            }

            if (@key.KeyLabel == Key.G) {
                this.entities.Add(new Zombie(this.level, this.textures, this.player.x, this.player.y, this.player.z));
            }
        }
    }

    private void handleMouseClick()
    {
        if (this.editMode == 0) {
            if (this.hitResult != null) {
                Tile oldTile = Tile.tiles[this.level.getTile(this.hitResult.x, this.hitResult.y, this.hitResult.z)];
                bool changed = this.level.setTile(this.hitResult.x, this.hitResult.y, this.hitResult.z, 0);
                if (oldTile != null && changed) {
                    oldTile.destroy(this.level, this.hitResult.x, this.hitResult.y, this.hitResult.z, this.particleEngine);
                }
            }
        } else if (this.hitResult != null) {
            int x = this.hitResult.x;
            int y = this.hitResult.y;
            int z = this.hitResult.z;
            if (this.hitResult.f == 0) {
                --y;
            }

            if (this.hitResult.f == 1) {
                ++y;
            }

            if (this.hitResult.f == 2) {
                --z;
            }

            if (this.hitResult.f == 3) {
                ++z;
            }

            if (this.hitResult.f == 4) {
                --x;
            }

            if (this.hitResult.f == 5) {
                ++x;
            }

            AABB aabb = Tile.tiles[this.paintTexture].getAABB(x, y, z);
            if (aabb == null || this.isFree(aabb)) {
                this.level.setTile(x, y, z, this.paintTexture);
            }
        }
    }

    private bool isFree(AABB aabb)
    {
        if (this.player.bb.intersects(aabb)) {
            return false;
        } else {
            for(int i = 0; i < this.entities.Count; ++i) {
                if (((Entity)this.entities[i]).bb.intersects(aabb)) {
                    return false;
                }
            }

            return true;
        }
    }

    public override void _Ready() {
        base._Ready();
        this.running = true;

        try {
            this.init();
        } catch (Exception e) {
            GD.PrintErr(e.ToString(), "Failed to start Minecraft", 0);
            return;
        }
    }


    long lastTime = (long) Time.GetTicksMsec();
    int frames = 0;
    public override void _Process(double delta) {
        if (this.running) {
            if (this.pause) {
                Thread.Sleep(100);
            } else {
                this.timer.advanceTime();

                for(int i = 0; i < this.timer.ticks; ++i) {
                    this.tick();
                }

                this.render(this.timer.a);
                ++frames;

                while(Time.GetTicksMsec() >= (ulong) (lastTime + 1000L)) {
                    this.fpsString = frames + " fps, " + Chunk.updates + " chunk updates";
                    Chunk.updates = 0;
                    lastTime += 1000L;
                    frames = 0;
                }
            }
        }
    }

    private void init()
    {
        this.width = 854;
        this.height = 480;
        this.fullscreen = false;
        GD.Print("Minecraft 0.0.11a");
        
        this.textures = new Textures();
        this.level = new Level(256, 256, 64);
        this.levelRenderer = new LevelRenderer(this.level, this.textures);
        this.player = new Player(this.level);
        this.particleEngine = new ParticleEngine(this.level, this.textures);

        for(int i = 0; i < 10; ++i) {
            Zombie zombie = new Zombie(this.level, this.textures, 128.0F, 0.0F, 128.0F);
            zombie.resetPos();
            this.entities.Add(zombie);
        }

        Camera3D viewCam = new Camera3D();
        viewCam.RotationDegrees = new Vector3(0.0F, 0.0F, 0.0F);
        viewCam.Fov = 50.0F;
        this.tileShower = new TextureRect();
        this.tileShower.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        this.viewport = new SubViewport();
        this.viewport.Size = new Vector2I(this.width, this.height);
        this.viewport.TransparentBg = true;
        this.viewport.HandleInputLocally = true;
        this.viewport.AddChild(viewCam);
        this.font = new Label();
        this.font.LabelSettings = new LabelSettings() {
            Font = GD.Load<FontFile>("res://assets/font/MinecraftRegular-Bmg3.otf"),
            FontSize = 20,
            ShadowSize = 0,
            ShadowColor = new Color(0.1F, 0.1F, 0.1F, 1.0F),
            ShadowOffset = new Vector2(2.0F, 2.0F),
        };

        AddChild(this.viewport);
        AddChild(this.tileShower);
        AddChild(this.font);
        AddChild(this.level);
        AddChild(this.player);
        AddChild(Tesselator.instance);
    }

    private void grabMouse()
    {
        if (!this.mouseGrabbed) {
            this.mouseGrabbed = true;
            if (this.appletMode) {
            } else {
                Input.SetMouseMode(Input.MouseModeEnum.Captured);
            }
        }
    }

    private void releaseMouse()
    {
        if (this.mouseGrabbed) {
            this.mouseGrabbed = false;
            if (this.appletMode) {
            } else {
                Input.SetMouseMode(Input.MouseModeEnum.Confined);
            }

        }
    }
    

    private void render(float a)
    {
        if (Input.MouseMode == Input.MouseModeEnum.Confined) {
            this.releaseMouse();
        }

        this.pick(a);
        this.setupCamera(a);
        Frustum frustum = Frustum.getFrustum();
        this.levelRenderer.updateDirtyChunks(this.player);
        this.setupFog(0);
        this.particleEngine.render(this.player, a, 0);
        this.setupFog(1);
        this.levelRenderer.render(this.player, 0);

        for(int i = 0; i < this.entities.Count; ++i) {
            Entity entity = (Entity)this.entities[i];
            if (entity.isLit() && frustum.isVisible(entity.bb)) {
                ((Entity)this.entities[i]).render(a);
            }
        }

        this.particleEngine.render(this.player, a, 0);
        this.levelRenderer.renderHit(this.hitResult, this.editMode, this.paintTexture);

        this.drawGui(a);
    }


    private void pick(float a)
    {
        this.hitResult = null;
        int touchDistance = 5;
        Camera3D cam = this.player.camera;
        Vector3 from = cam.GlobalPosition;
        Vector3 dir = cam.GlobalTransform.Basis.Z * new Vector3(-1.0F, -1.0F, -1.0F);

        for (float t = 0.0f; t < touchDistance; t += 0.05f)
        {
            Vector3 to = from + dir * t;
            int x = (int)to.X;
            int y = (int)to.Y;
            int z = (int)to.Z;

            if (this.level.getTile(x, y, z) != 0)
            {
                Vector3 prev = from + dir * (t - 0.05f);
                int prevX = (int)prev.X;
                int prevY = (int)prev.Y;
                int prevZ = (int)prev.Z;
                int dx = x - prevX;
                int dy = y - prevY;
                int dz = z - prevZ;

                int face = 0;
                if (dx != 0)
                    face = dx > 0 ? 4 : 5;
                else if (dy != 0)
                    face = dy > 0 ? 0 : 1;
                else if (dz != 0)
                    face = dz > 0 ? 2 : 3;

                this.hitResult = new HitResult(1, x, y, z, face);
                break;
            }
        }
    }


    private void setupCamera(float a)
    {
        this.player.camera.Fov = 70.0F;
        this.player.camera.Near = 0.05F;
        this.player.camera.Far = 1000.0F;
        this.moveCameraToPlayer(a);
    }

    private void moveCameraToPlayer(float a)
    {
        float x = this.player.xo + (this.player.x - this.player.xo) * a;
        float y = this.player.yo + (this.player.y - this.player.yo) * a;
        float z = this.player.zo + (this.player.z - this.player.zo) * a;
        this.player.camera.GlobalPosition = new Vector3(x, y, z);
        this.player.camera.GlobalRotationDegrees = new Vector3(this.player.xRot, this.player.yRot, 0.0F);
    }

    MeshInstance3D[] hits = new MeshInstance3D[1];
    private void drawGui(float a)
    {
        if (this.hits[0] != null) {
            this.hits[0].QueueFree();
            this.hits[0] = null;
        }
        Tesselator t = Tesselator.instance;
        t.init();
        Tile.tiles[this.paintTexture].render(t, this.level, 0, -2, 0, 0);
        MeshInstance3D mesh = t.flush();
        mesh.RotationDegrees = new Vector3(15.0F, 50.0F, 15.0F);
        mesh.GlobalPosition = new Vector3(0.0F, 0.0F, -12.0F);
        this.hits[0] = mesh;
        this.tileShower.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
        this.tileShower.Texture = this.viewport.GetTexture();
        this.tileShower.Position = new Vector2(410.0F, -200.0F);
        this.font.Position = new Vector2(2, 2);
        this.font.Text = "0.0.11a" + '\n';
        this.font.Text += this.fpsString;
    }

    private void setupFog(int v)
    {
    }

    private void stop()
    {
        this.running = false;
    }


    private void destroy()
    {
        try {
            this.level.save();
        } catch (Exception var2) {
        }
    }


    public void tick() {
        this.level.tick();
        this.particleEngine.tick();

        for(int i = 0; i < this.entities.Count; ++i) {
            ((Entity)this.entities[i]).tick();
            if (((Entity)this.entities[i]).removed) {
                this.entities.Remove((Entity)this.entities[i]);
            }
        }

        this.player.tick();
    }
}