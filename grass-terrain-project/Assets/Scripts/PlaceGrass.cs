using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceGrass : MonoBehaviour
{
    public GameObject grassClump;
    public GameObject MeshGenerator;
    public float grassScale = 1;

    public int numClumps = 2;

    /*    private int xMIN = -5;
        private int xMAX = 5;
        private int zMIN = -5;
        private int zMAX = 5;*/

    private Vector2 xBounds;
    private Vector2 zBounds;

    private MeshGenerator meshScript;
    //private Vector2 meshSize;

    // grass will fall into scene
    private int startingY = 5;

    // Start is called before the first frame update
    void Start()
    {
        meshScript = MeshGenerator.GetComponent<MeshGenerator>();
        //Vector2 meshSize = meshScript.GetSize();

        // set bounds according to mesh bounds
        xBounds = meshScript.GetXBounds();
        zBounds = meshScript.GetZBounds();

        for (int i = 0; i < numClumps; i++)
        {
            PlaceGrassRandom();
        }
    }

    void PlaceGrassRandom()
    {
        float x = Random.Range(xBounds.x, xBounds.y);
        float z = Random.Range(zBounds.x, zBounds.y);

        // apply scale to grass clumps
        Transform t = this.gameObject.transform;
        t.localScale = new Vector3(grassScale, grassScale, grassScale);

        // Instantiate at position (x, startingY, z) and zero rotation and transform t
        Instantiate(grassClump, new Vector3(x, startingY, z), Quaternion.identity, t);
    }
}
