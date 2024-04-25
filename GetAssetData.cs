using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using TMPro;

public class GetAssetData : MonoBehaviour
{

    public static string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI1ZWIzMTE4My05ZjI5LTQ2MzItODg5OC0xOTE1YzZmYTNkZGQiLCJzdWIiOiJjaGFuZGxlci5icm93bi10dWZmaWVsZEBsbWNvLmNvbSIsInVuaXF1ZV9uYW1lIjoiMjQxNjMiLCJuYW1laWQiOiIyNDE2MyIsIk5hbWUiOiIyNDE2MyIsImF1dGhfdGltZSI6IjYzODMyNDgyMzk3NDY4ODk4NyIsInRva2VuX3VzZSI6ImFjY2VzcyIsImFwaV9hY2Nlc3MiOiJUaGluYWVyIiwiZW1haWxfdmVyaWZpZWQiOiJUcnVlIiwiZW1haWwiOiJjaGFuZGxlci5icm93bi10dWZmaWVsZEBsbWNvLmNvbSIsInNzbyI6InRydWUiLCJyb2xlIjoiUmVhZE9ubHkiLCJDbGFpbXMiOlsiUmVhZE9ubHkiXSwibmJmIjoxNjk2ODg1NTk3LCJleHAiOjE2OTc0OTAzOTcsImlzcyI6InRoaW5hZXIuaW8iLCJhdWQiOiJ0aGluYWVyLmlvIn0.A_QkjPdwVNB-AckXukEQ8r6nAUnYnQ9kQANbNgjMmBE";

    private const string DATACSV = "IFFactoryList.csv";

    private float mperpx;
    public GameObject dataPoint;
    public GameObject deadDataPoint;
    public Transform cullParent;
    public LoadingIconManager loadingIcon;
    public float cullDist;

    private Color hot = new Color(1, 0, 0);
    private Color cold = new Color(0, 0, 1);
    private Color full = new Color(0, 1, 0);
    private Color warn = new Color(1, 0.329f, 0);
    private Color empty = new Color(1, 0, 0);

    private DataAssets dataAssets;

    private bool isloading = false;

    public Dictionary<string, HashSet<Datapoint>> assetPlaces = new Dictionary<string, HashSet<Datapoint>>();

    public Dictionary<string, HashSet<Datapoint>> hasIdentifier = new Dictionary<string, HashSet<Datapoint>>();

    public Dictionary<string, HashSet<string>> terms = new Dictionary<string, HashSet<string>>();

    //private Dictionary<string, string> knownDataLinksCombined = new Dictionary<string, string>();
    private Dictionary<string, string[]> knownDataLinks = new Dictionary<string, string[]>();

    private List<Place> places = new List<Place>();

    public Sprite[] sprites;

    public Dictionary<string, HashSet<string>> manualOptions = new Dictionary<string, HashSet<string>>()
    {
        {"description" ,new HashSet<string>(){"Flat", "Torque", "Caliper", "Club Car", "Filler", "Gantry", "HeatShield"}},
    };

    //Structure of json in filter of post request
    #region SentJson
    [Serializable]
    public class Filter
    {
        public string field;
        public string type;
        public int filter;

        public Filter(string field, string type, int filter)
        {
            this.field = field;
            this.type = type;
            this.filter = filter;
        }
    }
    [Serializable]
    public class Body
    {
        public List<int> applicationIds;
        public int skip;
        public int take;
        public List<Filter> filters;
        public List<Sort> sorts;
        public List<string> fields;
        public string search;
        public string condition;
    }
    [Serializable]
    public class Sort
    {
        public string field;
        public bool ascending;
        public Sort(string field, bool ascending)
        {
            this.field = field;
            this.ascending = ascending;
        }
    }
    #endregion

    //Structure of json recieved from server
    #region RecievedJson
    [Serializable]
    public class Beacon
    {
        public int id;
        public string name;
        public string deviceId;
        public List<Reading> readings;
        public List<TriggerCondition> triggerConditions;
        public DateTime assigned;
        public DateTime lastCheckin;
        public bool isOnline;
        public int beaconTypeId;
        public string beaconType;
        public bool isLocationBeacon;
        public Coordinates coordinates;
        public GpsCoordinates gpsCoordinates;
    }
    [Serializable]
    public class Coordinates
    {
        public float x;
        public float y;
        public int z;
        public int m;
        public string coordinateValue;
    }
    [Serializable]
    public class Delay
    {
        public int ticks;
        public int days;
        public int hours;
        public int milliseconds;
        public int minutes;
        public int seconds;
        public int totalDays;
        public int totalHours;
        public int totalMilliseconds;
        public int totalMinutes;
        public int totalSeconds;
    }
    [Serializable]
    public class Gps
    {
        public int additionalProp1;
        public int additionalProp2;
        public int additionalProp3;
    }
    [Serializable]
    public class GpsCoordinates
    {
        public int x;
        public int y;
        public int z;
        public int m;
        public string coordinateValue;
    }
    [Serializable]
    public class GpsLocation
    {
        public int mapId;
        public string mapName;
        public Coordinates coordinates;
        public List<Place> places;
        public int mapWidth;
        public int mapHeight;
    }
    [Serializable]
    public class Identifier
    {
        public string key;
        public string value;
    }
    [Serializable]
    public class Location
    {
        public int mapId;
        public string mapName;
        public Coordinates coordinates;
        public List<Place> places;
        public int mapWidth;
        public int mapHeight;
    }
    [Serializable]
    public class Place
    {
        public int placeId;
        public string name;
        public int area;
        public string translation;
    }
    [Serializable]
    public class Reading
    {
        public DateTime timestamp;
        public int abilityId;
        public string ability;
        public string value;
        public int min;
        public int max;
        public int mean;
        public int median;
        public int n;
        public string timestampAsString;
    }
    [Serializable]
    public class DataAssets
    {
        public List<DataAsset> items;
        public int count;
        public string next;
    }
    [Serializable]
    public class Scan
    {
        public int id;
        public DateTime created;
        public string notes;
        public int sonarUserId;
        public string value;
        public string name;
        public Gps gps;
    }
    [Serializable]
    public class Trigger
    {
        public int id;
        public string name;
        public int status;
        public int beaconId;
        public string beaconName;
    }
    [Serializable]
    public class TriggerCondition
    {
        public int triggerConditionId;
        public int triggerId;
        public int operationOrder;
        public string operation;
        public Delay delay;
        public DateTime lastMet;
        public string triggerName;
        public string triggerDescription;
        public string triggerStatus;
    }
    [Serializable]
    public class DataAsset
    {
        public int id;
        public string name;
        public DateTime created;
        public int movementThreshold;
        public DateTime lastCheckin;
        public List<Beacon> beacons;
        public List<string> keywords;
        public Location location;
        public GpsLocation gpsLocation;
        public List<Identifier> identifiers;
        public List<Trigger> triggers;
        public List<Scan> scans;
        public int status;
        public bool favorite;
        public int locationBeaconId;
        public string preSignedImageUrl;
    }
    #endregion
    void Start()
    {
        StartCoroutine(PostFilter());
        if (TryGetComponent(out GetPlaceData placeData))
        {
            placeData.Post();
        }



    }

    public void Post()
    {
        if (isloading == false)
        {
            StartCoroutine(PostFilter());
        }
        else
        {
            Debug.Log("Getting points already, skipping");
        }
    }

    /// <summary>
    /// Does post request to thinaer to get information and creates the datapoints based on that data
    /// </summary>
    /// <returns></returns>
    IEnumerator PostFilter()
    {
        //Resets all list that helps organize the points
        places.Clear();
        knownDataLinks.Clear();
        //knownDataLinksCombined.Clear();
        assetPlaces.Clear();
        hasIdentifier.Clear();
        terms.Clear();

        isloading = true;
        loadingIcon.show = true;

        //Makes sure filtering script isn't still activated
        if (FilterData.instance != null)
        {
            FilterData.instance.UnFilter();
        }

        //Destroys all previous datapoints
        SelectDatapoint.OnDeselect();
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in cullParent)
        {
            Destroy(child.gameObject);
        }

        //Creates body for post request
        Body body = new Body();

        body.applicationIds = new List<int>() { 34 };
        body.skip = 0;
        body.take = 2;
        body.filters = new List<Filter>()
        {
            new Filter("mapId","equal",ScaleManager.instance.mapID),
        };
        body.fields = new List<string>();
        body.sorts = new List<Sort>()
        {
            new Sort("name",true)
        };
        body.search = "";
        body.condition = "And";

        string json = JsonUtility.ToJson(body);

        /*knownDataLinksCombined = ReadCSV.ReadDataCSV(DATACSV);

        foreach (KeyValuePair<string, string> item in knownDataLinksCombined) {
            print("KEY: " + item.Key + " VALUE: " + item.Value);
            knownDataLinks.Add(item.Key, item.Value.Split(','));
        }*/

        knownDataLinks = ReadCSV.ReadDataCSVList(DATACSV);

        using (UnityWebRequest webRequest = new UnityWebRequest("https://odata-api-lb-prod.us.lmco.com/api/v2/assets/filter/?applicationId=34", "POST"))
        {
            byte[] jsonData = Encoding.UTF8.GetBytes(json);
            webRequest.uploadHandler = new UploadHandlerRaw(jsonData);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Authorization", "Bearer " + token);
            webRequest.SetRequestHeader("Content-Type", "application/json");
            yield return webRequest.SendWebRequest();

            Transform cam = Camera.main.transform;

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(webRequest.error);
                loadingIcon.show = false;
                isloading = false;
            }
            else
            {
                print(webRequest.downloadHandler.text);

                //Reads in scale of map and all asset data
                mperpx = ScaleManager.instance.mperpx;
                dataAssets = JsonUtility.FromJson<DataAssets>(webRequest.downloadHandler.text);

 

                //Manually adds beacon attribute to points
                hasIdentifier.Add("beacon", new HashSet<Datapoint>());
                terms.Add("beacon", new HashSet<string>());

                int usedDataLinks = 0;

                loadingIcon.show = false;

                foreach (DataAsset asset in dataAssets.items)
                {

                    //This check is in case the asset has multiple assets, the location beacon is used
                    Beacon locationBeacon = asset.beacons[0];
                    bool isOnline = false;
                    foreach (Beacon beacon in asset.beacons)
                    {
                        if (beacon.isLocationBeacon)
                        {
                            locationBeacon = beacon;
                            break;
                        }
                    }

                    if (locationBeacon.isOnline) 
                    {
                        isOnline = true;
                    }

                    //checks if close enough
                    Vector3 pos = new Vector3(asset.location.coordinates.x * mperpx, -1, asset.location.coordinates.y * mperpx);

                    float distToCam = Vector3.Distance(pos, cam.position);
                    bool cull = distToCam > cullDist;

                    //Exception points we have specifiv data for
                    bool exception = false;

                    if (knownDataLinks.ContainsKey(asset.name.TrimStart('0')))
                    {
                        exception = true;
                    }
                    GameObject newPoint;
                    GameObject examplePoint;

                    //Dead points are given a different prefab to differentiate them
                    if (!exception & !isOnline)
                    {
                        examplePoint = deadDataPoint;
                    }
                    else 
                    {
                        examplePoint = dataPoint;
                    }
                    if (cull == false)
                    {
                        newPoint = Instantiate(examplePoint, pos, dataPoint.transform.rotation, transform);
                        yield return null;
                    }
                    else
                    {
                        newPoint = Instantiate(examplePoint, pos, dataPoint.transform.rotation, cullParent);
                    }
  

                    //Assigns beacon to identifier of the new datapoint
                    newPoint.name = asset.name;
                    Datapoint datapoint = newPoint.GetComponent<Datapoint>();
                    datapoint.locationBeacon = locationBeacon;
                    datapoint.data = asset;
                    datapoint.identifiers.Add("beacon", locationBeacon.name);
                    hasIdentifier["beacon"].Add(datapoint);
                    terms["beacon"].Add(locationBeacon.name);

                    if (!exception & !isOnline)
                    {
                        datapoint.isOnline = false;
                    }


                    // Manually creates place identifier and addes points to it
                    if (asset.location.places.Count != 0)
                    {

                        foreach (Place place in asset.location.places)
                        {
                            Identifier identifier = new Identifier();
                            identifier.key = "place";
                            identifier.value = place.name;
                            AddIdentifier(identifier, datapoint);
                            if (assetPlaces.ContainsKey(place.name))
                            {
                                assetPlaces[place.name].Add(datapoint);
                            }
                            else
                            {
                                places.Add(place);
                                assetPlaces[place.name] = new HashSet<Datapoint>() { datapoint };
                            }
                            datapoint.places.Add(place.name);
                        }
                    }

                    //Adds all recieved identifiers to recieved point
                    foreach (Identifier identifier in asset.identifiers)
                    {
                        datapoint.identifiers.Add(identifier.key, identifier.value);
                        if (identifier.key.Trim() != "")
                        {
                            AddIdentifier(identifier, datapoint);
                        }
                    }

                    //If we have extra data on a point we assign it to its identifiers 
                    if (datapoint.identifiers.ContainsKey("sap"))
                    {
                        string sap = datapoint.identifiers["sap"].TrimStart('0');
                        if (knownDataLinks.ContainsKey(sap))
                        {
                            string[] values = knownDataLinks[sap];
                            int valLen = values.Length;

                            string link = values[0];
                            if (link != "")
                            {
                                datapoint.identifiers.Add("Link", link);
                                datapoint.dataLink = link;
                            }

                            if (valLen >= 2)
                            {
                                string iconName = values[1];
                                if (iconName != "" && iconName != null)
                                {
                                    //datapoint.identifiers.Add("Icon", iconName);
                                    SetSprite(newPoint, iconName);
                                }

                                if (valLen >= 3)
                                {
                                    string prefabName = values[2];
                                    if (prefabName != "" && prefabName != null)
                                    {
                                        datapoint.identifiers.Add("Model", values[2]);
                                    }
                                }
                            }

                            usedDataLinks += 1;
                            //print("Data Linked: " + datapoint.identifiers["description"] + " Length: " + valLen + " Val: " + link);
                        }
                    }
                    //Initialies point by assigning its name to its text asset
                    newPoint.GetComponent<Datapoint>().SetUpPoint();
                }

                //Prepares filter menu
                FilterData.instance.PopulateMenu();

                /*
                print("Points: " + transform.childCount);
                print("Culled Points: " + cullParent.childCount);
                print("Data Links Used: " + usedDataLinks);
                */

                isloading = false;
            }
        }
    }
    /// <summary>
    /// Creates/Adds to dictionary of known identifiers for points
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="datapoint"></param>
    private void AddIdentifier(Identifier identifier, Datapoint datapoint)
    {
        if (hasIdentifier.ContainsKey(identifier.key))
        {
            hasIdentifier[identifier.key].Add(datapoint);
        }
        else
        {
            hasIdentifier[identifier.key] = new HashSet<Datapoint>() { datapoint };
            terms[identifier.key] = new HashSet<string>();
        }
        terms[identifier.key].Add(identifier.value.ToLower());
    }

    /// <summary>
    /// Changes sprite of point if we have extra data for it
    /// </summary>
    /// <param name="point"></param>
    /// <param name="icon"></param>
    void SetSprite(GameObject point, string icon)
    {
        SpriteRenderer sr = point.GetComponent<SpriteRenderer>();
        print("setting sprite to: " + icon);
        switch (icon)
        {
            case "model":
                sr.sprite = sprites[0];
                break;

            case "machine":
                sr.sprite = sprites[1];
                break;

            case "cart":
                sr.sprite = sprites[2];
                break;

            case "tag":
                sr.sprite = sprites[3];
                break;
        }
    }

    /// <summary>
    /// Changes all points color based on their temperature
    /// </summary>
    public void RenderHeatmap()
    {
        foreach (Transform child in transform)
        {
            HeatMapCalc(child);
        }
        foreach (Transform child in cullParent)
        {
            HeatMapCalc(child);
        }
    }

    /// <summary>
    /// Calculates what a points color should be based on its temeperature
    /// </summary>
    /// <param name="child"></param>
    private void HeatMapCalc(Transform child)
    {
        Datapoint dp = child.gameObject.GetComponent<Datapoint>();

        float temp = 0;

        //Points that dont have readings (or are at zero) default to white
        if (dp.locationBeacon.readings == null)
        {
            child.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            return;
        }

        //Gets float reading for temperature
        foreach (Reading r in dp.locationBeacon.readings)
        {
            if (r.ability == "Temperature")
            {
                temp = float.Parse(r.value);
            }
        }

        if (temp == 0)
        {
            child.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            return;
        }


        //Lerps colors depending on temperature
        float tempPerc = map(temp, 5, 30, 0, 1);

        Color newColor = Color.Lerp(cold, hot, tempPerc);

        child.gameObject.GetComponent<SpriteRenderer>().color = newColor;
    }

    /// <summary>
    /// Changes all points color based on their battery life amd online status
    /// </summary>
    public void RenderBatteryMap()
    {
        GetComponent<Culling>().showOffline = true;

        //Turns off culling since most of the beacons will be removed anyway that are 100%
        if (GetComponent<Culling>().isCulling == true)
        {
            GetComponent<Culling>().isCulling = false;
            for (int i = cullParent.childCount - 1; i >= 0; i--)
            {
                cullParent.GetChild(i).transform.parent = transform;
            }
        }

        //Calculates color for all datapoints
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            BatteryCalc(transform.GetChild(i).transform);
        }
    }

    /// <summary>
    /// Calculates the proper color of a point based on its battery life
    /// </summary>
    /// <param name="child"></param>
    private void BatteryCalc(Transform child)
    {
        Datapoint dp = child.gameObject.GetComponent<Datapoint>();

        float batt = 0;

        //Defaults points without readings to white
        if (dp.locationBeacon.readings == null)
        {
            child.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.05f);
            child.transform.parent = cullParent;
            return;
        }

        //Gets battery value of point
        bool hasBattery = false;
        foreach (Reading r in dp.locationBeacon.readings)
        {
            if (r.ability == "Battery")
            {
                batt = float.Parse(r.value);
                hasBattery = true;
            }

        }

        //Points over 95 are counted as full and are green
        if (batt > 95)
        {
            child.gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0);
            child.transform.parent = cullParent;
        }
        //Finds gradient color of points that are dying
        else
        {
            float battPerc = map(batt, 20, 100, 0, 1);
            Color newColor = Color.Lerp(empty, full, battPerc);
            child.gameObject.GetComponent<SpriteRenderer>().color = newColor;
        }

        //Defaults points without battery value to white
        if (hasBattery == false)
        {
            child.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        }

        //Changes the opacity based on whether the beacon is online
        if (child.gameObject.GetComponent<Datapoint>().isOnline == true)
        {
            SpriteRenderer rend = child.gameObject.GetComponent<SpriteRenderer>();
            Color c = rend.color;
            c.a = 1;
            rend.color = c;
        }
        else
        {
            SpriteRenderer rend = child.gameObject.GetComponent<SpriteRenderer>();
            Color c = rend.color;
            c.a = 0.05f;
            rend.color = c;
        }

    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

    /// <summary>
    /// Removes heatmap colors and goes back to normal culling
    /// </summary>
    public void ClearHeatmap()
    {
        GetComponent<Culling>().showOffline = false;
        GetComponent<Culling>().isCulling = true;

        Color c = new Color(0, 0.745f, 1);
        Color oc = new Color(0.5f, 0.5f, 0.5f, 0.25f);

        foreach (Transform child in transform)
        {
            if (child.GetComponent<Datapoint>().isOnline == true)
            {
                child.gameObject.GetComponent<SpriteRenderer>().color = c;
            }
            else
            {
                child.gameObject.GetComponent<SpriteRenderer>().color = oc;
            }
        }

        foreach (Transform child in cullParent)
        {
            if (child.GetComponent<Datapoint>().isOnline == true)
            {
                child.gameObject.GetComponent<SpriteRenderer>().color = c;
            }
            else
            {
                child.gameObject.GetComponent<SpriteRenderer>().color = oc;
            }
        }
    }

}
