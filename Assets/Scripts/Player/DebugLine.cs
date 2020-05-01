using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLine : MonoBehaviour
{
    public int maxRenderers;

    List<LineRenderer> lines = new List<LineRenderer>();

    private void Start()
    {
        //for (int i = 0; i < maxRenderers; i++)
        //{
        //    CreateLine(i);
        //}
    }

    void CreateLine(int i)
    {
        GameObject go = new GameObject();
        lines.Add(go.AddComponent<LineRenderer>());
        lines[i].widthMultiplier = 0.05f;
    }

    public void SetLine(Vector3 startPosition, Vector3 endPosition, int index)
    {
        if (index > lines.Count - 1)
            CreateLine(index);

        lines[index].SetPosition(0, startPosition);
        lines[index].SetPosition(1, endPosition);
    }

    public static DebugLine singleton;
    private void Awake()
    {
        singleton = this;
    }
}
