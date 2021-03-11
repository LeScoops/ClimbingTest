using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_Voronoi", menuName = "Scriptable Objects/Values/Voronoi")]
public class SO_Voronoi : BaseTerrainGeneration
{
    public int vPeakCount = 3;
    public float vFallOff = 0.2f;
    public float vDropOff = 0.6f;
    public float vMinHeight = 0.25f;
    public float vMaxHeight = 0.4f;
    public VoronoiType voronoiType = VoronoiType.Linear;
    public int smoothIterations = 1;

    BaseTerrainGeneration leftNeighbour;
    BaseTerrainGeneration upNeighbour;
    BaseTerrainGeneration rightNeighbour;
    BaseTerrainGeneration downNeighbour;

    public override void GenerateTerrain(TerrainData terrainData, float[,] heightMap, Transform givenTransform, Vector2 offset,
        GameObject leftTerrain = null, GameObject upNeighbour = null)
    {
        for (int p = 0; p < vPeakCount; p++)
        {
            Vector3 peak = new Vector3(UnityEngine.Random.Range(0, terrainData.heightmapResolution),
                                       UnityEngine.Random.Range(vMinHeight, vMaxHeight),
                                       UnityEngine.Random.Range(0, terrainData.heightmapResolution));

            if (heightMap[(int)peak.x, (int)peak.z] < peak.y)
            {
                heightMap[(int)peak.x, (int)peak.z] = peak.y;
            }
            else
            {
                continue;
            }

            Vector2 peakLocation = new Vector2(peak.x, peak.z);
            float maxDistance = Vector2.Distance(new Vector2(0, 0), new Vector2(terrainData.heightmapResolution, terrainData.heightmapResolution));

            for (int y = 0; y < terrainData.heightmapResolution; y++)
            {
                for (int x = 0; x < terrainData.heightmapResolution; x++)
                {
                    if (!(x == peak.x && y == peak.z))
                    {
                        float distanceToPeak = Vector2.Distance(peakLocation, new Vector2(x, y)) / maxDistance;
                        float height;
                        if (voronoiType == VoronoiType.Combined)
                        {
                            height = peak.y - distanceToPeak * vFallOff - Mathf.Pow(distanceToPeak, vDropOff);
                        }
                        else if (voronoiType == VoronoiType.Power)
                        {
                            height = peak.y - Mathf.Pow(distanceToPeak, vDropOff) * vFallOff;
                        }
                        else if (voronoiType == VoronoiType.SinPow)
                        {
                            height = peak.y - Mathf.Pow(distanceToPeak * 3, vFallOff) -
                                Mathf.Sin(distanceToPeak * 2 * Mathf.PI) / vDropOff;
                        }
                        else
                        {
                            height = peak.y - distanceToPeak * vFallOff;
                        }

                        if (heightMap[x, y] < height)
                        {
                            heightMap[x, y] = height;
                        }
                    }
                }
            }
        }
        Smooth(terrainData, smoothIterations);
        terrainData.SetHeights(0, 0, heightMap);
    }

    public void SetValues(int vPeakCount = 3, float vFallOff = 0.2f, float vDropOff = 0.6f, float vMinHeight = 0.25f,
        float vMaxHeight = 0.4f, VoronoiType voronoiType = VoronoiType.Linear, int smoothIterations = 1)
    {
        this.vPeakCount = vPeakCount;
        this.vFallOff = vFallOff;
        this.vDropOff = vDropOff;
        this.vMinHeight = vMinHeight;
        this.vMaxHeight = vMaxHeight;
        this.voronoiType = voronoiType;
        this.smoothIterations = smoothIterations;
    }

    public override void SetNeighbours(BaseTerrainGeneration leftNeighbour = null, BaseTerrainGeneration upNeighbour = null,
        BaseTerrainGeneration rightNeighbour = null, BaseTerrainGeneration downNeighbour = null)
    {
        this.leftNeighbour = leftNeighbour;
        this.upNeighbour = upNeighbour;
        this.rightNeighbour = rightNeighbour;
        this.downNeighbour = downNeighbour;
    }
}
