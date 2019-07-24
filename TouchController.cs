using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Responsible for getting touch location
identifying the types of touches
and triggering actions in other classes
*/
public class TouchController : MonoBehaviour //weapons
{
    GameObject character;
    Character birdCharacter;
    public Camera characterCamera;
    string objectName = "";
    string prevObjectName = "";
    public float rotSpeed = 0.5f;
    float startSwivelTime;
    float swivelTime = 3f;
    float rotX ,rotY, lowerYBound, upperYBound, lowerXBound, upperXBound;
    private Quaternion origQuat;
    Touch iniTouch;
    public bool isSwivel;
    GameObject dot; //red dot for aiming

    // Start is called before the first frame update
    void Start()
    {
        dot = GameObject.Find("Crosshair");
        dot.SetActive(false);
        isSwivel = false;
        character = GameObject.Find("duckofficer");
        birdCharacter = character.GetComponent<Character>();
    }

    public bool isSwivelOver() //check if return swivel is over before saving original rotation, otherwise you wont ave the right rotation
    {
        if (Time.time - startSwivelTime < swivelTime)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void turnOnDot()
    {
        dot.SetActive(true);
    }

    public void turnOffDot()
    {
        dot.SetActive(false);
    }

    public void turnOnSwivel(float lowY,float upY,float lowX,float upX) //adjust the bounds of gun viewing, and save the original rotation
    {
        isSwivel = true;
        origQuat = birdCharacter.gameObject.transform.rotation; //for returning character to original rotation
        Debug.Log("save rotation " + origQuat);
        Vector3 birdRot = birdCharacter.transform.eulerAngles;
        rotX = birdRot.x; //save rotation
        rotY = birdRot.y;
        lowerYBound = rotY - lowY; //45f;
        upperYBound = rotY + upY;  //45f;
        lowerXBound = rotX - lowX; //15f; //how far you can look upwards
        upperXBound = rotX + upX;  //10f; //how far you can look downwards
        //turn on dot
        Debug.Log("initial rotX " + rotX);
        Debug.Log("initial rotY " + rotY);
        Debug.Log("clamping rotX from " + lowerXBound + " to " + upperXBound);
        Debug.Log("clamping rotY from " + lowerYBound + " to " + upperYBound);
    }

    public void turnOffSwivel() //rotate the body back to the original position
    {
        isSwivel = false;
        //restore original rotation
        birdCharacter.rotateBody(origQuat,swivelTime);
        startSwivelTime = Time.time; //start timer for rotation
    }

    private void FixedUpdate()
    {
        if (isSwivel) { //when shooting, swivelling is enabled
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    iniTouch = touch;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    float deltaX = iniTouch.position.x - touch.position.x;
                    float deltaY = iniTouch.position.y - touch.position.y;
                    rotY -= deltaX * Time.deltaTime * rotSpeed;
                    rotX -= deltaY * Time.deltaTime * rotSpeed;
                    rotX = Mathf.Clamp(rotX, lowerXBound, upperXBound);
                    rotY = Mathf.Clamp(rotY, lowerYBound, upperYBound);
                    birdCharacter.transform.eulerAngles = new Vector3(rotX, rotY, 0);
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    iniTouch = new Touch();
                }
            }
        }
    } 
}
