using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="BiomeAttributes",menuName ="Minecraft/Biome Attribute")]
public class BiomeAttributes :ScriptableObject
{

    public string biomeName;

    public int solidGroundHeight;
    public int terrainHeight;
    public float terrainScale;

	[Header("Tress")]
	public float treeZoneScale=1.3f;
	public float treeZoneThreshold=0.6f;
			


    public Lode[] lodes;
}

[System.Serializable]
public class Lode
{
    //FOR ORES or other blocktypes were they can spawn

    public string nodeName;
    public byte blockID;
    public int minHeight;
    public int maxHeight;
    public float scale;
    public float threshold;
    public float noiseOffset;
        
}