using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIExpandAnim : MonoBehaviour
{
    private VerticalLayoutGroup layout;
    public float lerpSpeed;
    public float spacing = 0;

    // Start is called before the first frame update
    void Start()
    {
        layout = GetComponent<VerticalLayoutGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        spacing = Mathf.Lerp(spacing, 0, lerpSpeed);
        layout.spacing = spacing;
    }
}
