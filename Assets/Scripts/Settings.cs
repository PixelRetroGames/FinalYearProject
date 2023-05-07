using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    [SerializeField] private int qualityLevel;

    public int GetQualityLevel()
    {
        return qualityLevel;
    }
}
