using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinValueHolder : MonoBehaviour
{
    [SerializeField] SO_PerlinValues perlinValues = null;
    [SerializeField] int biomeIndex = 0;

    public void SetPerlinValues(SO_PerlinValues perlinValues, int biomeIndex)
    {
        this.perlinValues = perlinValues;
        this.biomeIndex = biomeIndex;
    }

    public SO_PerlinValues GetPerlinValues()
    {
        return perlinValues;
    }

    public int GetBiomeIndex()
    {
        return biomeIndex;
    }
}
