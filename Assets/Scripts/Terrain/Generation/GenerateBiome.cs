using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GenerateBiome : MonoBehaviour
{
    [SerializeField] bool resetTerrain = false;
    [SerializeField] bool generateBiome = false;
    [SerializeField] SO_Biome biome = null;

    Terrain terrain = null;
    TerrainData terrainData = null;

    private void OnEnable()
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;
    }

    void Update()
    {
        if (generateBiome)
        {
            if (!biome)
            {
                Debug.Log("Missing biome info");
                return;
            }

            Generate();
            generateBiome = false;
        }
    }

    void Generate()
    {
        Vector2 seed = new Vector2(Random.Range(0, 10000), Random.Range(0, 10000));
        biome.Generate(terrainData, GetHeightMap(), this.transform, seed);
    }

    float[,] GetHeightMap()
    {
        if (!resetTerrain)
        {
            return terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        }
        else
        {
            return new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
        }
    }
}
