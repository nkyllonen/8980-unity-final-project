using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    // terrain attributes
    public int xSize = 20;
    public int zSize = 20;
    public float noisiness = 2f;
    public Gradient gradient;
    private float minHeight = 100;
    private float maxHeight = -100;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
    }

    void CreateShape()
    {
        // grid contains an extra row and extra col of vertices
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        //{
        //    new Vector3(0,0,0),
        //    new Vector3(0,0,1),
        //    new Vector3(1,0,0),
        //    new Vector3(1,0,1)
        //};

        // loop through and generate vertex positions
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                // adjust noise so it's more noticeable
                float height = Mathf.PerlinNoise(x * 0.3f, z * 0.3f) * noisiness;
                vertices[i] = new Vector3(x, height, z);
                i++;

                // determine min and max heights
                if (height < minHeight) minHeight = height;
                if (height > maxHeight) maxHeight = height;
            }
        }

        triangles = new int[xSize * zSize * 6];
        /*triangles = new int[]
        {
            0,xSize+1,1,
            1,xSize+1,xSize+2
        };*/

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

        colors = new Color[vertices.Length];
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
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();
    }

    private void Update()
    {
        // use any new input values and recalculate
        CreateShape();
        UpdateMesh();
    }

    /*private void OnDrawGizmos()
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
