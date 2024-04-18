using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Globalization;

public class InformationDisplay : MonoBehaviour
{
    private Transform parentImage;
    public GameObject row;
    private TextInfo textInfo;
    public static InformationDisplay instance;

    private void Start()
    {
        parentImage = gameObject.transform.GetChild(0);
        instance = this;
        textInfo = new CultureInfo("en-US", false).TextInfo;
    }

    /// <summary>
    /// Removes info from menu
    /// </summary>
    public void ChangeSelectMenu()
    {
        if (parentImage.childCount != 0)
        {
            foreach (Transform child in parentImage)
            {
                Destroy(child.gameObject);
            }
        }
    }

    /// <summary>
    /// Puts all identifieng data about the selected point into the left hand information ui panel
    /// </summary>
    /// <param name="datapointInfo"></param>
    /// <param name="beacon"></param>
    /// <param name="places"></param>
    public void ChangeSelectMenu(Dictionary<string,string> datapointInfo,GetAssetData.Beacon beacon,List<string> places) 
    {
        if (parentImage.childCount != 0) 
        {
            foreach (Transform child in parentImage) 
            {
                Destroy(child.gameObject);
            }
        }
        foreach (KeyValuePair<string, string> identifier in datapointInfo) 
        {
            if(identifier.Key == "beacon")
            {
                continue;
            }
            AddRowToMenu(identifier.Key,identifier.Value);
        }
        if (beacon != null)
        {

            AddRowToMenu("Beacon Name", beacon.name);
            if (beacon.readings != null)
            {
                foreach (GetAssetData.Reading reading in beacon.readings)
                {
                    if (reading.ability != "Health Check")
                    {
                        AddRowToMenu(reading.ability, reading.value);
                    }
                }
            }
            else
            {
                print("no beacon.readings");
            }
        }
        if (places.Count != 0) 
        {
            string placestr = "";
            for (int i = 0; i < places.Count; i++)
            {
                placestr += places[i];
                if (i != places.Count - 1)
                {
                    placestr += ", ";
                }
            }
            AddRowToMenu("Places", placestr);
            /*AddRowToMenu("Places", places[0]);
            for (int i = 1; i < places.Count; i++) 
            {
                AddRowToMenu("", places[i]);
            }*/
        }
    }


    /// <summary>
    /// Adds row to ui bases on what kind of data is being displayed
    /// </summary>
    /// <param name="catergory"></param>
    /// <param name="value"></param>
    private void AddRowToMenu(string catergory, string value) 
    {

        GameObject newRow = Instantiate(row, parentImage);

        if (catergory != "")
        {
            string catName = textInfo.ToTitleCase(catergory) + ":";

            switch (catName)
            {
                case "Battery:":
                    catName = "<sprite name=\"mi_batt\">";
                    value += "%";
                    break;

                case "Voltage:":
                    catName = "<sprite name=\"mi_power\">";
                    float volts = float.Parse(value);
                    if(volts > 100)
                    {
                        volts /= 1000;
                        value = volts.ToString("F2");
                    }
                    value += "V";
                    break;

                case "Temperature:":
                    catName = "<sprite name=\"mi_temp\">";
                    float cTemp = float.Parse(value);
                    float fTemp = cTemp * (9f / 5f) + 32;
                    value = Mathf.RoundToInt(fTemp).ToString()  + "°F, " + Mathf.RoundToInt(cTemp).ToString() + "°C";
                    break;

                case "Places:":
                    catName = "<sprite name=\"mi_pin\">";
                    break;

                case "Hours:":
                    value = Mathf.RoundToInt(float.Parse(value)).ToString();
                    break;
            }

            newRow.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = catName;
        }
        newRow.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = value;
    }

}
