using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class SocketServer
{
    private TcpListener listener;
    private IServerListener serverListener;
    private List<SocketConnection> connections = new List<SocketConnection>();
    private byte[] bindIP;

    public SocketServer(byte[] ip, int port, IServerListener serverlistener)
    {
        serverListener = serverlistener;
        bindIP = ip;
        listener = new TcpListener(new IPAddress(bindIP), port);
        listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        listener.Start();
        listener.Server.Blocking = false;
    }

    public void tick()
    {
        while (listener.Pending())
        {
            try
            {
                Socket client = listener.AcceptSocket();
                client.Blocking = false;
                SocketConnection conn = new SocketConnection(client);
                connections.Add(conn);
                serverListener.clientConnected(conn);
            }
            catch (Exception e)
            {
            }
        }

        for (int i = connections.Count - 1; i >= 0; i--)
        {
            if (!connections[i].isConnected())
            {
                connections[i].Dispose();
                connections.RemoveAt(i);
                continue;
            }

            try
            {
                connections[i].tick();
            }
            catch (Exception e)
            {
                serverListener.clientException(connections[i], e);
                connections[i].Dispose();
                connections.RemoveAt(i);
            }
        }
    }
}