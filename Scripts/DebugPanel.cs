using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using System;

public class DebugPanel : MonoBehaviour
{
    // Start is called before the first frame update
    public float updateTime = 0.5f;
    public static string bluetoothStatus = "OFF";
    public static string foundDevices = "";
    public static string scanStatus = "";
    public bool includeStackTrace = false;
    private TextMeshProUGUI tMeshPro, rssiTMesh;
    float counter;
    public string debugString;
    public GameObject rssiText;
    private ConnectBluetooth cb;
    public int targetAddressIndex = 0;
    List<byte[]> testTargets = new List<byte[]>();
    public bool showNewSamples;

    CoordinateManager cm;
    void Start()
    {
        tMeshPro = GetComponent<TextMeshProUGUI>();
        rssiTMesh = rssiText.GetComponent<TextMeshProUGUI>();
        cb = FindObjectOfType<ConnectBluetooth>();
        cm = FindObjectOfType<CoordinateManager>();
        counter = updateTime;

        Application.logMessageReceived += Application_logMessageReceived;
        debugString = "--DEBUG PANEL--\n";
        byte[] target1 = { 0xAC, 0x23, 0x3F, 0xAB, 0x46, 0x06 };
        byte[] target2 = { 0xAC, 0x23, 0x3F, 0xAB, 0x45, 0xFE };
        testTargets.Add(target1);
        testTargets.Add(target2);
    }

    // Update is called once per frame
    
    void Update()
    {
        
        counter -= Time.deltaTime;
        if (counter < 0)
        {
            if (ConnectBluetooth.changed)
            {
                
                rssiTMesh.text = "RSSI: " + ConnectBluetooth.HexStringToSignedByte(ConnectBluetooth.rssiValue);

                //call multilateration
                if (CoordinateManager.sampling)
                {
                    Debug.Log("Got new sample: " + rssiTMesh.text);
                    cm.CreateNewSample();
                }
                ConnectBluetooth.changed = false;
            }
            counter = updateTime;
        }
    }


    //redirects debug output to debug panel
    private void Application_logMessageReceived(string message, string stackTrace, LogType type)
    {
        if (type == LogType.Log || type == LogType.Error || type == LogType.Exception)
        {
            string msg = message;
            if ((type == LogType.Exception || type == LogType.Error))
            {
                if (includeStackTrace)
                    msg += $"\n*** stack trace ***\n{stackTrace}\n*** end stack trace ***";
            }
            tMeshPro.text += msg + "\n";
        }
    }


    public void CycleTargets()
    {
        cb.WriteToTargetAddress(testTargets[targetAddressIndex]);
        targetAddressIndex++;
        if (targetAddressIndex >= testTargets.Count)
        {
            targetAddressIndex = 0;
        }
    }

    public void ClearDebugText()
    {
        tMeshPro.text = "";
    }
}
