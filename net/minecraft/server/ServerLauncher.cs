using Godot;

public partial class ServerLauncher : Node
{
    public override void _Ready()
    {
        var server = new MinecraftServer(new byte[] {127, 0, 0, 1}, 20801);
        server.Start();
        GD.Print("Minecraft server started on 127.0.0.1:20801");
    }
}