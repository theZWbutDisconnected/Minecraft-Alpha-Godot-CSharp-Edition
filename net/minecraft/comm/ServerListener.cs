
using System;

public interface IServerListener
{
    void clientConnected(SocketConnection connection);
    void clientException(SocketConnection connection, Exception exception);
}