using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using static Microsoft.MixedReality.GraphicsTools.MeshInstancer;

public class CoordinateManager : MonoBehaviour
{
    // Start is called before the first frame update
    public bool debug = true;
    public bool createDummyRSSI;
    public bool clearRSSIEntries = false;
    public GameObject cam;
    public GameObject anchorPrefab, relativeAnchorPrefab;
    public static Vector3 multilaterationStartPoint;

    public static string filePath;
    private void Awake()
    {
        filePath = Application.persistentDataPath + "/current_coord.txt";
    }
    void Start()
    {
        multilaterationStartPoint = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void StartMultilateration()
    {
        //Get rid of all sampled points to restart
        ARAnchor[] anchorScripts = FindObjectsOfType<ARAnchor>();
        foreach (ARAnchor anchorScript in anchorScripts)
        {
            Destroy(anchorScript.gameObject);
        }

        //initialize the starting point of multilateration
        multilaterationStartPoint = cam.transform.position;

        if (clearRSSIEntries)
        {
            using (StreamWriter writer = new StreamWriter(filePath, false)) { writer.Write(""); }
        }

        CreateNewSample(true);
    }
    
    public void CreateNewSample(bool relative=false)
    {
        //init
        Vector3 currentPos = cam.transform.position;
        Vector3 relativePos = multilaterationStartPoint - currentPos;
        Debug.Log("Current Position: " + currentPos);
        Debug.Log("Reference Point: " + multilaterationStartPoint);
        Debug.Log("Relative Position: " + relativePos);
        

        try
        {
            //coordinate file processing
            string fileContents = File.ReadAllText(filePath);
            string[] entries = fileContents.Split('$');
            string rssi = entries[entries.Length - 1];
            if (createDummyRSSI)
            {
                rssi = Random.Range(-45, -70).ToString();
            }
            string newEntry = "\n{" + relativePos + ";" + rssi.Trim() + "}$\n";

            string final = "";
            for (int i = 0; i < entries.Length - 1; i++)
            {
                final += entries[i] + "$";
            }
            final += newEntry;

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(final);
            }
        }
        catch (IOException e)
        {
            Debug.LogError("Error reading or writing the file: " + e.Message);
        }

        Multilateration m = FindObjectOfType<Multilateration>();
        m.DisplayCandidatePoints(multilaterationStartPoint);
        CreateAnchor(relative);
    }

    //Creates an anchor object in the worldspace
    public void CreateAnchor(bool relative=false)
    {
        Debug.Log("Creating Anchor...");
        GameObject anchor;
        if (relative)
        {
            anchor = Instantiate(relativeAnchorPrefab, cam.transform.position, Quaternion.identity);
        }
        else
        {
            anchor = Instantiate(anchorPrefab, cam.transform.position, Quaternion.identity);
        }
        if (anchor.GetComponent<ARAnchor>() == null)
        {
            anchor.AddComponent<ARAnchor>();
        }
    }
}
