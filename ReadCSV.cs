using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Microsoft.VisualBasic;

public class ReadCSV : MonoBehaviour
{
    public static Dictionary<string, string> ReadDataCSV(string csv)
    {

        Dictionary<string, string> readData = new Dictionary<string, string>();

        string path = Path.Combine(Application.streamingAssetsPath, csv);
        if (File.Exists(path))
        {
            StreamReader reader = new StreamReader(File.OpenRead(path));
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] splitLine = line.Split(",");
                readData.Add(splitLine[0].TrimStart('0'), splitLine[1]);
            }
        }

        return readData;
    }

    public static Dictionary<string, string[]> ReadDataCSVList(string csv)
    {

        Dictionary<string, string[]> readData = new Dictionary<string, string[]>();

        string path = Path.Combine(Application.streamingAssetsPath, csv);
        if (File.Exists(path))
        {
            StreamReader reader = new StreamReader(File.OpenRead(path));
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] splitLine = line.Split(",");
                string[] values = new string[3];
                
                for(int i=1; i < splitLine.Length; i++)
                {
                    values[i-1] = splitLine[i];
                }

                readData.Add(splitLine[0].TrimStart('0'), values);
            }
        }

        return readData;
    }
}
