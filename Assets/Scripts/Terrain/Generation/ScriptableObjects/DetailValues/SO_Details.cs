using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_Details", menuName = "Scriptable Objects/Values/Details")]
public class SO_Details : BaseDetailsGeneration
{
    [System.Serializable]
    public class Detail
    {
        [Header("Pick Either prototype or prototype texture, not both")]
        public GameObject prototype = null;
        public Texture2D prototypeTexture = null;
        public float minHeight = 0.1f;
        public float maxHeight = 0.2f;
        public float minSlope = 0;
        public float maxSlope = 1;
        public Color dryColour = Color.white;
        public Color healthyColour = Color.white;
        public Vector2 heightRange = new Vector2(1, 1);
        public Vector2 widthRange = new Vector2(1, 1);
        public float noiseSpread = 0.5f;
        public float overlap = 0.01f;
        public float feather = 0.5f;
        public float density = 0.5f;
        public int inDetailSpacing = 1;
    }
    public List<Detail> details = new List<Detail>()
    {
        new Detail()
    };

    public override void Generate(TerrainData terrainData, Transform parentTransform, TerrainData leftNeighbour = null, TerrainData downNeighbour = null)
    {
        DetailPrototype[] newDetailPrototypes;
        newDetailPrototypes = new DetailPrototype[details.Count];
        int dindex = 0;
        foreach (Detail d in details)
        {
            newDetailPrototypes[dindex] = new DetailPrototype();
            newDetailPrototypes[dindex].prototype = d.prototype;
            newDetailPrototypes[dindex].prototypeTexture = d.prototypeTexture;
            newDetailPrototypes[dindex].dryColor = d.dryColour;
            newDetailPrototypes[dindex].healthyColor = d.healthyColour;
            newDetailPrototypes[dindex].minHeight = d.heightRange.x;
            newDetailPrototypes[dindex].maxHeight = d.heightRange.y;
            newDetailPrototypes[dindex].minWidth = d.widthRange.x;
            newDetailPrototypes[dindex].maxWidth = d.widthRange.y;
            newDetailPrototypes[dindex].noiseSpread = d.noiseSpread;
            if (newDetailPrototypes[dindex].prototype)
            {
                newDetailPrototypes[dindex].usePrototypeMesh = true;
                newDetailPrototypes[dindex].renderMode = DetailRenderMode.VertexLit;
            }
            else
            {
                newDetailPrototypes[dindex].usePrototypeMesh = false;
                newDetailPrototypes[dindex].renderMode = DetailRenderMode.GrassBillboard;
            }
            dindex++;
        }
        terrainData.detailPrototypes = newDetailPrototypes;

        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        for (int i = 0; i < terrainData.detailPrototypes.Length; i++)
        {

            int[,] detailMap = new int[terrainData.detailWidth, terrainData.detailHeight];
            for (int y = 0; y < terrainData.detailResolution; y += details[i].inDetailSpacing)
            {
                for (int x = 0; x < terrainData.detailResolution; x += details[i].inDetailSpacing)
                {
                    if (UnityEngine.Random.Range(0.0f, 1.0f) > details[i].density) continue;
                    int xHM = (int)(x / (float)terrainData.detailWidth * terrainData.heightmapResolution);
                    int yHM = (int)(y / (float)terrainData.detailHeight * terrainData.heightmapResolution);
                    float thisNoise = TerrainUtils.Map(Mathf.PerlinNoise(x * details[i].feather,
                        y * details[i].feather), 0, 1, 0.5f, 1);
                    float thisHeightStart = details[i].minHeight * thisNoise - details[i].overlap * thisNoise;
                    float nextHeightStart = details[i].maxHeight * thisNoise + details[i].overlap * thisNoise;
                    float thisHeight = heightMap[yHM, xHM];
                    float steepness = terrainData.GetSteepness(xHM / (float)terrainData.heightmapResolution, yHM / (float)terrainData.heightmapResolution);
                    if ((thisHeight >= thisHeightStart && thisHeight <= nextHeightStart) &&
                        (steepness >= details[i].minSlope && steepness <= details[i].maxSlope))
                    {
                        detailMap[y, x] = 1;
                    }
                }
            }
            terrainData.SetDetailLayer(0, 0, i, detailMap);
        }
    }

    public void SetValues(List<Detail> details)
    {
        this.details = details;
    }
}
