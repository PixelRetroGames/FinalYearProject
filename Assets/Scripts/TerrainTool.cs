using System;
using System.Collections.Generic;
using UnityEditor.TextCore.Text;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainTool : MonoBehaviour
{
    public Vector3 safeZonePosition;
    public float safeZoneRadius;
    public float safeZoneHardness;

    public float borderZoneOffsetRadius;
    public float borderZoneRadius;
    public float borderZoneHardness;

    public Terrain _Terrain;
    public TerrainData _TerrainData;

    public float[,] Mesh
    {
        get
        {
            return _Mesh;
        }
        set
        {
            _Mesh = value;
        }
    }

    [System.Serializable]
    public class LayerFields
    {
        public TerrainModLayer Layer;
        public bool Rebuild = false;
    }

    public LayerFields[] Layers;
    public bool RebuildAll = false;

    [HideInInspector]
    public int HeightRes;
    private float[,] _Mesh;
    private Vector3 size;
    private Vector3 scale;
    private bool refresh = false;

    public void OnEnable()
    {
        RebuildAll = false;
        foreach (var layer in Layers)
        {
            layer.Layer.Tool = this;
        }
    }

    public T GetLayer<T>() where T : TerrainModLayer
    {
        foreach (var layer in Layers)
        {
            if (layer.Layer is T) return (T)layer.Layer;
        }

        return null;
    }

    public void Update()
    {
        if (Application.isPlaying) return;

        if (!refresh)
        {
            return;
        }
        refresh = false;
        RefreshTerrain();
    }

    public void OnValidate()
    {
        if (RebuildAll == true)
        {
            refresh = true;
        }
    }

    public void RefreshTerrain()
    {
        size = _TerrainData.size;
        HeightRes = _TerrainData.heightmapResolution;
        scale = size / HeightRes;
        Mesh = new float[HeightRes, HeightRes];

        if (Layers != null)
        {
            var illegalZones = new List<Tuple<Vector3, float>>();
            if (RebuildAll)
            {
                transform.SetPositionAndRotation(new Vector3(-size.x / 2, 0, -size.z / 2), Quaternion.identity);
                bool reset = false;

                foreach (var layer in Layers)
                {
                    layer.Layer.Tool = this;
                    if (layer.Layer.GetType() == typeof(RunestoneSpawner))
                    {
                        var spawner = (RunestoneSpawner)layer.Layer;
                        spawner.SetSafeZone(safeZonePosition);
                        spawner.ComputeSpawnPositions();

                        illegalZones.AddRange(spawner.GetRunestonesIllegalAreas());
                    }

                    if (layer.Layer.GetType() == typeof(TerrainPainterTerrainModLayer))
                    {
                        var painter = ((TerrainPainterTerrainModLayer)layer.Layer);
                        painter.SetSafeZone(safeZonePosition, safeZoneRadius, safeZoneHardness);
                        painter.SetBorderZone(borderZoneOffsetRadius, borderZoneRadius, borderZoneHardness);
                        illegalZones.Add(new Tuple<Vector3, float>(safeZonePosition, safeZoneRadius));
                    }

                    if (layer.Layer.GetType() == typeof(ObjectPlacerTerrainModLayer))
                    {
                        if (!reset)
                        {
                            ((ObjectPlacerTerrainModLayer)layer.Layer).ResetLayers();
                            reset = true;
                        }
                        ((ObjectPlacerTerrainModLayer)layer.Layer).AddPrototypes();
                    }
                }
            }

            foreach (var layer in Layers)
            {       
                if (layer.Rebuild || RebuildAll)
                {
                    if (layer.Layer.GetType() == typeof(ObjectPlacerTerrainModLayer))
                    {
                        ((ObjectPlacerTerrainModLayer)layer.Layer).SetIllegalZones(illegalZones);
                    }
                    layer.Layer.Rebuild();
                    layer.Layer.Apply();
                    layer.Rebuild = false;
                }
            }
            RebuildAll = false;
        }

        _TerrainData.SetHeights(0, 0, _Mesh);
    }

    public Vector3 WorldPositionFromHeightMapIndex(int x, int y)
    {
        return
            new Vector3(
                y * scale.z,
                size.y * _Mesh[x, y],
                x * scale.x)
            + _Terrain.transform.position;
    }

    public float GetTerrainPercentForWorldHeight(float y)
    {
        if (y < _Terrain.transform.position.y) return 0;
        if (y > _Terrain.transform.position.y + size.y) return 1;
        y += 0 - _Terrain.transform.position.y;
        return y / size.y;

    }
}