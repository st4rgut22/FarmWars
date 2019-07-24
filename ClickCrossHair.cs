using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickCrossHair : MonoBehaviour, IPointerDownHandler
{

    public void OnPointerDown(PointerEventData pointEvent)
    {
        Debug.Log("clicked on image");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
