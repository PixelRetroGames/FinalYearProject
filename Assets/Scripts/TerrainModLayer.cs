using System;
using UnityEngine;

[Serializable]
public abstract class TerrainModLayer : UnityEngine.ScriptableObject
{
    [UnityEngine.HideInInspector]
    public TerrainTool Tool;
    public abstract void Apply();
    public abstract void Rebuild();

    public Vector3 GetWorldPosition(float x, float y)
    {
        var width = Tool._TerrainData.alphamapWidth;
        var height = Tool._TerrainData.alphamapHeight;
        float realW = Tool._TerrainData.size.x;
        float realH = Tool._TerrainData.size.z;
        Vector3 pos = new Vector3(((float)x) / width * realW - realW / 2, 0, ((float)y) / height * realH - realH / 2);
        return pos;
    }
}