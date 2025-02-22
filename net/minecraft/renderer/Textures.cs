using System;
using Godot;

public class Textures
{
    public static Texture2D texture;
    public int loadTexture(string v1, int v2)
    {
        texture = GD.Load<Texture2D>("res://assets" + v1);
        return v2;
    }
}