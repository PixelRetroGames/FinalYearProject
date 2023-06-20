using UnityEngine;

[ExecuteAlways]
public class TerrainTool : MonoBehaviour
{
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

    public void OnEnable()
    {
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

    public void OnValidate()
    {
        RefreshTerrain();
    }

    public void RefreshTerrain()
    {
        HeightRes = _TerrainData.heightmapResolution;
        size = _TerrainData.size;
        scale = size / HeightRes;
        Mesh = new float[HeightRes, HeightRes];

        if (Layers != null)
        {
            if (RebuildAll)
            {
                bool reset = false;
                foreach (var layer in Layers)
                {
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
                layer.Layer.Tool = this;
                if (layer.Rebuild || RebuildAll)
                {
                    layer.Layer.Rebuild();
                    layer.Layer.Apply();
                    layer.Rebuild = false;
                }
            }
            RebuildAll= false;
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