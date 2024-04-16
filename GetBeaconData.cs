using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class GetBeaconData : MonoBehaviour
{

    private float pixelToMeters = .28808f;

    public GameObject dataPoint;
    private DataAssets dataAssets;
    

    [Serializable]
    public class Beacon
    {
        public int id;
        public string name;
        public object location;
        public object gps;
        public DateTime lastCheckin;
        public bool isOnline;
        public string deviceId;
        public List<Reading> readings;
    }
    [Serializable]
    public class Location
    {
        public float x;
        public float y;
        public int z;
        public int m;
    }
    [Serializable]
    public class Place
    {
        public int id;
        public string name;
    }
    [Serializable]
    public class Reading
    {
        public int abilityId;
        public string ability;
        public string value;
        public double min;
        public double max;
        public double mean;
        public double median;
        public int n;
        public string timestamp;
    }
    [Serializable]
    public class DataAssets
    {
        public List<DataAsset> value;
        public int count;
        public string next;
    }
    [Serializable]
    public class DataAsset
    {
        public int id;
        public string name;
        public Location location;
        public object gps;
        public DateTime lastCheckin;
        public bool isOnline;
        public List<Beacon> beacons;
        public List<Place> places;
    }


    void Start()
    {

        StartCoroutine(GetRequest("https://odata-api-lb-prod.us.lmco.com/api/v2/assets/filter"));

        //StartCoroutine(GetRequest("https://odata-api-lb-prod.us.lmco.com/api/maps/172/images/wa"));
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            webRequest.SetRequestHeader("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiJjMzZiOGZhMC1lYmRmLTQxMzQtYWY5Zi0yOWY0YzZhZmY5NzIiLCJzdWIiOiJjaGFuZGxlci5icm93bi10dWZmaWVsZEBsbWNvLmNvbSIsInVuaXF1ZV9uYW1lIjoiMjQxNjMiLCJuYW1laWQiOiIyNDE2MyIsIk5hbWUiOiIyNDE2MyIsImF1dGhfdGltZSI6IjYzODIwMzgxODk4MTUyMzUxNiIsInRva2VuX3VzZSI6ImFjY2VzcyIsImFwaV9hY2Nlc3MiOiJUaGluYWVyIiwiZW1haWxfdmVyaWZpZWQiOiJUcnVlIiwiZW1haWwiOiJjaGFuZGxlci5icm93bi10dWZmaWVsZEBsbWNvLmNvbSIsInNzbyI6InRydWUiLCJyb2xlIjoiUmVhZE9ubHkiLCJDbGFpbXMiOlsiUmVhZE9ubHkiXSwibmJmIjoxNjg0Nzg1MDk4LCJleHAiOjE2ODUzODk4OTgsImlzcyI6InRoaW5hZXIuaW8iLCJhdWQiOiJ0aGluYWVyLmlvIn0.JLzgKdIDnkgUHGCTgyVOBqLkQ5zyATQ-T9nws2XCFnE");
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    print(webRequest.downloadHandler.text);
                    dataAssets = JsonUtility.FromJson<DataAssets>(webRequest.downloadHandler.text);
                    foreach (DataAsset asset in dataAssets.value) 
                    {
                        if (asset.isOnline)
                        {
                            GameObject newPoint = Instantiate(dataPoint, new Vector3(asset.location.x * pixelToMeters, 0, asset.location.y * pixelToMeters), dataPoint.transform.rotation, transform);
                            newPoint.name = asset.name;
                        }
                    }
                    break;
            }
        }
    }


}
