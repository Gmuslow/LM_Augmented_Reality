using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CommandListManager : MonoBehaviour
{
    private Transform parentImage;
    public GameObject row;
    public static InformationDisplay instance;
    public string[] commands;
    // Start is called before the first frame update
    void Start()
    {
        parentImage = gameObject.transform.GetChild(1);
        foreach (string command in commands)
        {
            string[] cmd = command.Split(',');
            AddRowToList("\"" + cmd[0] + "\" :",cmd[1]);
        }
    }

    private void AddRowToList(string command, string result)
    {
        GameObject newRow = Instantiate(row, parentImage);
        newRow.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = command;
        newRow.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = result;
    }
}
