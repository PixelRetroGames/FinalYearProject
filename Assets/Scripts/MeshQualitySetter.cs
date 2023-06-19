using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeshQualitySetter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int qualityLevel = GetQualityLevel();
        SetQualityLevel(qualityLevel);
    }

    private void SetQualityLevel(int qualityLevel)
    {
        const string lodFormat = "LOD";
        string lod = lodFormat + qualityLevel.ToString();
        foreach (Transform child in transform)
        {
            string level = child.name.Substring(child.name.Length - lodFormat.Length - 1);
            if (!level.Equals(lod) && level.Substring(0, lodFormat.Length).Equals(lodFormat))
            {
                Object.Destroy(child.gameObject);
            }
        }
        Destroy(gameObject.GetComponent<LODGroup>());
    }

    private int GetQualityLevel()
    {
        int qualityLevel = 3;
        GameObject settings = GameObject.FindGameObjectWithTag("Settings");
        if (settings == null)
        {
            Debug.Log("no settings");
        }
        else
        {
            qualityLevel = settings.GetComponent<Settings>().GetQualityLevel();
        }
        return qualityLevel;
    }
}
