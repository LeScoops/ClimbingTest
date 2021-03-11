using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "so_Trees", menuName = "Scriptable Objects/Values/Trees")]
public class SO_Trees : BaseDetailsGeneration
{
    [System.Serializable]
    public class Trees
    {
        public GameObject mesh;
        public float minHeight = 0.1f;
        public float maxHeight = 0.2f;
        public float scattering = 5.0f;
        public float minSlope = 0;
        public float maxSlope = 90;
        public float minScale = 0.5f;
        public float maxScale = 1.0f;
        public Color colour1 = Color.white;
        public Color colour2 = Color.white;
        public Color lightColour = Color.white;
        public int numberOfTrees = 100;
    }
    public List<Trees> trees = new List<Trees>()
    {
        new Trees()
    };
    public DetailGenerationTypes generationType = DetailGenerationTypes.Grid;
    public int maximumTrees = 1000;
    public int treeSpacing = 5;
    [SerializeField] int terrainLayer = 8;

    public override void Generate(TerrainData terrainData, Transform parentTransform, TerrainData leftNeighbour = null, TerrainData downNeighbour = null)
    {
        TreePrototype[] newTreePrototypes;
        newTreePrototypes = new TreePrototype[trees.Count];
        int tindex = 0;
        foreach (SO_Trees.Trees t in trees)
        {
            newTreePrototypes[tindex] = new TreePrototype();
            newTreePrototypes[tindex].prefab = t.mesh;
            tindex++;
        }
        terrainData.treePrototypes = newTreePrototypes;

        if (generationType == DetailGenerationTypes.Grid)
        {
            List<TreeInstance> allVegetation = new List<TreeInstance>();
            float treesProgress = 0;
            EditorUtility.DisplayProgressBar("Generating Trees", "Progress", treesProgress);
            for (int z = 0; z < terrainData.size.z; z += treeSpacing)
            {
                for (int x = 0; x < terrainData.size.x; x += treeSpacing)
                {
                    for (int tp = 0; tp < terrainData.treePrototypes.Length; tp++)
                    {
                        float thisHeight = terrainData.GetHeight(x, z) / terrainData.size.y;
                        float thisHeightStart = trees[tp].minHeight;
                        float thisHeightEnd = trees[tp].maxHeight;

                        float steepness = terrainData.GetSteepness(x / terrainData.size.x, z / terrainData.size.z);

                        if ((thisHeight >= thisHeightStart && thisHeight <= thisHeightEnd) &&
                            steepness >= trees[tp].minSlope && steepness <= trees[tp].maxSlope)
                        {
                            TreeInstance instance = new TreeInstance();
                            instance.position = new Vector3(
                                (x + Random.Range(-trees[tp].scattering, trees[tp].scattering)) / terrainData.size.x,
                                terrainData.GetHeight(x, z) / terrainData.size.y,
                                (z + Random.Range(-trees[tp].scattering, trees[tp].scattering)) / terrainData.size.z);

                            Vector3 treeWorldPos = new Vector3(instance.position.x * terrainData.size.x,
                                instance.position.y * terrainData.size.y,
                                instance.position.z * terrainData.size.z) + parentTransform.position;

                            RaycastHit hit;
                            int layerMask = terrainLayer;
                            if (Physics.Raycast(treeWorldPos, -Vector3.up, out hit, 100, layerMask) ||
                                Physics.Raycast(treeWorldPos, Vector3.up, out hit, 100, layerMask))
                            {
                                float treeHeight = (hit.point.y - parentTransform.position.y) / terrainData.size.y;
                                instance.position = new Vector3(instance.position.x, treeHeight, instance.position.z);
                                instance.rotation = Random.Range(0, 360);
                                instance.prototypeIndex = tp;
                                instance.color = Color.Lerp(trees[tp].colour1, trees[tp].colour2, Random.Range(0.0f, 1.0f));
                                instance.lightmapColor = trees[tp].lightColour;
                                float scale = Random.Range(trees[tp].minScale, trees[tp].maxScale);
                                instance.heightScale = scale;
                                instance.widthScale = scale;
                                allVegetation.Add(instance);
                                if (allVegetation.Count >= maximumTrees) goto TREESDONE;
                                treesProgress++;
                                EditorUtility.DisplayProgressBar("Generating Trees", "Progress", treesProgress / trees[tp].numberOfTrees);
                            }
                        }
                    }
                }
            }
        TREESDONE:
            terrainData.treeInstances = allVegetation.ToArray();
            EditorUtility.ClearProgressBar();
        }
        else if (generationType == DetailGenerationTypes.Random)
        {
            List<TreeInstance> allVegetation = new List<TreeInstance>();
            float treesProgress = 0;
            EditorUtility.DisplayProgressBar("Generating Trees", "Progress", treesProgress);
            for (int tp = 0; tp < terrainData.treePrototypes.Length; tp++)
            {
                int treesSpawned = 0;
                while (treesSpawned < trees[tp].numberOfTrees)
                {
                    TreeInstance instance = new TreeInstance();
                    int x = Random.Range(0, (int)terrainData.size.x);
                    int z = Random.Range(0, (int)terrainData.size.z);

                    if (terrainData.GetHeight(x, z) / terrainData.size.y < trees[tp].minHeight ||
                        terrainData.GetHeight(x, z) / terrainData.size.y > trees[tp].maxHeight)
                    {
                        continue;
                    }
                    instance.position = new Vector3(x / terrainData.size.x,
                                                    terrainData.GetHeight(x, z) / terrainData.size.y,
                                                    z / terrainData.size.z);

                    float targetX = instance.position.x * terrainData.size.x / terrainData.alphamapWidth;
                    float targetZ = instance.position.z * terrainData.size.z / terrainData.alphamapHeight;
                    if (targetX > 1f || targetZ > 1f) { continue; }

                    float steepness = terrainData.GetSteepness(targetX, targetZ);
                    if (steepness <= trees[tp].minSlope - 0.1f || steepness >= trees[tp].maxSlope) { continue; }

                    instance.position = new Vector3(targetX, instance.position.y, targetZ);
                    instance.rotation = Random.Range(0, 360);
                    instance.prototypeIndex = tp;
                    instance.color = Color.Lerp(trees[tp].colour1, trees[tp].colour2, Random.Range(0.0f, 1.0f));
                    instance.lightmapColor = trees[tp].lightColour;
                    float scale = Random.Range(trees[tp].minScale, trees[tp].maxScale);
                    instance.heightScale = scale;
                    instance.widthScale = scale;
                    allVegetation.Add(instance);
                    treesSpawned++;
                    treesProgress++;
                    EditorUtility.DisplayProgressBar("Generating Trees", "Progress", treesProgress/ trees[tp].numberOfTrees);
                }
            }
            terrainData.treeInstances = allVegetation.ToArray();
            EditorUtility.ClearProgressBar();
        }
    }

    public void RemoveTrees(TerrainData terrainData)
    {
        terrainData.treePrototypes = null;
    }

    public void SetValues(List<Trees> trees, DetailGenerationTypes generationType = DetailGenerationTypes.Random, int maximumTrees = 1000,
        int treeSpacing = 5, int terrainLayer = 8)
    {
        this.trees = trees;
        this.generationType = generationType;
        this.maximumTrees = maximumTrees;
        this.treeSpacing = treeSpacing;
        this.terrainLayer = terrainLayer;
    }
}
