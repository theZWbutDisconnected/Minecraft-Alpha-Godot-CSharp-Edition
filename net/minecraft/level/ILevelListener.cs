using Godot;
using System;

public interface ILevelListener
{
    void flushAround(int x, int y, int z);
    void flushLight(int x, int z, int y0, int y1);
    void flushAll();
}