
using System;
using System.Collections.Generic;
using System.Threading;
using Godot;

public class MinecraftServer : IServerListener
{
    private SocketServer _socketServer;
    private Dictionary<SocketConnection, Client> _clientMap = new Dictionary<SocketConnection, Client>();
    private List<Client> _clients = new List<Client>();
    private bool _running;

    public MinecraftServer(byte[] ips, int port)
    {
        try 
        {
            _socketServer = new SocketServer(ips, port, this);
        }
        catch (System.Exception e)
        {
            GD.PrintErr($"Server init failed: {e}");
        }
    }

    public void clientConnected(SocketConnection connection)
    {
        Client client = new Client(this, connection);
        _clientMap.Add(connection, client);
        _clients.Add(client);
        GD.Print($"Client connected: {connection.getIP()}");
    }

    public void disconnect(Client client)
    {
        _clientMap.Remove(client.ServerConnection);
        _clients.Remove(client);
        client.ServerConnection.Dispose();
        GD.Print($"Client disconnected: {client.ServerConnection.getIP()}");
    }

    public void clientException(SocketConnection connection, System.Exception e)
    {
        if (_clientMap.TryGetValue(connection, out Client client))
        {
            client.handleException(e);
        }
    }

    public void start()
    {
        _running = true;
        new Thread(run).Start(); 
    }

    private void run()
    {
        while (_running)
        {
            tick();
            Thread.Sleep(5); 
        }
    }

    private void tick()
    {
        try 
        {
            _socketServer.tick();
        }
        catch (System.Exception e)
        {
            GD.PrintErr($"Server tick error: {e}");
        }
    }

    public void stop()
    {
        _running = false;
        foreach (var client in _clients.ToArray())
        {
            disconnect(client);
        }
    }
}
