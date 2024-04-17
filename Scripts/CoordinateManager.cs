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
    public bool clearRSSIEntries = false;
    public GameObject cam;
    public GameObject anchorPrefab, relativeAnchorPrefab;
    public static Vector3 multilaterationStartPoint;
    public static bool sampling = false;

    public static string filePath;
    private Multilateration mul;
    private void Awake()
    {
        filePath = Application.persistentDataPath + "/current_coord.txt";
        mul = FindObjectOfType<Multilateration>();
    }
    void Start()
    {
        multilaterationStartPoint = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Clears previous measurements and destroys all previous anchor points
    public void StartMultilateration()
    {
        Debug.Log("Starting Multilateration...");
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

        sampling = true;
        CreateAnchor(true);

        mul.InstantiateCandidates(multilaterationStartPoint);
        //CreateNewSample(true);
    }



    public void StartStopSampling()
    {
        sampling = !sampling;
    }
    

    //Creates a new anchor point relative to the starting point of multilateration
    //Creates a new entry in the coordinate file.
    //Calls multilateration script 
    public void CreateNewSample(bool relative=false, bool dummyRSSI = false)
    {
        //init
        Vector3 currentPos = cam.transform.position;
        Vector3 relativePos = multilaterationStartPoint - currentPos;
        //Debug.Log("Current Position: " + currentPos);
        //Debug.Log("Reference Point: " + multilaterationStartPoint);
        //Debug.Log("Relative Position: " + relativePos);
        

        try
        {
            //coordinate file processing
            string fileContents = File.ReadAllText(filePath);
            string[] entries = fileContents.Split('$');
            string rssi = ConnectBluetooth.HexStringToSignedByte(ConnectBluetooth.rssiValue).ToString();
            //Debug.Log("Received rssi: " + rssi);
            if (dummyRSSI)
            {
                rssi = Random.Range(-45, -70).ToString();
                Debug.Log("Created Dummy RSSI: " + rssi);
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

        
        mul.DisplayCandidatePoints(multilaterationStartPoint);
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

    public void CreateDummySample()
    {
        CreateNewSample(dummyRSSI: true);
    }
}
