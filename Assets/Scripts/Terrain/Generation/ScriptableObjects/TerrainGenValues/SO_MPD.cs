using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_MPD", menuName = "Scriptable Objects/Values/MidpointDisplacement")]
public class SO_MPD : BaseTerrainGeneration
{
    public float heightMin = -5;
    public float heightMax = 5;
    public int heightDampenerPower = 2;
    public int roughness = 2;
    public int smoothIterations = 1;

    BaseTerrainGeneration leftNeighbour;
    BaseTerrainGeneration upNeighbour;
    BaseTerrainGeneration rightNeighbour;
    BaseTerrainGeneration downNeighbour;

    public override void GenerateTerrain(TerrainData terrainData, float[,] heightMap, Transform givenTransform, Vector2 offset,
        GameObject leftTerrain = null, GameObject upNeighbour = null)
    {
        int width = terrainData.heightmapResolution - 1;
        int squareSize = width;
        float heightDampener = (float)Mathf.Pow(heightDampenerPower, -1 * roughness);

        int cornerX, cornerY;
        int midX, midY;
        int pmidXL, pmidXR, pmidYU, pmidYD;

        while (squareSize > 0)
        {
            for (int x = 0; x < width; x += squareSize)
            {
                for (int y = 0; y < width; y += squareSize)
                {
                    cornerX = (x + squareSize);
                    cornerY = (y + squareSize);

                    midX = (int)(x + squareSize / 2.0f);
                    midY = (int)(y + squareSize / 2.0f);

                    heightMap[midX, midY] = (float)((heightMap[x, y] +
                                                     heightMap[cornerX, y] +
                                                     heightMap[x, cornerY] +
                                                     heightMap[cornerX, cornerY]) / 4.0f +
                                                     UnityEngine.Random.Range(heightMin, heightMax));
                }
            }

            for (int x = 0; x < width; x += squareSize)
            {
                for (int y = 0; y < width; y += squareSize)
                {
                    cornerX = (x + squareSize);
                    cornerY = (y + squareSize);
                    midX = (int)(x + squareSize / 2.0f);
                    midY = (int)(y + squareSize / 2.0f);
                    pmidXR = (int)(midX + squareSize);
                    pmidYU = (int)(midY + squareSize);
                    pmidXL = (int)(midX - squareSize);
                    pmidYD = (int)(midY - squareSize);

                    if (pmidXL <= 0 || pmidYD <= 0 || pmidXR >= width - 1 || pmidYU >= width - 1)
                        continue;

                    heightMap[midX, y] = (float)((heightMap[midX, midY] +
                                                  heightMap[x, y] +
                                                  heightMap[midX, pmidYD] +
                                                  heightMap[cornerX, y]) / 4.0f + UnityEngine.Random.Range(heightMin, heightMax));

                    heightMap[midX, cornerY] = (float)((heightMap[x, cornerY] +
                                                        heightMap[midX, midY] +
                                                        heightMap[cornerX, cornerY] +
                                                        heightMap[midX, pmidYU]) / 4.0f + UnityEngine.Random.Range(heightMin, heightMax));

                    heightMap[x, midY] = (float)((heightMap[x, y] +
                                                  heightMap[pmidXL, midY] +
                                                  heightMap[x, cornerY] +
                                                  heightMap[midX, midY]) / 4.0f + UnityEngine.Random.Range(heightMin, heightMax));

                    heightMap[cornerX, midY] = (float)((heightMap[cornerX, y] +
                                                        heightMap[midX, midY] +
                                                        heightMap[cornerX, cornerY] +
                                                        heightMap[pmidXR, midY]) / 4.0f + UnityEngine.Random.Range(heightMin, heightMax));
                }
            }
            squareSize = (int)(squareSize / 2.0f);
            heightMin *= heightDampener;
            heightMax *= heightDampener;
        }
        Smooth(terrainData, smoothIterations);

        terrainData.SetHeights(0, 0, heightMap);
    }

    public void SetValues(float mpdHeightMin = -5, float mpdHeightMax = 5, int mpdHeightDampenerPower = 2, int mpdRoughness = 2, int mpdSmoothIterations = 1)
    {
        heightMin = mpdHeightMin;
        heightMax = mpdHeightMax;
        heightDampenerPower = mpdHeightDampenerPower;
        roughness = mpdRoughness;
        smoothIterations = mpdSmoothIterations;
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
