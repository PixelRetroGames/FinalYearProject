using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Terrain/Layers/Paint By Height")]
[Serializable]
public class TerrainPainterTerrainModLayer : TerrainModLayer
{
    public float[,,] splatMap = null;

    [Range(0.0f, 1.0f)]
    public float scale;

    [System.Serializable]
    public class TerrainLayerProps
    {
        public TerrainLayer terrainLayer;
        [Range(0.0f, 100.0f)]
        public float percentage;
    }

    public TerrainLayerProps[] layers;

    public void OnValidate()
    {
        Tool.OnValidate();
    }

    private void SetLayers()
    {
        TerrainLayer[] terrainLayers = new TerrainLayer[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            terrainLayers[i] = layers[i].terrainLayer;
        }
        Tool._TerrainData.SetTerrainLayersRegisterUndo(terrainLayers, "updated layers");
    }

    public override void Rebuild()
    {
        SetLayers();

        var widhth = Tool._TerrainData.alphamapWidth;
        var height = Tool._TerrainData.alphamapHeight;
        var layerCount = Tool._TerrainData.alphamapLayers;
        splatMap = new float[widhth, height, layerCount];

        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);

        float seed = UnityEngine.Random.Range(0.0f, 10000.0f);
        Debug.Log("seed is " + seed.ToString());

        bool usedTopology = false;
        for (int x = 0; x < widhth; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float perlin = Mathf.PerlinNoise(seed + 1.0f * x * scale, seed + 1.0f * y * scale);

                usedTopology = false;
                float sum = 0;
                for (int i = 0; i < layers.Length; i++)
                {
                    float normPercentage = layers[i].percentage / 100;
                    if (perlin < sum + normPercentage)
                    {
                        usedTopology = true;
                        float percent = (perlin - sum) / normPercentage;
                        splatMap[x, y, i] = percent;
                        if (i > 0)
                        {
                            splatMap[x, y, i - 1] = 1 / percent;
                        } else
                        {
                            splatMap[x, y, i] = 1;
                        }
                        
                        break;
                    }

                    sum += normPercentage;
                }

                if (!usedTopology)
                {
                    splatMap[x, y, layers.Length - 1] = 1;
                }
            }
        }
    }

    public override void Apply()
    {
        if (splatMap == null)
        {
            Rebuild();
        }
        Tool._TerrainData.SetAlphamaps(0, 0, splatMap);
        // Tool._Terrain.basemapDistance = 2000;
    }
}