using Godot;
using System;

public class Frustum
{
    private float[][] m_Frustum = new float[6][];
    private float[] clip = new float[16];

    private Frustum()
    {
        for(int i=0; i<6; i++)
            m_Frustum[i] = new float[4];
    }

    public static Frustum getFrustum()
    {
        var frustum = new Frustum();
        frustum.calculateFrustum();
        return frustum;
    }

    private void calculateFrustum()
    {
        Camera3D camera = Minecraft.instance.player.camera;
        if (camera == null) return;

        Projection projection = camera.GetCameraProjection();
        Transform3D viewTransform = camera.GlobalTransform.AffineInverse();
        Projection viewMatrix = new Projection(viewTransform);
        Projection viewProj = projection * viewMatrix;

        for (int col = 0; col < 4; col++)
        {
            for (int row = 0; row < 4; row++)
            {
                clip[col * 4 + row] = viewProj[col][row];
            }
        }

        m_Frustum[0][0] = clip[3] + clip[0];
        m_Frustum[0][1] = clip[7] + clip[4];
        m_Frustum[0][2] = clip[11] + clip[8];
        m_Frustum[0][3] = clip[15] + clip[12];
        normalizePlane(0);

        m_Frustum[1][0] = clip[3] - clip[0];
        m_Frustum[1][1] = clip[7] - clip[4];
        m_Frustum[1][2] = clip[11] - clip[8];
        m_Frustum[1][3] = clip[15] - clip[12];
        normalizePlane(1);

        m_Frustum[2][0] = clip[3] + clip[1];
        m_Frustum[2][1] = clip[7] + clip[5];
        m_Frustum[2][2] = clip[11] + clip[9];
        m_Frustum[2][3] = clip[15] + clip[13];
        normalizePlane(2);

        m_Frustum[3][0] = clip[3] - clip[1];
        m_Frustum[3][1] = clip[7] - clip[5];
        m_Frustum[3][2] = clip[11] - clip[9];
        m_Frustum[3][3] = clip[15] - clip[13];
        normalizePlane(3);

        m_Frustum[4][0] = clip[3] + clip[2];
        m_Frustum[4][1] = clip[7] + clip[6];
        m_Frustum[4][2] = clip[11] + clip[10];
        m_Frustum[4][3] = clip[15] + clip[14];
        normalizePlane(4);

        m_Frustum[5][0] = clip[3] - clip[2];
        m_Frustum[5][1] = clip[7] - clip[6];
        m_Frustum[5][2] = clip[11] - clip[10];
        m_Frustum[5][3] = clip[15] - clip[14];
        normalizePlane(5);
    }

    private void normalizePlane(int side)
    {
        float magnitude = Mathf.Sqrt(
            m_Frustum[side][0] * m_Frustum[side][0] + 
            m_Frustum[side][1] * m_Frustum[side][1] + 
            m_Frustum[side][2] * m_Frustum[side][2]);
        
        m_Frustum[side][0] /= magnitude;
        m_Frustum[side][1] /= magnitude;
        m_Frustum[side][2] /= magnitude;
        m_Frustum[side][3] /= magnitude;
    }

    public bool isVisible(AABB aabb)
    {
        return cubeInFrustum(aabb.x0, aabb.y0, aabb.z0, aabb.x1, aabb.y1, aabb.z1);
    }

    private bool cubeInFrustum(float x1, float y1, float z1, float x2, float y2, float z2)
    {
        for(int i=0; i<6; i++)
        {
            float px = m_Frustum[i][0] > 0 ? x2 : x1;
            float py = m_Frustum[i][1] > 0 ? y2 : y1;
            float pz = m_Frustum[i][2] > 0 ? z2 : z1;
            
            float nx = m_Frustum[i][0] > 0 ? x1 : x2;
            float ny = m_Frustum[i][1] > 0 ? y1 : y2;
            float nz = m_Frustum[i][2] > 0 ? z1 : z2;

            if((m_Frustum[i][0] * px + m_Frustum[i][1] * py + m_Frustum[i][2] * pz + m_Frustum[i][3]) <= 0)
                return false;

            if((m_Frustum[i][0] * nx + m_Frustum[i][1] * ny + m_Frustum[i][2] * nz + m_Frustum[i][3]) > 0)
                continue;
        }
        return true;
    }
}