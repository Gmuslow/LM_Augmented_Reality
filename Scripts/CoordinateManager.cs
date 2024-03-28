using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using static Microsoft.MixedReality.GraphicsTools.MeshInstancer;

public class CoordinateManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject camera;
    public GameObject anchorPrefab;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateAnchor()
    {
        Debug.Log("Creating Anchor...");
        GameObject anchor = Instantiate(anchorPrefab, camera.transform.position, Quaternion.identity);
        if (anchor.GetComponent<ARAnchor>() == null)
        {
            anchor.AddComponent<ARAnchor>();
        }
    }
}
