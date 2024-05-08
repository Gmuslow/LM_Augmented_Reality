using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TagSelectorButton : MonoBehaviour
{
    // Start is called before the first frame update
    public string tagName;
    public byte[] macAddress = {0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SendMacToMicrocontroller()
    {
        ConnectBluetooth b = FindObjectOfType<ConnectBluetooth>();
        Debug.Log("Clicked: " + tagName);
        b.WriteToTargetAddress(macAddress);
    }

    public void SetName()
    {
        GetComponentInChildren<TextMeshProUGUI>().text = tagName;
    }
}
