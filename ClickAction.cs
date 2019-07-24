using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ClickAction : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public GameObject watchpost, fence, field;
    public GameObject fenceLog, seed, watchpostLog;
    public GameObject placeThing;
    Text fenceDesc, watchpostDesc, fieldDesc;
    public Camera characterCamera;
    Character character;
    CanvasController inventory;
    BuildController buildController;
    bool isSufficientFunds;
    bool isOverImage;
    string saveName;
    Vector3 falsyVal;

    private void Start()
    {
        character = GameObject.Find("duckofficer").GetComponent<Character>();
        inventory = GameObject.Find("ActionsCanvas").GetComponent<CanvasController>();
        fenceDesc = inventory.fenceDesc;
        watchpostDesc = inventory.watchpostDesc;
        fieldDesc = inventory.fieldDesc; 
        buildController = GameObject.Find("BuildController").GetComponent<BuildController>();
        isSufficientFunds = false;
        falsyVal = new Vector3(-1, -1, -1);
        isOverImage = false;
        saveName = "";
    }

    void disableColliderForPlacement(GameObject go) 
    {
        BoxCollider[] boxColliders = go.GetComponents<BoxCollider>();
        foreach (BoxCollider box in boxColliders)
        {
            box.enabled = false;
        }
    }

    public GameObject createMaterials(string objectName,Vector3 location) //purchased tower materials
    {
        GameObject material = null;
        if (objectName == "imgFence")
        {
            material = Instantiate(fenceLog);
            material.transform.position = location;
        }
        else if (objectName == "imgField")
        {
            material = Instantiate(seed);
            material.transform.position = location;
        }
        else if (objectName == "imgPost")
        {
            material = Instantiate(watchpostLog);
            material.transform.position = location;
        }
        return material;
    }

    public void createPlaceholderObject(string objectName) //intantiate GameObject when the finger is released
    {
        if (placeThing != null)
        {
            Debug.Log("place thing " + placeThing.name + " hasn't been placed");
            return; //somethings already been assigned, but it hasn't been placed for some reason
        }
        Vector3 rot = buildController.changeProjectRotation();

        if (objectName == "imgFence")
        {
            Debug.Log("assign " + objectName + " to placething");
            placeThing = Instantiate(fence);
            disableColliderForPlacement(placeThing.transform.GetChild(0).gameObject); //fence child has collider on it
        }
        else if (objectName == "imgPost")
        {
            Debug.Log("assign " + objectName + " to placething");
            placeThing = Instantiate(watchpost); 
            disableColliderForPlacement(placeThing.transform.gameObject);
        }
        else if (objectName == "imgField")
        {
            Debug.Log("assign " + objectName + " to placething");
            placeThing = Instantiate(field);
        }
        placeThing.transform.eulerAngles = rot;
    }

    Vector3 didRaycastHitTerrain(Vector2 screenCoord)
    {
        RaycastHit hit;
        Ray ray = characterCamera.ScreenPointToRay(screenCoord);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.name == "Terrain")
            {
                return hit.point;
            }
            return falsyVal;
        }
        return falsyVal;
    }

    public void OnPointerUp(PointerEventData pointEvent) //doesn't always work
    {
        Debug.Log("onpointerup fires");
    }

    public void OnPointerDown(PointerEventData pointEvent)
    {
        Debug.Log("pointer down on game object tag is " + gameObject.tag);
        pointerdown(gameObject.tag);
        isOverImage = true; //detect when touch has lifted
    }

    public void pointerdown(string thing)
    {
        saveName = thing;
        //if sufficient funds
        isSufficientFunds = inventory.isSufficientFund(thing);
        createPlaceholderObject(thing); //instantiate thing
        character.dontMove(); 
        //character.dontMoveThisFrame(); 
    }

    public void pointerup()  //user wants to create it. have an option where user can undo the decision
    {
        if (EventSystem.current.IsPointerOverGameObject()) //show description dont build
        {
            Debug.Log("show description");
            inventory.showDescription(saveName);
        }
        else //build something if sufficient funds
        {
            Vector3 raycastHitPoint = didRaycastHitTerrain(Input.mousePosition);
            if (raycastHitPoint != falsyVal)
            {
                if (isSufficientFunds && placeThing!=null)
                {
                    Vector3 rot = placeThing.transform.eulerAngles;
                    //Debug.Log("sufficient funds");
                    //instantiate material prefabs
                    //create new building
                    GameObject material = createMaterials(saveName, raycastHitPoint);
                    if (saveName == "imgPost")
                    {
                        Debug.Log("add watchpost");
                        buildController.addBuilding(material,"tower",rot);
                    }
                    else if (saveName == "imgField")
                    {
                        Debug.Log("add field");
                        buildController.addBuilding(material,"field",rot);
                    }
                    else if (saveName == "imgFence")
                    {
                        Debug.Log("add fence");
                        buildController.addBuilding(material,"fence",rot);
                    }
                }
            }
            else { Debug.Log("raycast falsy value"); }
        }
        Debug.Log("destroy place thing called " + placeThing.name);
        Destroy(placeThing);
        placeThing = null;
        character.buildingOver();
    }

    private void Update()
    {
    
        /*click downthis is hell*/
        /*
        if (Input.GetMouseButton(0)) 
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Ray ray = characterCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(Input.mousePosition, Vector2.down);
                Debug.Log("mouse position is " + Input.mousePosition);
                Debug.DrawRay(characterCamera.ScreenToWorldPoint(Input.mousePosition), Vector3.down,Color.green,10);
                if (hit.collider != null && placeThing==null) //wrong triggers when finger is not over an image
                {
                    string colliderTag = hit.collider.tag;
                    //Debug.Log("--------------");
                    // Debug.Log("Name = " + hit.collider.name);
                    //Debug.Log("Tag = " + hit.collider.tag);
                    //Debug.Log("Hit Point = " + hit.point);
                    //Debug.Log("Object position = " + hit.collider.gameObject.transform.position);
                    //Debug.Log("--------------");
                    if (colliderTag == "imgFence" || colliderTag == "imgPost" || colliderTag == "imgField")
                    {
                        pointerdown(colliderTag);
                        isOverImage = true; //detect when touch has lifted
                    }
                }
            }
        }
        */
        /*click up*/
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("getMouseButtonUp fires");
            if (isOverImage)
            {
                pointerup();
                isOverImage = false;
            }
        }
        if (placeThing != null) //move place thing to cursor location while draggin
        {
            Vector3 raycastHitPoint = didRaycastHitTerrain(Input.mousePosition);
            if (raycastHitPoint != falsyVal)
            {
                placeThing.transform.position = raycastHitPoint;
            }
        }
    }
}