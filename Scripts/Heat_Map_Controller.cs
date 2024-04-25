using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MixedReality.Toolkit.UX;
using TMPro;

public class Heat_Map_Controller : MonoBehaviour
{
    public GameObject[] sensors;
    public float control_value = 100;
    public float control_deviance = 15f;

    Mesh mesh;
    UnityEngine.Vector3[] vertices; // Changed Vector3 to UnityEngine.Vector3
    UnityEngine.Vector3[] sensor_positions; // Changed Vector3 to UnityEngine.Vector3
    float[] sensorVals;

    public float power = 2f; // Default power value

    public TextMeshProUGUI powerText; // Reference to the UI Text component

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        //Debug.Log("Number of Vertices: " + vertices.Length);

        sensor_positions = new UnityEngine.Vector3[sensors.Length]; // Changed Vector3 to UnityEngine.Vector3
        sensorVals = new float[sensors.Length];
        for (int i = 0; i < sensors.Length; i++) //get sensor positions
        {
            UnityEngine.Vector3 pos = sensors[i].transform.position; // Changed Vector3 to UnityEngine.Vector3

            //Debug.Log("Sensor " + i + " Position: " + pos);
            sensor_positions[i] = pos;
            sensorVals[i] = sensors[i].GetComponent<SensorController>().value;
        }
        
        Color[] colors = new Color[vertices.Length];
        for (int i = 0;i < vertices.Length;i++) //loop through all vertices
        {
            //Debug.Log(vertices[i]);
            UnityEngine.Vector3 currentVertex = transform.TransformPoint(vertices[i]); // Changed Vector3 to UnityEngine.Vector3
            //Debug.Log(currentVertex);
            float val = IDW(currentVertex, sensor_positions, sensorVals, power);
            //Debug.Log(val);

            colors[i] = GetColor(val);
        }
        SetVertexColors(colors);

        UpdateHeatMap();

    }

    // Update is called once per frame
    float counter = 0;
    float restartTime = 0.05f;
    void Update()
    {
        UpdateHeatMap();

     
        counter += Time.deltaTime;
        if(counter > restartTime)
        {
            for (int i = 0; i < sensors.Length; i++) //get sensor positions and values
            {
                UnityEngine.Vector3 pos = sensors[i].transform.position; // Changed Vector3 to UnityEngine.Vector3

                //Debug.Log("Sensor " + i + " Position: " + pos);
                sensor_positions[i] = pos;
                sensorVals[i] = sensors[i].GetComponent<SensorController>().value;
            }


            Color[] colors = new Color[vertices.Length];
            for (int i = 0; i < vertices.Length; i++) //loop through all vertices
            {
                //Debug.Log(vertices[i]);
                UnityEngine.Vector3 currentVertex = transform.TransformPoint(vertices[i]); // Changed Vector3 to UnityEngine.Vector3
               // Debug.Log(currentVertex);
                float val = IDW(currentVertex, sensor_positions, sensorVals, power);
               // Debug.Log(val);
                colors[i] = GetColor(val);
            }
            SetVertexColors(colors);
            counter = 0;
        }
        
    }

    public void UpdateHeatMap(){

        Color[] colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
           UnityEngine.Vector3 currentVertex = transform.TransformPoint(vertices[i]); // Changed Vector3 to UnityEngine.Vector3
            float val = IDW(currentVertex, sensor_positions, sensorVals, power);
            colors[i] = GetColor(val);
        }
        SetVertexColors(colors);
    }

    // Function to increase the power value
    public void IncreasePower()
    {
        if (power < 5) // Check if power is less than the max limit
        {
            power += 1; // Increase power by 1
            UpdateHeatMap(); // Update the heat map with the new power value
            UpdatePowerText();
        }
    }

    // Function to decrease the power value
    public void DecreasePower()
    {
        if (power > 0) // Check if power is greater than the min limit
        {
            power -= 1; // Decrease power by 1
            UpdateHeatMap(); // Update the heat map with the new power value
            UpdatePowerText();
        }
    }

     void UpdatePowerText()
    {
        if (powerText != null)
        {
            powerText.text = "Power: " + power.ToString(); // Update the text content with the current power value
        }
        else
        {
            Debug.LogError("Power Text reference is not set.");
        }
    }

    void SetVertexColors(Color[] vertexColors)
    {
        // Ensure there is a MeshRenderer component
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogError("MeshRenderer component not found.");
            return;
        }

        // Assign the colors to the existing mesh
        mesh.colors = vertexColors;
        //Debug.Log("Colors set");
    }

    public Gradient colorGradient;
    Color GetColor(float value)
    {
        //Debug.Log(("blah"));
        float max = control_value + control_deviance;
        float min = control_value - control_deviance;
        float diff = (value - min) / (max - min); //linear interpolation
        //Debug.Log("Color value for given value " + value + " : " + diff);
        if (diff > 1f)
        {
            diff = 1f;
        }
        if (diff < 0f)
        {
            diff = 0f;
        }
        if (float.IsNaN(diff))
        {
            diff = 0f;
        }
        Color c;
        try
        {
            //Debug.Log(diff);
            c = colorGradient.Evaluate(diff);
        }
        catch (Exception e)
        {
            Debug.LogError("Color error. Value: " + diff);
            c = Color.black;
        }
        return c;
    }

    float IDW(UnityEngine.Vector3 vertexPos, UnityEngine.Vector3[] sensorPositions, float[] sensorValues, float bounds = 100f, float power = 2f)
    {
        

        float sumTop = 0;
        float sumBottom = 0;
        for (int i = 0; i < sensorPositions.Length; i++)
        {
            UnityEngine.Vector3 diff = sensorPositions[i] - vertexPos;
            if (diff.magnitude < bounds) //if the sensor is within the bounds
            {
                sumTop += (float)(sensorValues[i] / (float)(Math.Pow(diff.magnitude, power)));
                sumBottom += (float)(1f / (Math.Pow(diff.magnitude, power)));
                //Debug.Log("sumTop: " + sumTop);
                //Debug.Log("sumBottom: " + sumBottom);
            }
        }
        if (sumBottom == 0)
            return 0;
        else
            return (float)sumTop / (float)sumBottom;
    }

    void Test_IDW(int numSensors, int numVertices)
    {
        System.Random rnd = new System.Random();

        UnityEngine.Vector3[] sensorPos = new UnityEngine.Vector3[numSensors]; // Changed Vector3 to UnityEngine.Vector3
        float[] sensorVals = new float[numSensors];
        for (int i = 0; i < numSensors; i++)
        {
            UnityEngine.Vector3 pos = new UnityEngine.Vector3((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()); // Changed Vector3 to UnityEngine.Vector3

            sensorPos[i] = pos;
            sensorVals[i] = (float)rnd.NextDouble();
        }

        var watch = System.Diagnostics.Stopwatch.StartNew();

        for (int j = 0; j < numVertices; j++)
        {
            UnityEngine.Vector3 vertexPos = new UnityEngine.Vector3((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()); // Changed Vector3 to UnityEngine.Vector3
            float val = IDW(vertexPos, sensorPos, sensorVals, power);
        }

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        Debug.Log("Elapsed time for " + numSensors + " sensors and " + numVertices + " vertices:\t" + elapsedMs);
    }

    
}
