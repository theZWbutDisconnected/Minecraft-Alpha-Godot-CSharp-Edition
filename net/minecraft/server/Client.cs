
public class Client : IConnectionListener
{
    public readonly SocketConnection ServerConnection;
    private readonly MinecraftServer _server;

    public Client(MinecraftServer server, SocketConnection connection)
    {
        _server = server;
        ServerConnection = connection;
        connection.setConnectionListener(this);
    }

    public void command(byte cmd, int remaining, byte[] buffer)
    {
    }

    public void handleException(System.Exception exception)
    {
        disconnect();
    }

    public void disconnect()
    {
        _server.disconnect(this);
    }
}