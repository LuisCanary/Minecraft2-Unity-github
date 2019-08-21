using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise 
{
    
    public static float Get2DPerlin(Vector2 position,float offset, float scale)
    {
        return Mathf.PerlinNoise((position.x/VoxelData.ChunkWidth*scale+offset),(position.y) / VoxelData.ChunkWidth * scale + offset);
    }

    public static bool Get3DPerlin(Vector3 position, float offset, float scale, float thershold)
    {
        // https://www.youtube.com/watch?v=Aga0TBJkchM For the noise

        float x = (position.x + offset) * scale;
        float y = (position.y + offset) * scale;
        float z = (position.z + offset) * scale;

        float AB = Mathf.PerlinNoise(x, y);
        float BC = Mathf.PerlinNoise(y, z);
        float AC = Mathf.PerlinNoise(x, z);
        float BA = Mathf.PerlinNoise(y, x);
        float CB = Mathf.PerlinNoise(z, y);
        float CA = Mathf.PerlinNoise(z, x);

        if ((AB+BC+AC+BA+CB+CA)/ 6f>thershold)
        {
            return true;
        }
        else
        {
            return false;
        }
       
    }


}
