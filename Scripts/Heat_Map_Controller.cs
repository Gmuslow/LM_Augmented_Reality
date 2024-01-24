using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Heat_Map_Controller : MonoBehaviour
{
    public GameObject[] sensors;
    public int resolution = 3;
    private float[,,] heatMap;
    private GameObject[,,] innerCubes;
    private float sidelength = 1;
    public GameObject innerCubePrefab;
    Mesh mesh;
    Vector3[] vertices;
    // Start is called before the first frame update
    void Start()
    {
        
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        Vector3 tmp = vertices[0] - vertices[1];
        sidelength = tmp.magnitude;
        heatMap = new float[resolution, resolution, resolution];
        innerCubes = new GameObject[resolution, resolution, resolution];

        innerCubePrefab.transform.localScale = new Vector3(innerCubePrefab.transform.localScale.x / resolution, innerCubePrefab.transform.localScale.y / resolution, innerCubePrefab.transform.localScale.z / resolution);

        for (int i = 0; i < sensors.Length; i++) //get sensor positions
        {
            Vector3 pos = sensors[i].transform.position;
            
            Debug.Log(vertices[0]);
            
            Debug.Log(pos);
            Vector3 indices = convertPosToIndex(pos, vertices[0], sidelength, sidelength, sidelength, resolution);
            Debug.Log(indices);
        }
        DisplayHeatMapCube(new Vector3(-0.5f, -0.5f, -0.5f));

        Vector3[] sensorPos = { new Vector3(1, 1, 1), new Vector3(0, 0, 2) };
        float[] values = { 2f, 5f };


        for (int j = 0; j < 4; j++)
        {
            for (int i = 0; i < 15; i++)
            {

                Test_IDW(5 * j, (int)Math.Pow(2, i));
            }
        }
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Vector3 convertPosToIndex(Vector3 pos, Vector3 startVertex, float maxLength, float maxWidth, float maxHeight, float resolution)
    {
        Vector3 Vsp = pos - startVertex;

        int x = (int)((Vsp.x / maxLength) * resolution);
        int y = (int)((Vsp.y / maxWidth) * resolution);
        int z = (int)((Vsp.z / maxHeight) * resolution);

        Vector3 indices = new Vector3(x, y, z);
        return indices;
    }

    void DisplayHeatMapCube(Vector3 startVertex)
    {
        float length = (float)(1.0 / resolution);
        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                for (int k = 0; k < resolution; k++)
                { 
                    //only create a cube if it is on the surface
                    if (i == 0 || i == resolution - 1 || j == 0 || j == resolution - 1 || k == 0 || k == resolution - 1) {
                        Vector3 offset = new Vector3(length * (0.5f + i), length * (0.5f + j), length * (0.5f + k));

                        GameObject innerCube = Instantiate(innerCubePrefab, startVertex + offset, Quaternion.identity);
                        Renderer r = innerCube.GetComponent<Renderer>();
                        r.material.SetColor("_Color", GetColor(i * 10 + j*10 + k*10));
                    }
                }
            }
        }
    }

    Color GetColor(float value)
    {
        return new Color((value % 255) / 255, 0f, 0f, 1f);
    }

    float IDW(Vector3 cellPos, Vector3[] sensorPositions, float[] sensorValues, float bounds = 100f, float power = 2)
    {
        float sumTop = 0;
        float sumBottom = 0;
        for (int i = 0; i < sensorPositions.Length; i++)
        {
            Vector3 diff = sensorPositions[i] - cellPos;
            if (diff.magnitude < bounds) //if the sensor is within the bounds
            {
                sumTop += (float)(sensorValues[i] / (float)(Math.Pow(diff.magnitude, power)));
                sumBottom += (float)(1f / (Math.Pow(diff.magnitude, power)));

            }
        }
        if (sumBottom == 0)
            return 0;
        else
            return sumTop / sumBottom;
    }

    void Test_IDW(int numSensors, int numVertices)
    {
        System.Random rnd = new System.Random();
        
        Vector3[] sensorPos = new Vector3[numSensors];
        float[] sensorVals = new float[numSensors];
        for (int i = 0; i < numSensors; i++)
        {
            Vector3 pos = new Vector3((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());

            sensorPos[i] = pos;
            sensorVals[i] = (float)rnd.NextDouble();
        }

        var watch = System.Diagnostics.Stopwatch.StartNew();
        
        for (int j = 0; j < numVertices; j++)
        {
            Vector3 vertexPos = new Vector3((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());
            float val = IDW(vertexPos, sensorPos, sensorVals);
        }

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        Debug.Log("Elapsed time for " + numSensors + " sensors and " + numVertices + " vertices:\t" + elapsedMs);
    }
}
