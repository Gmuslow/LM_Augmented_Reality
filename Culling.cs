using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Culling : MonoBehaviour
{
    public bool isCulling = true;
    public bool showOffline = true;

    public Transform cullParent;
    public int cullPerFrame = 8;
    public int pointsPerFrame = 4;
    public float cullDist = 50;
    public float textCullDist = 25;

    private int cullIndex = 0;
    private int pointIndex = 0;
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void Update()
    {
        //always update tag rotation, sometimes cull
        if (isCulling)
        {
            NewCull();
        }
        UpdateVisible(isCulling);
    }

    public void NewCull()
    {
        Vector3 camPos = cam.position;
        bool showOfflineLocal = showOffline;

        int cullPoints = cullParent.childCount;
        if (cullPoints > 0)
        {
            //cull fixed amount of points per frame to avoid slowdown
            for (int i = 0; i < cullPerFrame; i++)
            {
                //reset if above max
                if (cullIndex >= cullPoints)
                {
                    cullIndex = 0;
                    break;
                }

                Transform child = cullParent.GetChild(cullIndex);
                Vector3 pos = new Vector3(child.position.x, 0, child.position.z);
                float distToCam = Vector3.Distance(pos, camPos);
                bool cull = (distToCam > cullDist);

                //cull
                if (!cull && ((showOfflineLocal == false && child.GetComponent<Datapoint>().isOnline == true) || showOfflineLocal == true))
                {
                    child.GetChild(0).gameObject.SetActive(false);
                    child.SetParent(transform);
                    cullPoints -= 1;
                }

                cullIndex += 1;
            }

        }

        
    }

    public void UpdateVisible(bool isCulling)
    {
        Vector3 camPos = cam.position;
        int points = transform.childCount;
        bool showOfflineLocal = showOffline;

        if (points > 0)
        {
            //cull fixed amount of points per frame to avoid slowdown
            for (int i = 0; i < (pointsPerFrame); i++)
            {
                //reset if above max
                if (pointIndex >= points)
                {
                    pointIndex = 0;
                    break;
                }

                Transform child = transform.GetChild(pointIndex);
                Vector3 pos = new Vector3(child.position.x, 0, child.position.z);
                float distToCam = Vector3.Distance(pos, camPos);
                bool cull = distToCam > cullDist;
                bool cullText = distToCam > textCullDist;

                //cull text label
                if (cullText)
                {
                    child.GetChild(0).gameObject.SetActive(false);
                }
                else
                {
                    child.GetChild(0).gameObject.SetActive(true);
                }

                //un-cull
                if (isCulling && (cull || (showOfflineLocal == false && child.GetComponent<Datapoint>().isOnline == false)))
                {
                    child.SetParent(cullParent);
                    points -= 1;
                }

                //rotate to face camera
                Vector3 rot = Vector3.zero;
                rot.y = Mathf.Rad2Deg * Mathf.Atan2(child.position.x - camPos.x, child.position.z - camPos.z);
                child.eulerAngles = rot;

                pointIndex += 1;
            }

        }
    }

    public void ToggleOffline()
    {
        showOffline = !showOffline;
    }

    public void UnCull() 
    {
        //move all to transform
        isCulling = false;
        int children = cullParent.childCount;
        for (int i = children - 1; i >= 0; i--)
        {
            cullParent.GetChild(i).SetParent(transform);
        }

    }
}
