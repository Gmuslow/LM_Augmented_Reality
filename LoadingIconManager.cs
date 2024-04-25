using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingIconManager : MonoBehaviour
{
    public Vector3 fullScale;
    public bool show = false;

    private void Start()
    {
        transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (show)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, fullScale, Time.deltaTime);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime);
            if(transform.localScale.x < 0.001)
            {
                transform.localScale = Vector3.zero;
            }
        }
    }
}
