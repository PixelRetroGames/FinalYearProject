using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Terrain/Layers/Paint By Height")]
[Serializable]
public class TerrainPainterTerrainModLayer : TerrainModLayer
{
    public float[,,] splatMap = null;

    private Vector3 safeZonePosition;
    private float safeZoneRadius;
    private float safeZoneHardness;

    private float borderZoneOffsetRadius;
    private float borderZoneRadius;
    private float borderZoneHardness;

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

    public void SetSafeZone(Vector3 safeZonePosition, float safeZoneRadius, float safeZoneHardness)
    {
        this.safeZonePosition = safeZonePosition;
        this.safeZoneRadius = safeZoneRadius;
        this.safeZoneHardness = safeZoneHardness;
    }

    public void SetBorderZone(float borderZoneOffsetRadius, float borderZoneRadius, float borderZoneHardness)
    {
        this.borderZoneOffsetRadius= borderZoneOffsetRadius;
        this.borderZoneRadius= borderZoneRadius;
        this.borderZoneHardness= borderZoneHardness;
    }

    private void SetLayers()
    {
        TerrainLayer[] terrainLayers = new TerrainLayer[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            terrainLayers[i] = layers[i].terrainLayer;
        }

        #if UNITY_EDITOR
        Tool._TerrainData.SetTerrainLayersRegisterUndo(terrainLayers, "updated layers");
        #endif
    }

    public override void Rebuild()
    {
        SetLayers();

        var width = Tool._TerrainData.alphamapWidth;
        var height = Tool._TerrainData.alphamapHeight;
        var layerCount = Tool._TerrainData.alphamapLayers;
        splatMap = new float[width, height, layerCount];

        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);

        float seed = UnityEngine.Random.Range(0.0f, 10000.0f);
        Debug.Log("seed is " + seed.ToString());

        bool usedTopology = false;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float perlin = Mathf.PerlinNoise(seed + 1.0f * x * scale, seed + 1.0f * y * scale);

                Vector3 pos = GetWorldPosition((float) x, (float) y);
                float distanceToSafe = Vector3.Distance(safeZonePosition, pos);
                float distanceToCenter = Vector3.Distance(Vector3.zero, pos);

                if (distanceToSafe < safeZoneRadius)
                {
                    perlin = Mathf.Max(0f, perlin - (safeZoneRadius / distanceToSafe) * safeZoneHardness);
                } else if (distanceToCenter > borderZoneOffsetRadius)
                {
                    var delta = distanceToCenter - borderZoneOffsetRadius;
                    var borderDelta = borderZoneRadius - borderZoneOffsetRadius;
                    perlin = Mathf.Min(1f, perlin + (delta / borderDelta) * borderZoneHardness);
                }

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