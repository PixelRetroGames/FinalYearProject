using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Terrain/Layers/RunestoneSpawner")]
[Serializable]
public class RunestoneSpawner : TerrainModLayer
{
    public GameObject runestonePrefab;
    // tuple of min dist and max dist

    [System.Serializable]
    public class RunestoneSpawnProp
    {
        public float minDist;
        public float maxDist;
    }


    [SerializeField]
    public RunestoneSpawnProp[] runestonesSpawnProps;
    
    
    public float runestonesIllegalDistance;

    private List<Vector3> runestonesPositions = new List<Vector3>();

    private Vector3 safeZonePosition;
    private List<GameObject> spawnedObjects = new List<GameObject>();

    public void SetSafeZone(Vector3 safeZonePosition)
    {
        this.safeZonePosition = safeZonePosition;
    }

    public void Clear()
    {
        foreach (var obj in spawnedObjects)
        {
            DestroyImmediate(obj, true);
        }
        spawnedObjects.Clear();
        runestonesPositions.Clear();
    }

    public void ComputeSpawnPositions()
    {
        Clear();

        foreach (var area in runestonesSpawnProps)
        {
            float minDist = area.minDist;
            float maxDist = area.maxDist;
            float angleRad = UnityEngine.Random.Range(0f, 2 * Mathf.PI);
            Vector3 angle = new Vector3(Mathf.Sin(angleRad), 0, Mathf.Cos(angleRad));

            Vector3 pos = Vector3.zero + angle * UnityEngine.Random.Range(minDist, maxDist);
            runestonesPositions.Add(pos);
        }

        GameObject.FindGameObjectWithTag("ProgressionLogic").GetComponent<ProgressionLogic>().SetNumObjectives(runestonesPositions.Count);
    }

    public override void Apply()
    {
       foreach(var pos in runestonesPositions) {
            Spawn(pos);
       }
    }

    private void Spawn(Vector3 pos)
    {
        var obj = Instantiate(runestonePrefab.gameObject);
        obj.transform.position += pos;
        spawnedObjects.Add(obj);
    }

    public List<Tuple<Vector3, float>> GetRunestonesIllegalAreas()
    {
        var illegals = new List<Tuple<Vector3, float>>();
        foreach (var pos in runestonesPositions)
        {
            illegals.Add(new Tuple<Vector3, float>(pos, runestonesIllegalDistance));
        }
        return illegals;
    }

    public override void Rebuild()
    {
    }
}
