using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class TerrainNavMeshObstacleSetter
{
    private TerrainTool Tool;
    private GameObject created;

    public TerrainNavMeshObstacleSetter(TerrainTool tool)
    {
        Tool = tool;
    }

    private Vector3 GetWorldPosition(Vector3 pos)
    {
        return GetWorldPosition(pos.x, pos.z);
    }

    private Vector3 GetWorldPosition(float x, float y)
    {
        var width = Tool._TerrainData.alphamapWidth;
        var height = Tool._TerrainData.alphamapHeight;
        float realW = Tool._TerrainData.size.x;
        float realH = Tool._TerrainData.size.z;
        Vector3 pos = new Vector3(((float)x) / width * realW - realW / 2, 0, ((float)y) / height * realH - realH / 2);
        return pos;
    }

    public void Set()
    {
        var terrain = Tool._Terrain;
        var treeInstances = Tool._TerrainData.treeInstances;

        GameObject parent = new GameObject("Tree_Obstacles");
        parent.transform.parent = Tool.transform;
        parent.transform.position = Vector3.zero;
        created = parent;
        foreach (var tree in treeInstances) { 
            var prefab = terrain.terrainData.treePrototypes[tree.prototypeIndex].prefab;

            var sq = GameObject.Instantiate(prefab);
            sq.transform.parent = parent.transform;
            sq.transform.position = tree.position * Tool._TerrainData.size.x - Tool._TerrainData.size.x / 2f * new Vector3(1, 0, 1);
            var scale = sq.transform.localScale;
            sq.transform.localScale = new Vector3(tree.widthScale * scale.x, tree.heightScale * scale.y, tree.widthScale * scale.z);
        }

        GameObject.Find("SafeZone"). GetComponentsInChildren<SphereCollider>()[1].enabled = true;
    }

    public void Reset()
    {
        GameObject.DestroyImmediate(created);
        created = null;
        GameObject.Find("SafeZone").GetComponentsInChildren<SphereCollider>()[1].enabled = false;
    }
}