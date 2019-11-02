﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{

    public int seed;
    public BiomeAttributes biome;

    public Transform player;
    public Vector3 spawnPosition;

    public Material material;
    public BlockType[] blockTypes;

    Chunk[,] chunks = new Chunk[VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks];

    List<ChunkCoord> activeChunks = new List<ChunkCoord>();
    public ChunkCoord playerChunkCoord;
    ChunkCoord playerLastChunkCoord;

    List<ChunkCoord> chunksToCreate = new List<ChunkCoord>();
	private bool isCreatingChunks;

	public GameObject debugScreen;

    private void Start()
    {
        Random.InitState(seed);

        spawnPosition = new Vector3((VoxelData.WorldSizeInChunks)*VoxelData.ChunkWidth / 2f, VoxelData.ChunkHeight-50f, (VoxelData.WorldSizeInChunks) * VoxelData.ChunkWidth / 2f);
        GenerateWorld();
        playerLastChunkCoord = GetChunkCoordFromVector3(player.position);
    }

    private void Update()
    {
        playerChunkCoord = GetChunkCoordFromVector3(player.position);

		if (!playerChunkCoord.Equals(playerLastChunkCoord))
		{
			CheckViewDistance();
		}
		if (chunksToCreate.Count>0 && !isCreatingChunks)
		{
			StartCoroutine(CreateChunks());
		}
		if (Input.GetKeyDown(KeyCode.F3))
		{
			debugScreen.SetActive(!debugScreen.activeSelf);
		}
	}

    void GenerateWorld()
    {
        for (int x = (VoxelData.WorldSizeInChunks/2)-VoxelData.ViewDistanceInChunks; x < (VoxelData.WorldSizeInChunks / 2) + VoxelData.ViewDistanceInChunks; x++)
        {
            for (int z = (VoxelData.WorldSizeInChunks / 2) - VoxelData.ViewDistanceInChunks; z < (VoxelData.WorldSizeInChunks / 2) + VoxelData.ViewDistanceInChunks; z++)
            {
				chunks[x, z] = new Chunk(new ChunkCoord(x, z), this, true);
				activeChunks.Add(new ChunkCoord(x,z));
            }

        }
        player.position = spawnPosition;
    }

	IEnumerator CreateChunks()
	{
		isCreatingChunks = true;

		while (chunksToCreate.Count > 0)
		{
			chunks[chunksToCreate[0].x, chunksToCreate[0].z].Init();
			chunksToCreate.RemoveAt(0);
			yield return null;
		}


		isCreatingChunks = false;


	}


	ChunkCoord GetChunkCoordFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x/VoxelData.ChunkWidth);
        int z= Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);
        return new ChunkCoord(x, z);
    }

	public Chunk GetChuckFromVector3(Vector3 pos) 
	{
		int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
		int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);
		return chunks[x, z];

	}



    void CheckViewDistance()
    {
        ChunkCoord coord = GetChunkCoordFromVector3(player.position);
        playerLastChunkCoord = playerChunkCoord;

        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunks);

        for (int x = coord.x - VoxelData.ViewDistanceInChunks; x < coord.x + VoxelData.ViewDistanceInChunks; x++)
        {
            for (int z = coord.z - VoxelData.ViewDistanceInChunks; z < coord.z + VoxelData.ViewDistanceInChunks; z++)
            {
                if (IsChunkInWorld(new ChunkCoord(x,z)))
                {
                    if (chunks[x,z]==null)
                    {
						chunks[x, z] = new Chunk(new ChunkCoord(x, z), this, false);
                        chunksToCreate.Add(new ChunkCoord(x, z));
                    }
                    else if(!chunks[x,z].isActive)
                    {
                        chunks[x, z].isActive = true;
                    }
                    activeChunks.Add(new ChunkCoord(x, z));
                }
                for (int i = 0; i < previouslyActiveChunks.Count; i++)
                {
                    if (previouslyActiveChunks[i].Equals(new ChunkCoord(x,z)))
                    {
                        previouslyActiveChunks.RemoveAt(i);
                    }

                }
            }
        }

        foreach (ChunkCoord c in previouslyActiveChunks)
        {
            chunks[c.x, c.z].isActive = false;
        }

    }

    public bool CheckForVoxel(Vector3 pos)
    {
		ChunkCoord thisChunk = new ChunkCoord(pos);
		if ((!IsChunkInWorld(thisChunk)) || (pos.y<0) || (pos.y>VoxelData.ChunkHeight))
		{
			return false;
		}

		if ((chunks[thisChunk.x,thisChunk.z]!=null)&& (chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulate))
		{
			return blockTypes [chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].isSolid;
		}
		return blockTypes[GetVoxel(pos)].isSolid;
    }


    public byte GetVoxel(Vector3 pos)
    {
        int yPos = Mathf.FloorToInt(pos.y);


        //INMUTABLE PASS

        //If outside world, return air
        if (!IsVoxelInWorld(pos))       
            return 0;

        // If bottom blonk of chunk, return bedrock
        if (yPos == 0)
            return 1;

        //BASIC TERRAIN PASS//

        int terrainHeight = Mathf.FloorToInt(biome.terrainHeight* Noise.Get2DPerlin(new Vector2(pos.x,pos.z),0, biome.terrainScale))+biome.solidGroundHeight;
        byte voxelValue = 0;

        if (yPos == terrainHeight)
            voxelValue = 3;
        else if (yPos < terrainHeight && yPos > terrainHeight - 4)
            voxelValue = 5;
        else if (yPos > terrainHeight)
            return 0;
        else
            voxelValue = 2;

        //SECOND PASS//
        if (voxelValue == 2)
        {
            foreach (Lode lode in biome.lodes)
            {
                if (yPos >lode.minHeight && yPos<lode.maxHeight)
                {
                    if (Noise.Get3DPerlin(pos,lode.noiseOffset,lode.scale,lode.threshold))
                    {
                        voxelValue = lode.blockID;
                    }

                }
            }
        }       
        return voxelValue;

    }

   

    bool IsChunkInWorld(ChunkCoord coord)
    {
        if (coord.x > 0 && coord.x < VoxelData.WorldSizeInChunks -1 && coord.z>0 && coord.z<VoxelData.WorldSizeInChunks-1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool IsVoxelInWorld(Vector3 pos)
    {
        if (pos.x >= 0 && pos.x < VoxelData.WorldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.ChunkHeight && pos.z >= 0 && pos.z < VoxelData.WorldSizeInVoxels) 
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

[System.Serializable]
public class BlockType
{
    public string blockName;
    public bool isSolid;
	public Sprite icon;

    [Header("Texture Values")]
    public int backFaceTexture;
    public int frontFaceTexture;
    public int topFaceTexture;
    public int bottomFaceTexture;
    public int leftFaceTexture;
    public int rightFaceTexture;

    //Back, Front, Top, Bottom, Left, Right

    public int GetTextureID (int faceIndex)
    {
        switch (faceIndex)
        {
            case 0:
                return backFaceTexture;
            case 1:
                return frontFaceTexture;
            case 2:
                return topFaceTexture;
            case 3:
                return bottomFaceTexture;
            case 4:
                return leftFaceTexture;
            case 5:
                return rightFaceTexture;
            default:
                Debug.Log("Error in GetTextureID; invalid gace index");
                return 0;
        }
    }
}
