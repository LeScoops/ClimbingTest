using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseDetailsGeneration : ScriptableObject
{
    public abstract void Generate(TerrainData terrainData, Transform parentTransform, TerrainData leftNeighbour = null, TerrainData downNeighbour = null);
}
