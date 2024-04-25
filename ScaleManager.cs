using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleManager : MonoBehaviour
{
    public static ScaleManager instance;

    public HandMapManager mapManager;
    public GetAssetData assetData;

    public bool doit = false;

    public int mapID;
    [System.NonSerialized]
    public Texture2D map;
    [System.NonSerialized]
    public float pxperm = 0;
    [System.NonSerialized]
    public float CompassAngle;
    [System.NonSerialized]
    public Mesh nav;
    [System.NonSerialized]
    public float mperpx = 0;
    [System.NonSerialized]
    public int xRes = 0;
    [System.NonSerialized]
    public int yRes = 0;

    private void Awake()
    {
        //LoadValues(mapID);

        instance = this;

        mapManager.gameObject.SetActive(true);
    }

    public void LoadMap(int id)
    {
        scaledata data = GetComponent<ScaleData>().GetStructById(id);

        if(data.mapID == 0)
        {
            print("could not find data for " + id);
            return;
        }

        mapID = data.mapID;
        map = data.map;
        pxperm = data.pxperm;
        CompassAngle = data.compassAngle;
        nav = data.nav;

        xRes = map.width;
        yRes = map.height;
        mperpx = 1 / pxperm;

        mapManager.LoadMap();
        assetData.Post();
        assetData.gameObject.GetComponent<GetPlaceData>().Post();
    }

    public void LoadMapName(string name)
    {
        scaledata data = GetComponent<ScaleData>().GetStructByName(name);
        LoadMap(data.mapID);
    }

    private void Update()
    {
        if (doit)
        {
            LoadMap(mapID);
            doit = false;
        }
    }

}
