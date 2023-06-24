using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class NavMeshBaker
{
    private NavMeshSurface sq;

    public void Bake(NavMeshSurface surface)
    {
        /*#if UNITY_EDITOR
        UnityEditor.AI.NavMeshBuilder.ClearAllNavMeshes();
        UnityEditor.AI.NavMeshBuilder.BuildNavMesh();
        #else
        UnityEngine.AI.NavMeshBuilder.ClearAllNavMeshes();
        UnityEngine.AI.NavMeshBuilder.BuildNavMesh();
        #endif*/
        surface.BuildNavMesh();
    }
}
