using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Text;
using System;


public class GetPlaceData : MonoBehaviour
{

    public GameObject areaPrefab;
    public Transform areasParent;
    public Layout layout;
    


    #region RecievedJson
    [Serializable]
    public class Layout
    {
        public int id;
        public string name;
        public string description;
        public string imageUrl;
        public string preSignedImageUrl;
        public float scale;
        public float width;
        public float height;
        public List<string> keywords;
        public List<Place> places;
        public bool favorite;
    }
    [Serializable]
    public class Coordinate
    {
        public double x;
        public double y;
        public string z;
        public string m;
    }
    [Serializable]
    public class Place
    {
        public int id;
        public string name;
        public List<Coordinate> boundary;
        public double area;
        public int status;
        public object lastCheckin;
    }

    #endregion

    public void Post()
    {
        StartCoroutine(PostFilter());
    }

    IEnumerator PostFilter()
    {

        using (UnityWebRequest webRequest = new UnityWebRequest("https://odata-api-lb-prod.us.lmco.com/api/maps/"+ScaleManager.instance.mapID+"/layout", "GET"))
        {
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Authorization", "Bearer " + GetAssetData.token);
            webRequest.SetRequestHeader("Content-Type", "application/json");
            yield return webRequest.SendWebRequest();
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                layout = JsonUtility.FromJson<Layout>(webRequest.downloadHandler.text);
            }

        }

        MakeAreas();
    }

    void MakeAreas()
    {
        if (layout == null) 
        {
            return;
        }
        foreach(Place p in layout.places){

            GameObject pObj = Instantiate(areaPrefab, Vector3.zero, Quaternion.identity, areasParent);

            LineRenderer lr = pObj.GetComponent<LineRenderer>();
            int points = p.boundary.Count;
            lr.positionCount = points;

            for(int i=0; i < points; i++)
            {
                Coordinate cord = p.boundary[i];
                Vector3 point = new Vector3((float)cord.x * ScaleManager.instance.mperpx, -1.5f, (float)cord.y * ScaleManager.instance.mperpx);
                lr.SetPosition(i, point);
            }
            Bounds b = lr.bounds;
            pObj.transform.position = b.center + new Vector3(0,5,0);
            pObj.gameObject.name = p.name;
            pObj.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = p.name;
        }
    }
}