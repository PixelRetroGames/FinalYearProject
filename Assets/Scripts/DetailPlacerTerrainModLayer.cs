using Cinemachine;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Terrain/Layers/DetailPlacer")]
[Serializable]
public class DetailPlacerTerrainModLayer : TerrainModLayer
{
    public float[,,] splatMap = null;

    public bool disabled;

    [System.Serializable]
    public class TerrainLayerObjectsProps
    {
        public GameObject[] objects;
        public int splatMapLayer;
        [HideInInspector]
        public int[] indexes;
        public bool disabled;
        [Range(0.0f, 1.0f)]
        public float noSpawnChance;

        [Range(0.0f, 1.0f)]
        public float threshold;

        [Range(0, 500)]
        public int maxDensity;
    }

    public TerrainLayerObjectsProps[] layers;

    public void OnValidate()
    {
        Tool.OnValidate();
    }

    private void CleanLayers()
    {
        var width = Tool._TerrainData.detailWidth;
        var height = Tool._TerrainData.detailHeight;
        Tool._TerrainData.SetDetailResolution(width, Tool._TerrainData.detailResolutionPerPatch);
        for (int layer = 0; layer < layers.Length; layer++)
        {
            Tool._TerrainData.SetDetailLayer(0, 0, layer, new int[width, height]);
        }
    }

    public override void Rebuild()
    {
        CleanLayers();
        if (disabled)
        {
            return;
        }

        var width = Tool._TerrainData.detailWidth;
        var height = Tool._TerrainData.detailHeight;

        int splatW = Tool._TerrainData.alphamapWidth;
        int splatH = Tool._TerrainData.alphamapHeight;

        splatMap = Tool._TerrainData.GetAlphamaps(0, 0, splatW, splatH);

        for (int layer = 0; layer < layers.Length; layer++)
        {
            if (layers[layer].disabled)
            {
                continue;
            }

            var detailMap = new int[width, height];
            int splatMapLayer = layers[layer].splatMapLayer;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float p = UnityEngine.Random.Range(0.0f, 1.0f);
                    int splatX = (int)Math.Floor((1.0f * x) / (1.0f * width) * splatW);
                    int splatY = (int)Math.Floor((1.0f * y) / (1.0f * height) * splatH);

                    try
                    {
                        p = splatMap[splatX, splatY, splatMapLayer];
                    }
                    catch(Exception e)
                    {
                        Debug.Log(splatX.ToString() +  " " + splatY.ToString());
                        Debug.Log(x.ToString() +  " " + y.ToString());
                        Debug.Log(splatW.ToString() +  " " + splatH.ToString());
                        Debug.Log(width.ToString() +  " " + height.ToString());
                    }

                    if (/*p > splatMap[x, y, splatMapLayer] || */
                        splatMap[splatX, splatY, splatMapLayer] < layers[layer].threshold ||
                        p < layers[layer].noSpawnChance)
                    {
                        continue;
                    }

                    detailMap[x, y] = (int)(splatMap[splatX, splatY, splatMapLayer] * layers[layer].maxDensity);
                }
            }
            Tool._TerrainData.SetDetailLayer(0, 0, splatMapLayer, detailMap);
        }

        Tool._Terrain.Flush();
    }

    public override void Apply()
    {
        Rebuild();
    }
}
