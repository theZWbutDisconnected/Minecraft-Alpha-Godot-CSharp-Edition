
public class Client : IConnectionListener
{
    public readonly SocketConnection ServerConnection;
    private readonly MinecraftServer _server;

    public Client(MinecraftServer server, SocketConnection connection)
    {
        _server = server;
        ServerConnection = connection;
        connection.SetConnectionListener(this);
    }

    public void Command(byte cmd, int remaining, byte[] buffer)
    {
        // 根据实际业务逻辑实现命令处理
    }

    public void HandleException(System.Exception exception)
    {
        Disconnect();
    }

    public void Disconnect()
    {
        _server.Disconnect(this);
    }
}