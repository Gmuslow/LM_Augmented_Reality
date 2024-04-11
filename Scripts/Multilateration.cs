using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Multilateration : MonoBehaviour
{
    // Start is called before the first frame update
    public float maxDist = 5f;
    public int maxPoints = 11;
    public static bool showRSSISphere = false;
    public GameObject RSSISphere;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public static void toggle()
    {
        showRSSISphere = !showRSSISphere;
        RSSISphere[] spheres = FindObjectsOfType<RSSISphere>();
        foreach (RSSISphere sp in spheres)
        {
            sp.gameObject.SetActive(showRSSISphere);
        }
    }

    public class Candidate
    { 
        public Vector3 Position;
        public float Value;
        public Candidate(float v, Vector3 p)
        {
            Position = p;
            Value = v;
        }
    }


    public List<Candidate> PerformMultilateration()
    {
        string coord_filename = CoordinateManager.filePath;
        List<Vector3> current_coordinates = new List<Vector3>();
        List<int> rssi_values = new List<int>();

        using (StreamReader stream = new StreamReader(coord_filename))
        {
            string[] lines = stream.ReadToEnd().Split('$');
            for (int i = 0; i < lines.Length - 1; i++)
            {
                string entry = lines[i].Replace("{", "").Replace("}", "").Trim();
                int RSSI = int.Parse(entry.Split(";")[1]);
                string coord = entry.Split(";")[0].Replace("(", "").Replace(")", "");
                Debug.Log("coord" + coord);
                float x = float.Parse(coord.Split(",")[0]);
                float y = float.Parse(coord.Split(",")[1]);
                float z = float.Parse(coord.Split(",")[2]);
                current_coordinates.Add(new Vector3(x, y, z));
                rssi_values.Add(RSSI);
                
            }
        }

        List<float> radii = new List<float>();
        foreach(int  rssi in rssi_values)
        {
            radii.Add(RSSIToMeters(rssi));
        }

        List<Vector3> candidatePoints = CreateCandidatePoints();
        List<Candidate> candidates = new List<Candidate>();
        foreach (Vector3 candidatePoint in candidatePoints)
        {
            float candidate_value = 0f;
            float combined_distance_score = 0f;
            for (int i = 0; i < current_coordinates.Count; i++)
            {
                Vector3 vector = new Vector3(candidatePoint.x - current_coordinates[i][0], candidatePoint.y - current_coordinates[i][1], candidatePoint.z - current_coordinates[i][2]);
                float mag = vector.magnitude;
                float distance = Mathf.Abs(mag - radii[i]);
                combined_distance_score += distance * PriorityFunction(rssi_values[i]);
            }
            candidate_value = 1.0f / combined_distance_score;
            candidates.Add(new Candidate(candidate_value, candidatePoint));
        }

        if (showRSSISphere)
        {
            RSSISphere[] rssiSpheres = FindObjectsOfType<RSSISphere>();
            foreach (RSSISphere rSSISphere in rssiSpheres) //destroy all old rssiSpheres
            {
                Destroy(rSSISphere.gameObject);
            }

            for (int i = 0; i < radii.Count; i++)
            {
                Vector3 scale = new Vector3(2f * radii[i], 2f * radii[i], 2f * radii[i]);
                GameObject g = Instantiate(RSSISphere, current_coordinates[i] + CoordinateManager.multilaterationStartPoint, Quaternion.identity);
                g.transform.localScale = scale;
            }
        }
        return candidates;
    }

    public float RSSIToMeters(int rssiValue)
    {
        float N = 4f;
        float measuredPower = -40f;
        float exp = (measuredPower - (float)rssiValue) / (10f * N);
        float radius = Mathf.Pow(10, exp);
        return radius;
    }

    public List<Vector3> CreateCandidatePoints()
    {
        List<Vector3> candidatePoints = new List<Vector3>();
        float inc = maxDist * 2f / maxPoints;
        for (int i = 0; i < maxPoints; i++)
        {
            for (int j = 0; j < maxPoints; j++)
            {
                for (int k = 0; k < maxPoints; k++)
                {
                    candidatePoints.Add(new Vector3(i * inc - maxDist, j * inc - maxDist, k * inc - maxDist));
                }
            }
        }
        return candidatePoints;
    }

    public float PriorityFunction(int rssi)
    {
        return Mathf.Sqrt(Mathf.Abs(rssi) - 40);
    }


    public GameObject candidateObject;
    
    public void DisplayCandidatePoints(Vector3 relativeTo)
    {

        CandidateController[] prevCandidates = FindObjectsOfType<CandidateController>(); //Destroy all old candidates
        foreach (CandidateController candidateController in prevCandidates)
        {
            Destroy(candidateController.gameObject);
        }

        List<Candidate> candidates = PerformMultilateration();
        float max = 0f;
        foreach(Candidate candidate in candidates) //computing max real quick
        {
            if (candidate.Value > max)
            {
                max = candidate.Value;
            }
        }

        foreach (Candidate candidate in candidates)
        {
            float alpha = candidate.Value / max;
            if (alpha < 0.3)
            {
                continue;
            }
            GameObject g = Instantiate(candidateObject, candidate.Position + relativeTo, Quaternion.identity);
            Renderer rend = g.GetComponent<Renderer>();

            // Create a new material instance based on the object's material
            Material material = new Material(rend.material);

            // Set the alpha value of the material's color
            Color color = material.color;
            //new Color(1 - alpha, 1 - alpha, 1 - alpha);
            
            color.a = alpha; //set the alpha to some function of the candidate value (TODO<------------)
            material.color = color;


            // Assign the modified material to the object
            rend.material = material;
            
        }
    }

}
