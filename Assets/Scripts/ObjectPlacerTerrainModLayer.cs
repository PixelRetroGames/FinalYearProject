using Cinemachine;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Terrain/Layers/ObjectPlacer")]
[Serializable]
public class ObjectPlacerTerrainModLayer : TerrainModLayer
{
    // position and distance
    private static List<Tuple<Vector3, float>> illegalZones = new List<Tuple<Vector3, float>>();

    private static bool layersSet = false;
    private static List<TreePrototype> objects = new List<TreePrototype>();

    public bool createNavMeshAfterPlacing = false;
    public float[,,] splatMap = null;

    public bool disabled;

    [System.Serializable]
    public class TerrainLayerObjectsProps
    {
        public GameObject[] objects;
        public int minDistance;
        public int splatMapLayer;
        [HideInInspector]
        public int[] indexes;
        public bool disabled;
        [Range(0.0f, 1.0f)]
        public float noSpawnChance;

        [Range(0.0f, 1.0f)]
        public float threshold;

        public TerrainObjectPlacer objectPlacer;
    }

    public TerrainLayerObjectsProps[] layers;

    public void OnValidate()
    {
        //Tool.OnValidate();
    }

    public void SetIllegalZones(List<Tuple<Vector3, float>> illegals)
    {
        illegalZones = illegals;
    }

    public void AddPrototypes()
    {
        int index = objects.Count;
        for (int i = 0; i < layers.Length; i++)
        {
            layers[i].indexes = new int[layers[i].objects.Length];
            for (int j = 0; j < layers[i].objects.Length; j++)
            {
                var tree = new TreePrototype();
                tree.prefab = layers[i].objects[j];
                tree.bendFactor = 0.2f;
                tree.navMeshLod = 0;
                objects.Add(tree);
                layers[i].indexes[j] = index;
                index++;
            }
        }
    }

    public void ResetLayers()
    {
        objects.Clear();
        layersSet = false;
    }

    public void SetLayers()
    {   
        if (layersSet)
        {
            return;
        }
        layersSet = true;
        Tool._TerrainData.treePrototypes = objects.ToArray();
        Tool._TerrainData.RefreshPrototypes();
        Tool._TerrainData.SetTreeInstances(new TreeInstance[0], true);
    }

    public override void Rebuild()
    {
        SetLayers();

        if (disabled)
        {
            return;
        }

        var width = Tool._TerrainData.alphamapWidth;
        var height = Tool._TerrainData.alphamapHeight;
        splatMap = Tool._TerrainData.GetAlphamaps(0, 0, width, height);

        for (int layer = 0; layer < layers.Length; layer++)
        {
            if (layers[layer].disabled)
            {
                continue;
            }
            bool[,] placed = new bool[width, height];
            int splatMapLayer = layers[layer].splatMapLayer;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float p = UnityEngine.Random.Range(0.0f, 1.0f) * (splatMap[x, y, splatMapLayer]);

                    if (/*p > splatMap[x, y, splatMapLayer] || */
                        splatMap[x, y, splatMapLayer] < layers[layer].threshold ||
                        p < layers[layer].noSpawnChance)
                    {
                        continue;
                    }

                    int l = layers[layer].minDistance;
                    int minX = Math.Max(0, x - l);
                    int minY = Math.Max(0, y - l);
                    int maxX = Math.Min(width, x + l);
                    int maxY = Math.Min(height, y + l);
                    bool canSpawn = true;
                    for (int i = minX; i < maxX && canSpawn; i++)
                    {
                        for (int j = minY; j < maxY && canSpawn; j++)
                        {
                            if (placed[i, j])
                            {
                                canSpawn = false;
                            }
                        }
                    }

                    if (!canSpawn)
                    {
                        continue;
                    }

                    if (Spawn(y, x, layer))
                    {
                        placed[x, y] = true;
                    }
                }
            }

                    /*float p = UnityEngine.Random.Range(0.0f, 1.0f);
                    float sum = 0;
                    int layerTypeToSpawn = 0;
                    for (; layerTypeToSpawn < layerCount; layerTypeToSpawn++)
                    {
                        sum += splatMap[x, y, layerTypeToSpawn];
                        if (p < sum)
                        {
                            break;
                        }
                    }

                    if (layerTypeToSpawn >= layerCount)
                    {
                        continue;
                    }

                    p = UnityEngine.Random.Range(0.0f, 1.0f);
                    if (p < noSpawnChance)
                    {
                        continue;
                    }
                    Spawn(x, y, layerTypeToSpawn);*/
        }

        Tool._Terrain.Flush();
    }

    private bool Spawn(float x, float y, int layer)
    {
        var pos = GetWorldPosition(x, y);

        foreach (var illegal in illegalZones)
        {
            if (Vector3.Distance(pos, illegal.Item1) < illegal.Item2)
            {
                return false;
            }
        }

        TreeInstance treeTemp = new TreeInstance();

        var width = Tool._TerrainData.alphamapWidth;
        var height = Tool._TerrainData.alphamapHeight;
        treeTemp.position = new Vector3(x / width, 0, y / height);

        treeTemp.prototypeIndex = layers[layer].indexes[UnityEngine.Random.Range(0, layers[layer].indexes.Length)];
        unsafe
        {
            layers[layer].objectPlacer.SetPlacement(&treeTemp);
        }

        //treeTemp.color = Color.white;
        //treeTemp.lightmapColor = Color.white;
        Tool._Terrain.AddTreeInstance(treeTemp);
        return true;
    }

    public override void Apply()
    {
        Rebuild();
        if (createNavMeshAfterPlacing)
        {
            NavMeshBaker baker = new NavMeshBaker();
            baker.Bake();
        }
    }
}
