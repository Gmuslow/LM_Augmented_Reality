using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class Datapoint : MonoBehaviour
{
    public bool isOnline = true;

    private static Transform mainCamera;

    public string dataLink = "";
    [System.NonSerialized]
    public GetAssetData.DataAsset data;

    [System.NonSerialized]
    public GetAssetData.Beacon locationBeacon;

    [System.NonSerialized]
    public Dictionary<string, string> identifiers = new Dictionary<string, string>();

    [System.NonSerialized]
    public List<string> places = new List<string>();

    private Vector3 rotation;

    private void Start()
    {
        if (mainCamera == null) 
        {
            mainCamera = Camera.main.transform;
        }
        rotation = Vector3.zero;
    }

    public void SetUpPoint()
    {
        Transform child = transform.GetChild(0);
        if (identifiers.ContainsKey("description"))
        {
            child.GetComponent<TextMeshPro>().text = identifiers["description"];
        }
        else if (identifiers.ContainsKey("asset type"))
        {
            child.GetComponent<TextMeshPro>().text = identifiers["asset type"];
        }
        else 
        {
            child.GetComponent<TextMeshPro>().text = data.name;
        }
    }

    public virtual void Selected() 
    {
        if (identifiers != null)
        {
            InformationDisplay.instance.ChangeSelectMenu(identifiers, locationBeacon,places);
        }
    }

    public virtual void Deselected() 
    {
        InformationDisplay.instance.ChangeSelectMenu();
    }

    public abstract void Hover();

    public abstract void UnHover();
}
