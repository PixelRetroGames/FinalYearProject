using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    public GameObject[] objectTypes;
    public int numObjects;
    public float minSpacing;

    public float maxRotationAngle;
    public float minScaleHorizontal;
    public float maxScaleHorizontal;

    public float minScaleVertical;
    public float maxScaleVertical;

    public Rect mapRect;

    // Start is called before the first frame update
    void Start()
    {
        Vector3[] generatedPositions= new Vector3[numObjects];
        for (int i = 0; i < numObjects; i++)
        {
            Vector3 newPos = Vector3.zero;
            bool collision = true;
            int collisions = 100;
            while (collision && collisions > 0)
            {
                collisions--;
                newPos = new Vector3(Random.Range(mapRect.x, mapRect.x + mapRect.width), 0, Random.Range(mapRect.y, mapRect.y + mapRect.height));
                collision = PositionsCollide(newPos, generatedPositions, i);
            }
            if (collision)
            {
                numObjects = i;
                break;
            }
            generatedPositions[i] = newPos;
        }

        for (int i = 0; i < numObjects; i++)
        {
            int treeType = Random.Range(0, objectTypes.Length);
            GameObject newTree = Instantiate(objectTypes[treeType]);
            newTree.transform.position = generatedPositions[i];
            newTree.transform.rotation = new Quaternion(Random.Range(-maxRotationAngle, maxRotationAngle), Random.Range(0f, 1f), Random.Range(-maxRotationAngle, maxRotationAngle), 1);
            float scale = Random.Range(minScaleHorizontal, maxScaleHorizontal);
            newTree.transform.localScale = new Vector3(scale, Random.Range(minScaleVertical, maxScaleHorizontal), scale);
        }
    }

    private bool PositionsCollide(Vector3 newPos, Vector3[] generatedPositions, int numGen)
    {
        for (int i = 0; i < numGen; i++)
        {
            if (Collides(newPos, generatedPositions[i])) {
                return true;
            }
        }
        return false;
    }

    private bool Collides(Vector3 newPos, Vector3 oldPos)
    {
        return Vector3.Distance(newPos, oldPos) < minSpacing;
    }
}
