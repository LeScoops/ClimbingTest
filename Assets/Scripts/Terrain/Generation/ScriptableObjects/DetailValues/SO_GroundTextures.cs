using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "so_GroundTexture", menuName = "Scriptable Objects/Values/GroundTexture")]
public class SO_GroundTextures : BaseDetailsGeneration
{
    [System.Serializable]
    public class GroundTexture
    {
        public Texture2D texture = null;
        [Range(0,1)]
        public float minHeight = 0.1f;
        [Range(0, 1)]
        public float maxHeight = 0.2f;
        [Range(0, 90)]
        public float minSlope = 0.0f;
        [Range(0, 90)]
        public float maxSlope = 90.0f;
        public float offset = 0.1f;
        public float xNoise = 0.1f;
        public float yNoise = 0.1f;
        public float noiseScale = 0.05f;
        public Vector2 tileOffset = new Vector2(0, 0);
        public Vector2 tileSize = new Vector2(50, 50);
    }
    public List<GroundTexture> groundTextures = new List<GroundTexture>()
    {
        new GroundTexture()
    };

    public override void Generate(TerrainData terrainData, Transform parentTransform, TerrainData leftNeighbour = null, TerrainData downNeighbour = null)
    {
        TerrainLayer[] newSplatPrototype;
        newSplatPrototype = new TerrainLayer[groundTextures.Count];
        int spindex = 0;
        foreach (GroundTexture sh in groundTextures)
        {
            newSplatPrototype[spindex] = new TerrainLayer();
            newSplatPrototype[spindex].diffuseTexture = sh.texture;
            newSplatPrototype[spindex].tileOffset = sh.tileOffset;
            newSplatPrototype[spindex].tileSize = sh.tileSize;
            newSplatPrototype[spindex].diffuseTexture.Apply(true);
            string path = "Assets/TerrainAssets/Layers/" + parentTransform.name + "_" + spindex + ".terrainlayer";
            //string path = "Assets/TerrainAssets/Layers/Layer_" + spindex + ".terrainlayer";
            AssetDatabase.CreateAsset(newSplatPrototype[spindex], path);
            spindex++;
            Selection.activeObject = parentTransform.gameObject;
        }
        terrainData.terrainLayers = newSplatPrototype;

        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        float[,,] splatMapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                float[] splat = new float[terrainData.alphamapLayers];
                for (int i = 0; i < groundTextures.Count; i++)
                {
                    float noise = Mathf.PerlinNoise(x * groundTextures[i].xNoise, y * groundTextures[i].yNoise)
                        * groundTextures[i].noiseScale;
                    float offset = groundTextures[i].offset + noise;
                    float thisHeightStart = groundTextures[i].minHeight - offset;
                    float thisHeightStop = groundTextures[i].maxHeight + offset;
                    float steepness = terrainData.GetSteepness(y / (float)terrainData.alphamapHeight,
                        x / (float)terrainData.alphamapWidth);
                    if (heightMap[x, y] >= thisHeightStart && heightMap[x, y] <= thisHeightStop &&
                        steepness >= groundTextures[i].minSlope && steepness <= groundTextures[i].maxSlope)
                    {
                        splat[i] = 1;
                    }
                }
                NormalizeVector(splat);
                for (int j = 0; j < groundTextures.Count; j++)
                {
                    splatMapData[x, y, j] = splat[j];
                }
            }
        }
        terrainData.SetAlphamaps(0, 0, splatMapData);
    }

    public void SetValues(List<GroundTexture> groundTextures)
    {
        this.groundTextures = groundTextures;
    }

    void NormalizeVector(float[] v)
    {
        float total = 0;
        for (int i = 0; i < v.Length; i++)
        {
            total += v[i];
        }
        for (int i = 0; i < v.Length; i++)
        {
            v[i] /= total;
        }
    }
}
