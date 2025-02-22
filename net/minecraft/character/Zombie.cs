using System;
using System.Collections.Generic;
using Godot;

public partial class Zombie : Entity
{
    private Level level;
    private Textures textures;
    private float v1;
    private float v2;
    private float v3;

    public Zombie(Level level, Textures textures, float v1, float v2, float v3) : base(level)
    {
        this.level = level;
        this.textures = textures;
        this.v1 = v1;
        this.v2 = v2;
        this.v3 = v3;
    }
}