using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;

public class DebugPanel : MonoBehaviour
{
    // Start is called before the first frame update
    public float updateTime = 0.5f;
    public static string bluetoothStatus = "OFF";
    public static string foundDevices = "";
    public static string scanStatus = "";
    private TextMeshProUGUI tMeshPro;
    float counter;
    void Start()
    {
        tMeshPro = GetComponent<TextMeshProUGUI>();
        counter = updateTime;
    }

    // Update is called once per frame
    
    void Update()
    {
        string debugString = "--DEBUG PANEL--\n";
        counter -= Time.deltaTime;
        if (counter < 0)
        {
            string coordinateFile = CoordinateManager.filePath;
            
            using (StreamReader s = new StreamReader(coordinateFile))
            {
                string data = s.ReadToEnd();
                debugString += "current_coord file:\n";
                debugString += data;
               
            }
            debugString += "\nMicrocontroller Connectivity Status:\n" + bluetoothStatus;
            debugString += "\nScan Status:\t" + scanStatus + "\n";
            debugString += "\nFound Devices:\n" + foundDevices;
            tMeshPro.text = debugString;
            counter = updateTime;
        }
    }
}
