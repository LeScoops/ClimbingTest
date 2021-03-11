using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(Terrain))]
[ExecuteInEditMode]
public class BiomeCreator : MonoBehaviour
{
    private TerrainData terrainData;
    private float[,] heightMap;
    private List<BaseDetailsGeneration> listOfDetailValues;

    #region Terrain Generation Values
    [Header("----------")]
    [Header("Terrain Generation")]
    [Header("----------")]
    [SerializeField] bool resetTerrain = true;

    [Header("Perlin Noise")]
    [SerializeField] bool testPerlin = false;
    [SerializeField] float perlinXScale = 0.01f;
    [SerializeField] float perlinYScale = 0.01f;
    [SerializeField] int perlinOctaves = 3;
    [SerializeField] float perlinPersistance = 8;
    [SerializeField] float perlinHeightScale = 0.09f;
    [Range(0, 20)]
    [SerializeField] int perlinSmoothIterations = 1;
    [SerializeField] bool createPerlinSO = false;
    [SerializeField] string perlinSOName = "Default";

    [Header("Voronoi")]
    [SerializeField] bool testVoronoi = false;
    [SerializeField] int vPeakCount = 3;
    [SerializeField] float vFallOff = 0.2f;
    [SerializeField] float vDropOff = 0.6f;
    [SerializeField] float vMinHeight = 0.25f;
    [SerializeField] float vMaxHeight = 0.4f;
    [SerializeField] VoronoiType voronoiType = VoronoiType.Linear;
    [Range(0, 20)]
    [SerializeField] int vSmoothIterations = 1;
    [SerializeField] bool createVoronoiSO = false;
    [SerializeField] string voronoiSOName = "Default";

    [Header("Midpoint Displacement")]
    [SerializeField] bool testMPD = false;
    [SerializeField] float mpdHeightMin = -5;
    [SerializeField] float mpdHeightMax = 5;
    [SerializeField] int mpdHeightDampenerPower = 2;
    [SerializeField] int mpdRoughness = 2;
    [Range(0, 20)]
    [SerializeField] int mpdSmoothIterations = 1;
    [SerializeField] bool createMPDSO = false;
    [SerializeField] string MPDSOName = "Default";

    [Header("Water")]
    [SerializeField] bool testWater = false;
    [SerializeField] GameObject waterGameObject = null;
    [SerializeField] float waterHeight = 0.5f;
    [SerializeField] bool createWaterSO = false;
    [SerializeField] string waterSOName = "Default";
    #endregion
    #region Detail Generation Values
    [Header("----------")]
    [Header("Details Generation")]
    [Header("----------")]
    [SerializeField] bool testDetails = false;
    [SerializeField] List<SO_Details.Detail> details = new List<SO_Details.Detail>()
    {
        new SO_Details.Detail()
    };
    [SerializeField] bool createDetailsSO = false;
    [SerializeField] string detailsSOName = "Default";

    [Header("Trees")]
    [SerializeField] bool testTrees = false;
    [SerializeField] List<SO_Trees.Trees> trees = new List<SO_Trees.Trees>()
    {
        new SO_Trees.Trees()
    };
    [SerializeField] DetailGenerationTypes generationType = DetailGenerationTypes.Grid;
    [SerializeField] int maximumTrees = 1000;
    [SerializeField] int treeSpacing = 5;
    [SerializeField] int terrainLayer = 8;
    [SerializeField] bool createTreesSO = false;
    [SerializeField] string treesSOName = "Default";

    [Header("Ground Textures")]
    [SerializeField] bool testGroundTextures = false;
    [SerializeField] List<SO_GroundTextures.GroundTexture> groundTextures = new List<SO_GroundTextures.GroundTexture>()
    {
        new SO_GroundTextures.GroundTexture()
    };
    [SerializeField] bool createGroundTextureSO = false;
    [SerializeField] string groundTextureSOName = "Default";
    #endregion
    #region Biome Generation Bools
    [Header("----------")]
    [Header("Biome Generation")]
    [Header("----------")]
    [SerializeField] bool testBiome = false;
    [SerializeField] bool addPerlin = false;
    [SerializeField] bool addMPD = false;
    [SerializeField] bool addVoronoi = false;
    [SerializeField] bool addWater = false;
    [SerializeField] bool addTrees = false;
    [SerializeField] bool addDetail = false;
    [SerializeField] bool addTextures = false;
    [SerializeField] bool createBiomeSO = false;
    [SerializeField] string biomeSOName = "Default";
    #endregion

    private void Start()
    {
        terrainData = GetComponent<Terrain>().terrainData;
        listOfDetailValues = new List<BaseDetailsGeneration>();
    }

    private void Update()
    {
        // Terrain Generators
        Perlin();
        Voronoi();
        MPD();
        Water();

        // Detail Generators
        Details();
        Trees();
        GroundTextures();

        // Biome Generator
        Biome();
    }

    #region Perlin Noise
    void Perlin()
    {
        if (testPerlin)
        {
            SO_PerlinValues perlinValues = (SO_PerlinValues)ScriptableObject.CreateInstance("SO_PerlinValues");
            Vector2 seed = new Vector2(Random.Range(0, 10000), Random.Range(0, 10000));
            perlinValues.SetValues(perlinXScale, perlinYScale, perlinOctaves, perlinPersistance, perlinHeightScale, 
                perlinSmoothIterations);            
            perlinValues.GenerateTerrain(terrainData, GetHeightMap(), this.transform, seed);
            testPerlin = false;
        }
        if (createPerlinSO)
        {
            SO_PerlinValues perlinValues = (SO_PerlinValues)ScriptableObject.CreateInstance("SO_PerlinValues");
            perlinValues.SetValues(perlinXScale, perlinYScale, perlinOctaves, perlinPersistance, perlinHeightScale, 
                perlinSmoothIterations);
            AssetDatabase.CreateAsset(perlinValues, "Assets/Resources/ScriptableObjects/Perlin/SO_Perlin_" + perlinSOName + ".asset");
            AssetDatabase.SaveAssets();
            createPerlinSO = false;
        }
    }
    #endregion
    #region Voronoi
    void Voronoi()
    {
        if (testVoronoi)
        {
            SO_Voronoi voronoiValues = (SO_Voronoi)ScriptableObject.CreateInstance("SO_Voronoi");
            voronoiValues.SetValues(vPeakCount, vFallOff, vDropOff, vMinHeight, vMaxHeight, voronoiType, vSmoothIterations);
            voronoiValues.GenerateTerrain(terrainData, GetHeightMap(), this.transform, new Vector2(0,0));
            testVoronoi = false;
        }
        if (createVoronoiSO)
        {
            SO_Voronoi voronoiValues = (SO_Voronoi)ScriptableObject.CreateInstance("SO_Voronoi");
            voronoiValues.SetValues(vPeakCount, vFallOff, vDropOff, vMinHeight, vMaxHeight, voronoiType, vSmoothIterations);
            AssetDatabase.CreateAsset(voronoiValues, "Assets/Resources/ScriptableObjects/Voronoi/SO_Voronoi_" + voronoiSOName + ".asset");
            AssetDatabase.SaveAssets();
            createVoronoiSO = false;
        }
    }
    #endregion
    #region MidpointDisplacement
    void MPD()
    {
        if (testMPD)
        {
            SO_MPD mpdValues = (SO_MPD)ScriptableObject.CreateInstance("SO_MPD");
            mpdValues.SetValues(mpdHeightMin, mpdHeightMax, mpdHeightDampenerPower, mpdRoughness, mpdSmoothIterations);
            mpdValues.GenerateTerrain(terrainData, GetHeightMap(), this.transform, new Vector2(0, 0));
            testMPD = false;
        }
        if (createMPDSO)
        {
            SO_MPD mpdValues = (SO_MPD)ScriptableObject.CreateInstance("SO_MPD");
            mpdValues.SetValues(mpdHeightMin, mpdHeightMax, mpdHeightDampenerPower, mpdRoughness, mpdSmoothIterations);
            AssetDatabase.CreateAsset(mpdValues, "Assets/Resources/ScriptableObjects/MidpointDisplacement/SO_MPD_" + MPDSOName + ".asset");
            AssetDatabase.SaveAssets();
            createMPDSO = false;
        }
    }
    #endregion
    #region Water
    void Water()
    {
        if (testWater)
        {
            SO_Water waterValues = (SO_Water)ScriptableObject.CreateInstance("SO_Water");
            waterValues.SetValues(waterGameObject, waterHeight);
            waterValues.Generate(terrainData, this.transform);
            testWater = false;
        }
        if (createWaterSO)
        {
            SO_Water waterValues = (SO_Water)ScriptableObject.CreateInstance("SO_Water");
            waterValues.SetValues(waterGameObject, waterHeight);
            AssetDatabase.CreateAsset(waterValues, "Assets/Resources/ScriptableObjects/Water/SO_Water_" + waterSOName + ".asset");
            AssetDatabase.SaveAssets();
            createWaterSO = false;
        }
    }
    #endregion
    #region Details
    void Details()
    {
        if (testDetails)
        {
            SO_Details detailValues = (SO_Details)ScriptableObject.CreateInstance("SO_Details");
            detailValues.SetValues(details);
            detailValues.Generate(terrainData, this.transform);
            testDetails = false;
        }
        if (createDetailsSO)
        {
            SO_Details detailValues = (SO_Details)ScriptableObject.CreateInstance("SO_Details");
            detailValues.SetValues(details);
            AssetDatabase.CreateAsset(detailValues, "Assets/Resources/ScriptableObjects/Details/SO_Details_" + detailsSOName + ".asset");
            AssetDatabase.SaveAssets();
            createDetailsSO = false;
        }
    }
    #endregion
    #region Trees
    void Trees()
    {
        if (testTrees)
        {
            SO_Trees treeValues = (SO_Trees)ScriptableObject.CreateInstance("SO_Trees");
            treeValues.SetValues(trees, generationType, maximumTrees, treeSpacing, terrainLayer);
            treeValues.Generate(terrainData, this.transform);
            testTrees = false;
        }
        if (createTreesSO)
        {
            SO_Trees treeValues = (SO_Trees)ScriptableObject.CreateInstance("SO_Trees");
            treeValues.SetValues(trees, generationType, maximumTrees, treeSpacing, terrainLayer);
            AssetDatabase.CreateAsset(treeValues, "Assets/Resources/ScriptableObjects/Trees/SO_Trees_" + treesSOName + ".asset");
            AssetDatabase.SaveAssets();
            createTreesSO = false;
        }
    }
    #endregion
    #region Ground Textures
    void GroundTextures()
    {
        if (testGroundTextures)
        {
            SO_GroundTextures groundTextureValues = (SO_GroundTextures)ScriptableObject.CreateInstance("SO_GroundTextures");
            groundTextureValues.SetValues(groundTextures);
            groundTextureValues.Generate(terrainData, this.transform);
            testGroundTextures = false;
        }
        if (createGroundTextureSO)
        {
            SO_GroundTextures groundTextureValues = (SO_GroundTextures)ScriptableObject.CreateInstance("SO_GroundTextures");
            groundTextureValues.SetValues(groundTextures);
            AssetDatabase.CreateAsset(groundTextureValues, "Assets/Resources/ScriptableObjects/GroundTextures/SO_GroundTextures_"
                + groundTextureSOName + ".asset");
            AssetDatabase.SaveAssets();
            createGroundTextureSO = false;
        }
    }
    #endregion
    #region Biome
    void Biome()
    {
        if (addPerlin)
        {
            addVoronoi = addMPD = false;
        }
        else if (addVoronoi)
        {
            addPerlin = addMPD = false;
        }
        else if (addMPD)
        {
            addPerlin = addVoronoi = false;
        }

        if (testBiome)
        {
            SO_Biome biomeValues = (SO_Biome)ScriptableObject.CreateInstance("SO_Biome");
            BiomeTerrainValues(biomeValues);
            BiomeDetailValues(biomeValues);
            Vector2 seed = new Vector2(Random.Range(0, 10000), Random.Range(0, 10000));
            biomeValues.Generate(terrainData, GetHeightMap(), this.transform, seed);
            DeleteBiomeAssets();
            testBiome = false;
        }

        if (createBiomeSO)
        {
            SO_Biome biomeValues = (SO_Biome)ScriptableObject.CreateInstance("SO_Biome");
            if (addPerlin)
            {
                biomeValues.generationType = GenerationTypes.SinglePerlin;
            }
            else if (addVoronoi)
            {
                biomeValues.generationType = GenerationTypes.Voronoi;
            }
            else if (addMPD)
            {
                biomeValues.generationType = GenerationTypes.MidpointDisplacement;
            }
            BiomeTerrainValues(biomeValues);
            BiomeDetailValues(biomeValues);
            AssetDatabase.CreateAsset(biomeValues, "Assets/Resources/ScriptableObjects/Biomes/SO_Biome_"
                + biomeSOName + ".asset");
            AssetDatabase.SaveAssets();
            createBiomeSO = false;
        }
    }

    private void BiomeDetailValues(SO_Biome biomeValues)
    {
        listOfDetailValues.Clear();
        if (addWater)
        {
            SO_Water waterValues = (SO_Water)ScriptableObject.CreateInstance("SO_Water");
            waterValues.SetValues(waterGameObject, waterHeight);
            AssetDatabase.CreateAsset(waterValues, "Assets/Resources/ScriptableObjects/Water/SO_Water_" + biomeSOName + ".asset");
            AssetDatabase.SaveAssets();
            listOfDetailValues.Add(waterValues);
        }
        if (addDetail)
        {
            SO_Details detailValues = (SO_Details)ScriptableObject.CreateInstance("SO_Details");
            detailValues.SetValues(details);
            AssetDatabase.CreateAsset(detailValues, "Assets/Resources/ScriptableObjects/Details/SO_Details_" + biomeSOName + ".asset");
            AssetDatabase.SaveAssets();
            listOfDetailValues.Add(detailValues);
        }
        if (addTrees)
        {
            SO_Trees treeValues = (SO_Trees)ScriptableObject.CreateInstance("SO_Trees");
            treeValues.SetValues(trees, generationType, maximumTrees, treeSpacing, terrainLayer);
            AssetDatabase.CreateAsset(treeValues, "Assets/Resources/ScriptableObjects/Trees/SO_Trees_" + biomeSOName + ".asset");
            AssetDatabase.SaveAssets();
            listOfDetailValues.Add(treeValues);
        }
        if (addTextures)
        {
            SO_GroundTextures groundTextureValues = (SO_GroundTextures)ScriptableObject.CreateInstance("SO_GroundTextures");
            groundTextureValues.SetValues(groundTextures);
            AssetDatabase.CreateAsset(groundTextureValues, "Assets/Resources/ScriptableObjects/GroundTextures/SO_GroundTextures_"
                + biomeSOName + ".asset");
            AssetDatabase.SaveAssets();
            listOfDetailValues.Add(groundTextureValues);
        }

        if (!addWater)
        {
            if (transform.Find("water"))
            {
                DestroyImmediate(transform.Find("water").gameObject);
            }
        }
        if (!addDetail)
        {
            int[,] detailMap = new int[terrainData.detailWidth, terrainData.detailHeight];
            for (int i = 0; i < terrainData.detailPrototypes.Length; i++)
            {
                terrainData.SetDetailLayer(0, 0, i, detailMap);
            }
            terrainData.detailPrototypes = null;
        }
        if (!addTrees)
        {
            TreeInstance[] currentTreeInstances = new TreeInstance[0];
            terrainData.SetTreeInstances(currentTreeInstances, false);
            terrainData.treePrototypes = null;
        }
        if (!addTextures)
        {
            float[,,] splatMapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];
            terrainData.SetAlphamaps(0, 0, splatMapData);
            terrainData.terrainLayers = null;
        }
        biomeValues.SetDetailsGenerationList(listOfDetailValues);
    }

    private void BiomeTerrainValues(SO_Biome biomeValues)
    {
        if (addPerlin)
        {
            SO_PerlinValues perlinValues = (SO_PerlinValues)ScriptableObject.CreateInstance("SO_PerlinValues");
            perlinValues.SetValues(perlinXScale, perlinYScale, perlinOctaves, perlinPersistance, perlinHeightScale,
                perlinSmoothIterations);
            AssetDatabase.CreateAsset(perlinValues, "Assets/Resources/ScriptableObjects/Perlin/SO_Perlin_" + biomeSOName + ".asset");
            AssetDatabase.SaveAssets();
            biomeValues.SetBaseTerrain(perlinValues);
        }
        else if (addVoronoi)
        {
            SO_Voronoi voronoiValues = (SO_Voronoi)ScriptableObject.CreateInstance("SO_Voronoi");
            voronoiValues.SetValues(vPeakCount, vFallOff, vDropOff, vMinHeight, vMaxHeight, voronoiType, vSmoothIterations);
            AssetDatabase.CreateAsset(voronoiValues, "Assets/Resources/ScriptableObjects/Voronoi/SO_Voronoi_" + biomeSOName + ".asset");
            AssetDatabase.SaveAssets();
            biomeValues.SetBaseTerrain(voronoiValues);
        }
        else if (addMPD)
        {
            SO_MPD mpdValues = (SO_MPD)ScriptableObject.CreateInstance("SO_MPD");
            mpdValues.SetValues(mpdHeightMin, mpdHeightMax, mpdHeightDampenerPower, mpdRoughness, mpdSmoothIterations);
            AssetDatabase.CreateAsset(mpdValues, "Assets/Resources/ScriptableObjects/MidpointDisplacement/SO_MPD_" + biomeSOName + ".asset");
            AssetDatabase.SaveAssets();
            biomeValues.SetBaseTerrain(mpdValues);
        }
    }

    private void DeleteBiomeAssets()
    {
        if (addPerlin)
        {
            AssetDatabase.DeleteAsset("Assets/Resources/ScriptableObjects/Perlin/SO_Perlin_" + biomeSOName + ".asset");
        }
        else if (addVoronoi)
        {
            AssetDatabase.DeleteAsset("Assets/Resources/ScriptableObjects/Voronoi/SO_Voronoi_" + biomeSOName + ".asset");
        }
        else if (addMPD)
        {
            AssetDatabase.DeleteAsset("Assets/Resources/ScriptableObjects/MidpointDisplacement/SO_MPD_" + biomeSOName + ".asset");
        }

        if (addWater)
        {
            AssetDatabase.DeleteAsset("Assets/Resources/ScriptableObjects/Water/SO_Water_" + biomeSOName + ".asset");
        }
        if (addDetail)
        {
            AssetDatabase.DeleteAsset("Assets/Resources/ScriptableObjects/Details/SO_Details_" + biomeSOName + ".asset");
        }
        if (addTrees)
        {
            AssetDatabase.DeleteAsset("Assets/Resources/ScriptableObjects/Trees/SO_Trees_" + biomeSOName + ".asset");
        }
        if (addTextures)
        {
            AssetDatabase.DeleteAsset("Assets/Resources/ScriptableObjects/GroundTextures/SO_GroundTextures_"
                + biomeSOName + ".asset");
        }
    }
    #endregion


    #region Utility
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
    #endregion
}
