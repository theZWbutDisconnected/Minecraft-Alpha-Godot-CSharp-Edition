using System;

public class Frustum
{
    internal static Frustum getFrustum()
    {
        return new Frustum();
    }

    internal bool isVisible(AABB aabb)
    {
        return true;
    }
}