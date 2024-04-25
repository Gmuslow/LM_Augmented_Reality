using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectDatapoint : MonoBehaviour
{
    public static Transform selectedObject;

    public static Transform target;

    private static FilterData filter;

    public Transform helperTarget;


    private void Start()
    {
        filter = GetComponent<FilterData>();
        target = helperTarget;
    }

    public static void ManualSelect(Transform manualSelection) 
    {
        if (selectedObject != null)
        {
            selectedObject.GetComponent<Datapoint>().Deselected();
        }
        selectedObject = manualSelection;
        if (selectedObject != null)
        {
            HandUIHandler.instance.ChangeActivatedMenu(HandUIHandler.ActivatedMenu.AssetViewer);
            target.position = selectedObject.position;
            target.gameObject.SetActive(true);
            selectedObject.GetComponent<Datapoint>().Selected();
            filter.Filter(selectedObject.GetComponent<Datapoint>());
        }
    }

    public static void OnSelect() 
    {
       
        if (selectedObject != null) 
        {
            selectedObject.GetComponent<Datapoint>().Deselected();
        }
        selectedObject = ClosestPointToGaze.currentGaze;
        if (selectedObject != null)
        {
            HandUIHandler.instance.ChangeActivatedMenu(HandUIHandler.ActivatedMenu.AssetViewer);
            target.position = selectedObject.position;
            target.gameObject.SetActive(true);
            selectedObject.GetComponent<Datapoint>().Selected();
            filter.Filter(selectedObject.GetComponent<Datapoint>());
        }
    }

    public static void OnDeselect()
    {
        if (selectedObject != null)
        {
            selectedObject.GetComponent<Datapoint>().Deselected();
            selectedObject = null;
            if (FilterData.instance.isFiltered)         
            {
                filter.PopulateMenu();
                HandUIHandler.instance.ChangeActivatedMenu(HandUIHandler.ActivatedMenu.Filter);
                filter.ReFilter();
            }
            else 
            {
                HandUIHandler.instance.ChangeActivatedMenu(HandUIHandler.ActivatedMenu.Commands);
                filter.UnFilter();
            }

        }
    }

    public void HideNav()
    {
        target.gameObject.SetActive(false);
    }
}
