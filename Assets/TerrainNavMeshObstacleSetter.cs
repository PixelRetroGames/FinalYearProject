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
        //parent.transform.localScale = Tool._TerrainData.size;
        created = parent;
        int sqq = 20;
        foreach (var tree in treeInstances) { 
            var prefab = terrain.terrainData.treePrototypes[tree.prototypeIndex].prefab;
            var colliders = prefab.GetComponents<Collider>().ToList().Concat(prefab.GetComponentsInChildren<Collider>().ToList());

            var sq = GameObject.Instantiate(prefab);
            sq.transform.parent = parent.transform;
            sq.transform.position = tree.position * Tool._TerrainData.size.x - Tool._TerrainData.size.x / 2f * new Vector3(1, 0, 1);
            var scale = sq.transform.localScale;
            sq.transform.localScale = new Vector3(tree.widthScale * scale.x, tree.heightScale * scale.y, tree.widthScale * scale.z);
            continue;
            sqq--;
            if (sqq == 0)
            {
                break;
            }
            /*foreach (var collider in colliders)
            {
                var obstacle = new GameObject("Obstacle");
                obstacle.transform.SetParent(parent.transform);
                obstacle.transform.position = GetWorldPosition(tree.position);
                obstacle.transform.localScale = new Vector3(tree.widthScale, 1, tree.heightScale);

                var navObstacle = obstacle.AddComponent<NavMeshObstacle>();
                navObstacle.carving = true;
                navObstacle.carveOnlyStationary = true;

                if (collider.GetType() == typeof(CapsuleCollider))
                {
                    CapsuleCollider capsule = (CapsuleCollider) collider;
                    navObstacle.shape = NavMeshObstacleShape.Capsule;
                    navObstacle.center = capsule.center;
                    navObstacle.radius = capsule.radius;
                    navObstacle.height = capsule.height;
                }
                else if (collider.GetType() == typeof(BoxCollider))
                {
                    BoxCollider box = (BoxCollider) collider;
                    navObstacle.shape = NavMeshObstacleShape.Box;
                    navObstacle.center = box.center;
                    navObstacle.size = box.size;
                }
            }*/
        }
    }

    public void Reset()
    {
        GameObject.DestroyImmediate(created);
        created = null;
    }
}