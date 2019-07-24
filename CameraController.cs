using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject mapView;
    public Camera birdCam;
    float orthoZoomSpeed = 0.5f;
    bool isMapVisible;
    CanvasController cc;

    // Start is called before the first frame update
    void Start()
    {
        isMapVisible = true; //false; //TEMP//initially birdCam is enabled, topDown cam is disabled
        setChildrenActive(mapView, false);
        birdCam.enabled = true; 
        cc = GameObject.Find("ActionsCanvas").GetComponent<CanvasController>();
    }

    void switchCamera()
    {
        birdCam.enabled = !birdCam.enabled;
        if (isMapVisible) setChildrenActive(mapView, false); //disable mapview camera
        else { setChildrenActive(mapView, true); }
    }

    public void setChildrenActive(GameObject go,bool active)
    {
        foreach (Transform child in go.transform)
        {
            child.gameObject.SetActive(active);
        }
        isMapVisible = active;
    }

    public void toggleCamera()
    {
        if (birdCam.enabled)
        {
            cc.moveMapBtnToTiles();
            //switch to ortho maek button child of MapView
        }
        else
        {
            cc.moveMapBtnToActions();
        }
        switchCamera();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}