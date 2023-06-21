using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerrainObjectPlacer
{
    public float minScaleHorizontal;
    public float maxScaleHorizontal;
    public float minScaleVertical;
    public float maxScaleVertical;

    unsafe public void SetPlacement(TreeInstance* obj)
    {
        obj->widthScale = Random.Range(minScaleHorizontal, maxScaleHorizontal);
        obj->heightScale = Random.Range(minScaleVertical, maxScaleHorizontal);
    }
}
