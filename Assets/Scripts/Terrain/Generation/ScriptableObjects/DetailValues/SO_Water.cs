using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_Water", menuName = "Scriptable Objects/Values/Water")]
public class SO_Water : BaseDetailsGeneration
{
    [Header("Water")]
    public GameObject waterGameObject;
    public float waterHeight = 0.5f;

    //TODO AddShoreline
    //[Header("Shoreline")]
    /*public*/ Material shoreLineMaterial = null;
    /*public*/ float shoreSize = 10.0f;

    public override void Generate(TerrainData terrainData, Transform transform, TerrainData leftNeighbour = null, TerrainData downNeighbour = null)
    {
        AddWater(terrainData, transform);
        //AddShoreline(terrainData, transform);
    }

    public void AddWater(TerrainData terrainData, Transform transform)
    {
        GameObject water = GameObject.Find("water");
        if (!water)
        {
            water = Instantiate(waterGameObject, transform.position, transform.rotation);
            water.name = "water";
        }
        water.transform.position = transform.position + new Vector3(terrainData.size.x / 2,
                                                                        waterHeight * terrainData.size.y,
                                                                        terrainData.size.z / 2);
        water.transform.localScale = new Vector3(terrainData.size.x * 0.75f, 1, terrainData.size.z * 0.75f);
        water.transform.SetParent(transform);
    }

    public void AddShoreline(TerrainData terrainData, Transform transform)
    {
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);

        for (int y = 0; y < terrainData.heightmapResolution; y++)
        {
            for (int x = 0; x < terrainData.heightmapResolution; x++)
            {
                Vector2 thisLocation = new Vector2(x, y);
                List<Vector2> neighbours = GenerateNeighbours(thisLocation, terrainData.heightmapResolution, terrainData.heightmapResolution);
                foreach (Vector2 n in neighbours)
                {
                    if (heightMap[x, y] < waterHeight && heightMap[(int)n.x, (int)n.y] > waterHeight)
                    {
                        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                        go.GetComponent<MeshRenderer>().material = shoreLineMaterial;
                        go.transform.localScale *= shoreSize;

                        go.transform.position = transform.position +
                            new Vector3(y / (float)terrainData.heightmapResolution * terrainData.size.z,
                                        waterHeight * terrainData.size.y,
                                        x / (float)terrainData.heightmapResolution * terrainData.size.x);

                        go.transform.LookAt(new Vector3(n.y / (float)terrainData.heightmapResolution * terrainData.size.z,
                                                        waterHeight * terrainData.size.y,
                                                        n.x / (float)terrainData.heightmapResolution * terrainData.size.x));
                        go.transform.Rotate(90, 0, 0);
                        go.tag = "Shore";
                        go.layer = LayerMask.NameToLayer("Water");
                    }
                }
            }
        }

        GameObject[] shoreQuads = GameObject.FindGameObjectsWithTag("Shore");
        MeshFilter[] meshFilters = new MeshFilter[shoreQuads.Length];
        for (int m = 0; m < shoreQuads.Length; m++)
        {
            meshFilters[m] = shoreQuads[m].GetComponent<MeshFilter>();
        }
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
            i++;
        }

        GameObject currentShoreLine = GameObject.Find("ShoreLine");
        if (currentShoreLine)
        {
            DestroyImmediate(currentShoreLine);
        }
        GameObject shoreLine = new GameObject();
        shoreLine.name = "ShoreLine";
        //shoreLine.transform.SetParent(transform);
        //shoreLine.AddComponent<WaveAnimation>();
        shoreLine.transform.position = transform.position;
        shoreLine.transform.rotation = transform.rotation;
        MeshFilter thisMF = shoreLine.AddComponent<MeshFilter>();
        thisMF.mesh = new Mesh();
        shoreLine.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);

        MeshRenderer r = shoreLine.AddComponent<MeshRenderer>();
        r.sharedMaterial = shoreLineMaterial;

        for (int sQ = 0; sQ < shoreQuads.Length; sQ++)
        {
            DestroyImmediate(shoreQuads[sQ]);
        }
    }

    public void SetValues(GameObject waterGameObject = null, float waterHeight = 0.5f)
    {
        this.waterGameObject = waterGameObject;
        this.waterHeight = waterHeight;
    }

    List<Vector2> GenerateNeighbours(Vector2 pos, int width, int height)
    {
        List<Vector2> neighbours = new List<Vector2>();
        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                if (!(x == 0 && y == 0))
                {
                    Vector2 nPos = new Vector2(Mathf.Clamp(pos.x + x, 0, width - 1), Mathf.Clamp(pos.y + y, 0, height - 1));
                    if (!neighbours.Contains(nPos))
                    {
                        neighbours.Add(nPos);
                    }
                }
            }
        }
        return neighbours;
    }
}
