using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandUIHandler : MonoBehaviour
{
    public static HandUIHandler instance;

    private GameObject currentMenu;

    public ActivatedMenu activatedMenu;

    public enum ActivatedMenu {AssetViewer,Commands,Filter}

    private void Start()
    {
        instance = this;
        currentMenu = transform.GetChild(1).gameObject;
        activatedMenu = ActivatedMenu.Commands;
    }

    public void ChangeActivatedMenu(ActivatedMenu activated) 
    {
        currentMenu.SetActive(false);
        activatedMenu = activated;
        currentMenu = transform.GetChild((int)activated).gameObject;
        currentMenu.SetActive(true);
        currentMenu.GetComponent<UIExpandAnim>().spacing = -64;
        
    }

}
