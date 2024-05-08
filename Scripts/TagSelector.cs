using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using UnityEngine;

public class TagSelector : MonoBehaviour
{
    public List<TagObject> objects = new List<TagObject>();
    public GameObject buttonPrefab;

    async void Start()
    {
        // Retrieve device IDs from the API
        List<string> deviceIDs = await GetDeviceIDs();

        // Initialize objects list with device IDs
        for (int i = 0; i < deviceIDs.Count; i++)
        {
            byte[] macBytes = StringToByteArray(deviceIDs[i]);
            objects.Add(new TagObject("tag" + (i + 1), macBytes));
        }

        // Initialize the tag selector UI
        InitializeTagSelector();
    }

    public class TagObject
    {
        public string name;
        public byte[] tagAddress;

        public TagObject(string name, byte[] tagAddress)
        {
            this.name = name;
            this.tagAddress = tagAddress;
        }
    }

    public async System.Threading.Tasks.Task<List<string>> GetDeviceIDs()
    {
        List<string> deviceIDs = new List<string>();

        // Send HTTP request to retrieve device IDs
        using (var client = new HttpClient())
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://odata-api.prod-sql.thinaer.io/authentication/login");
            var content = new StringContent("{ \"userName\": \"sstev47@lsu.edu\",\r\n\"password\": \"Gamess1212$\"}", null, "application/json");

            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var info = await response.Content.ReadAsStringAsync();
            var cK = response.Headers.GetValues("Set-Cookie").FirstOrDefault();
            var aT = "Bearer " + Regex.Match(info, "accessToken\":\"+.*?\"").Value.Substring(13).Trim('\"');

            request = new HttpRequestMessage(HttpMethod.Get, "https://odata-api.prod-sql.thinaer.io/api/beacons");
            request.Headers.Add("Authorization", aT);
            request.Headers.Add("Cookie", cK);

            response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            info = await response.Content.ReadAsStringAsync();

            // Extract device IDs
            var deviceMatches = Regex.Matches(info, "deviceId\":\"(.*?)\"");
            foreach (Match match in deviceMatches)
            {
                deviceIDs.Add(match.Groups[1].Value);
            }
        }

        return deviceIDs;
    }

    public void InitializeTagSelector()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            GameObject g = Instantiate(buttonPrefab, this.gameObject.transform);
            TagSelectorButton t = g.GetComponent<TagSelectorButton>();
            t.tagName = objects[i].name;
            t.macAddress = objects[i].tagAddress;
            t.SetName();
        }
    }

    // Helper method to convert device ID string to byte array
    private byte[] StringToByteArray(string hex)
    {
        int length = hex.Length / 2;
        byte[] bytes = new byte[length];
        for (int i = 0; i < length; i++)
        {
            bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        }
        return bytes;
    }
}