using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_Biome", menuName = "Scriptable Objects/Biomes/Biome")]
public class SO_Biome : ScriptableObject
{
    public BaseTerrainGeneration terrainGenerationValues = null;
    public GenerationTypes generationType = GenerationTypes.Default;
    public List<BaseDetailsGeneration> listOfDetailsGeneration = null;

    public void Generate(TerrainData terrainData, float[,] heightMap, Transform givenTransform, Vector2 offset,
        GameObject leftTerrain = null, GameObject downNeighbour = null)
    {
        if (terrainGenerationValues)
        {
            terrainGenerationValues.GenerateTerrain(terrainData, heightMap, givenTransform, offset, leftTerrain, downNeighbour);
        }
        if (listOfDetailsGeneration.Count != 0)
        {
            foreach (BaseDetailsGeneration detailValues in listOfDetailsGeneration)
            {
                detailValues.Generate(terrainData, givenTransform/*, leftNeighbour, downNeighbour*/);
            }
        }
    }

    //TODO This is gross
    public SO_PerlinValues GetPerlinValues()
    {
        return (SO_PerlinValues)terrainGenerationValues;
    }

    public void SetBaseTerrain(BaseTerrainGeneration terrainGenerationValues)
    {
        this.terrainGenerationValues = terrainGenerationValues;
    }
    public void SetDetailsGenerationList(List<BaseDetailsGeneration> listOfDetailsGeneration)
    {
        this.listOfDetailsGeneration = listOfDetailsGeneration;
    }
}
