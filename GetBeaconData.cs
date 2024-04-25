using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using MixedReality.Toolkit.UX;
using MixedReality.Toolkit.UX.Experimental;

public class GetBeaconData : MonoBehaviour
{


    public ListMenuTheme listMenu; // Reference to the List Menu component
    public GameObject dataPointPrefab;
    public Transform parentTransform;
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

        StartCoroutine(FetchBeaconData("https://odata-api-lb-prod.us.lmco.com/api/v2/assets/filter"));

        //StartCoroutine(GetRequest("https://odata-api-lb-prod.us.lmco.com/api/maps/172/images/wa"));
    }


     IEnumerator FetchBeaconData(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            webRequest.SetRequestHeader("Authorization", "Bearer YOUR_TOKEN_HERE");
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error fetching beacon data: " + webRequest.error);
                yield break;
            }

            DataAssets dataAssets = JsonUtility.FromJson<DataAssets>(webRequest.downloadHandler.text);

            List<string> beaconNames = new List<string>(); // List to store beacon names

            foreach (DataAsset asset in dataAssets.value)
            {
                foreach (Beacon beacon in asset.beacons)
                {
                    if (beacon.isOnline)
                    {
                        beaconNames.Add(beacon.name);
                    }
                }
            }

            // Assign the beacon names to the List Menu options
            string[] beaconNamesArr = beaconNames.ToArray();
            foreach (string name in beaconNamesArr)
            {
                Debug.Log("Beacon Name: " + name);
            }
        }
    }

   

}
