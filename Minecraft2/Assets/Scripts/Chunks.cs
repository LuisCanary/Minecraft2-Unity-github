﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunks : MonoBehaviour
{
    public MeshRenderer meshRenderer;

    public MeshFilter meshFilter;

    int vertexIndex = 0;

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    World world;

    private void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();
        PopulateVoxelMap();
        CreateMeshData();
        CreateMesh();
    }


    void PopulateVoxelMap()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    if (y < 1)
                    {
                        voxelMap[x, y, z] = 0;
                    }
                    else if (y == VoxelData.ChunkHeight-1)
                    {
                        voxelMap[x, y, z] = 2;
                    }
                    else
                    {
                        voxelMap[x, y, z] = 1;
                    }

                }
            }
        }

    }

    void CreateMeshData()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    AddVoxelDataToChunk(new Vector3(x, y, z));

                }
            }
        }
    }

    bool CheckVoxel(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (x < 0 || x > VoxelData.ChunkWidth-1 || y < 0 || y >VoxelData.ChunkHeight-1 || z < 0 || z > VoxelData.ChunkWidth-1)
        {
            return false;
        }
        else
        {
            return world.blockTypes[voxelMap[x, y, z]].isSolid;
        }

    }

    void AddVoxelDataToChunk(Vector3 pos)
    {
        for (int p = 0; p < 6; p++)
        {
            if (!CheckVoxel(pos + VoxelData.faceChecks[p]))
            {
                byte blockID = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];

                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]]);
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);

                AddTexture(world.blockTypes[blockID].GetTextureID(p));

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex+1);
                triangles.Add(vertexIndex+2);
                triangles.Add(vertexIndex+2);
                triangles.Add(vertexIndex+1);
                triangles.Add(vertexIndex+3);

                vertexIndex += 4;

            }
        }
    }

    void CreateMesh()
    {

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    void AddTexture(int textureID)
    {
        float y = textureID / VoxelData.TextureAtLastSizeInBlock;
        float x = textureID - (y * VoxelData.TextureAtLastSizeInBlock);

        x *= VoxelData.NormalizedTextureSize;
        y *= VoxelData.NormalizedTextureSize;

        y = 1f - y - VoxelData.NormalizedTextureSize;

        uvs.Add(new Vector2(x,y));
        uvs.Add(new Vector2(x,y+VoxelData.NormalizedTextureSize));
        uvs.Add(new Vector2(x + VoxelData.NormalizedTextureSize, y));
        uvs.Add(new Vector2(x + VoxelData.NormalizedTextureSize, y + VoxelData.NormalizedTextureSize));
    }

}