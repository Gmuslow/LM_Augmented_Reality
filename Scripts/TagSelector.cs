using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

public class TagSelector : MonoBehaviour
{
    // Start is called before the first frame update
    public List<TagObject> objects = new List<TagObject>();

    public GameObject buttonPrefab;
    void Start()
    {


        byte[] target1 = { 0xAC, 0x23, 0x3F, 0xAB, 0x46, 0x06 };
        byte[] target2 = { 0xAC, 0x23, 0x3F, 0xAB, 0x45, 0xFE };


        objects.Add(new TagObject("tag1", target1));
        objects.Add(new TagObject("tag2", target2));


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

    public void InitializeTagSelector()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            GameObject g = Instantiate(buttonPrefab, this.gameObject.transform);
            TagSelectorButton t = g.GetComponent<TagSelectorButton>();
            t.tagName = objects[i].name;
            t.macAddress = objects[i].tagAddress;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
