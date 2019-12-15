using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    //Color32[] colors;

    //public GameObject camera;
    public GameObject player;

    // terrain attributes
    public int xSize = 20;
    public int zSize = 20;
    public float noisiness = 2f;
    //public Gradient gradient;
    //public int spacing = 1;

    private float minHeight = 100;
    private float maxHeight = -100;
    private Vector3 offsetFromPlayer;
    private Vector2 xBounds;
    private Vector2 zBounds;

    // Start is called before the first frame update
    void Start()
    {
        // set the components of this gameobject
        mesh = new Mesh();

        // center the player along the x
        /*offsetFromPlayer = player.transform.position - new Vector3(0.5f * xSize, 0, 0.5f * zSize);
        offsetFromPlayer = new Vector3(offsetFromPlayer.x, 0, offsetFromPlayer.z);
        Debug.Log(offsetFromPlayer);*/

        //this.transform.position = offsetFromPlayer;

        // determine edges of mesh relative to player's position
        Vector3 playerPos = player.transform.position;
        xBounds = new Vector2(playerPos.x - (xSize / 2), playerPos.x + (xSize / 2));
        zBounds = new Vector2(playerPos.z - (zSize / 2), playerPos.z + (zSize / 2));

        CreateShape();
        UpdateMesh();

        // set the meshs after they've been calculated
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    void CreateShape()
    {
        // grid contains an extra row and extra col of vertices
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        // loop through and generate vertex positions
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                // adjust noise so it's more noticeable
                float height = Mathf.PerlinNoise(x * 0.3f, z * 0.3f) * noisiness;

                // space vertices out
                //vertices[i] = new Vector3(x*spacing, height, z*spacing) + offsetFromPlayer;
                //vertices[i] = new Vector3(x * spacing, height, z * spacing);
                //vertices[i] = new Vector3(x, height, z);

                // use iterators to lerp between bounds
                float xx = (float)x / xSize;
                float zz = (float)z / zSize;

                vertices[i] = new Vector3(Mathf.Lerp(xBounds.x, xBounds.y, xx), height, Mathf.Lerp(zBounds.x, zBounds.y, zz));

                i++;

                // determine min and max heights
                if (height < minHeight) minHeight = height;
                if (height > maxHeight) maxHeight = height;
            }
        }

        triangles = new int[xSize * zSize * 6];

        // loop through and assign vertices to triangles
        // keeping clockwise ordering
        int tris = 0, verts = 0;
        for (int z = 0; z < zSize; z++)
        {
            // offset down each z row
            for (int x = 0; x < xSize; x++)
            {
                // bottom left
                triangles[tris + 0] = verts + 0;
                triangles[tris + 1] = verts + xSize + 1;
                triangles[tris + 2] = verts + 1;

                // top right
                triangles[tris + 3] = verts + 1;
                triangles[tris + 4] = verts + xSize + 1;
                triangles[tris + 5] = verts + xSize + 2;

                // keep track of which triangles we are drawing
                tris += 6;

                // using this instead of x since vertices are flattened
                // need count to NOT restart, but continue
                verts++;
            }
            //don't try to connect rows
            verts++;
        }

        /*colors = new Color32[vertices.Length];
        // loop through and select color relative to height
        for (int i = 0, z = 0; z < zSize; z++)
        {
            // offset down each z row
            for (int x = 0; x < xSize; x++)
            {
                float height = vertices[i].y;
                colors[i] = gradient.Evaluate(Mathf.InverseLerp(minHeight, maxHeight, height));

                i++;
            }
        }*/
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        //mesh.colors32 = colors;

        mesh.RecalculateNormals();
    }

    public Vector2 GetSize()
    {
        return new Vector2(xSize, zSize);
    }

    public Vector2 GetXBounds()
    {
        return xBounds;
    }

    public Vector2 GetZBounds()
    {
        return zBounds;
    }

    // PRIVATE FUNCTIONS //

    private void Update()
    {
        // use any new input values and recalculate
        CreateShape();
        UpdateMesh();
    }

   /* private void OnDrawGizmos()
    {
        // only draw if we have vertices
        if (vertices == null)
        {
            return;
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }*/
}
