using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// handles construction, storage, construction animation sequences, etc. 
public class BuildController : MonoBehaviour
{
    GameObject duck;
    public GameObject stage1Tower, stage2Tower, stage3Tower, stage4Tower, stage5Tower, fence;
    public GameObject field;
    int fenceId, towerId, fieldId;
    int buildingId;
    FieldControl fieldControl;

    //under construction projects
    Dictionary<string, Building> constructionProjects;
    Dictionary<string, Field> fieldConstruction;

    private void Awake()
    {
        fenceId = 0;
        towerId = 0;
        towerId = 0;
        buildingId = 0;
        constructionProjects = new Dictionary<string, Building>();

        duck = GameObject.Find("duckofficer");
        fieldControl = GameObject.Find("FieldController").GetComponent<FieldControl>();
    }

    public Vector3 changeProjectRotation()
    {
        Vector3 characterRot = duck.transform.rotation.eulerAngles;
        return new Vector3(characterRot.x, characterRot.y-25, characterRot.z); //fence faces in the same direction as character, some adjustment needed
    }

    public bool isBldgUnderConstruction(string name)
    {
        if (constructionProjects.ContainsKey(name)) { return true;  }
        return false;
    }

    public void upgradeStage(GameObject newStage, Building bldg) //upgrade the current construction phase to a new phase once health has been reached
    {
        GameObject upgradedTower = Instantiate(newStage);
        upgradedTower.transform.position = bldg.currentPhase.transform.position;
        upgradedTower.name = bldg.currentPhase.name;
        upgradedTower.transform.eulerAngles = bldg.currentPhase.transform.eulerAngles;
        Destroy(bldg.currentPhase);
        bldg.currentPhase = upgradedTower;
    }

    public void finishConstructionProject(string projectName)
    {
        Building bldg = constructionProjects[projectName];
        GameObject go = bldg.currentPhase; //reference to object, find type
        if (go.tag == "field")
        {
            Field newField = (Field) bldg;
            fieldControl.unsproutedList.Add(newField);
            fieldControl.setNextEvolutionTime(newField); //sets the next time to upgrade soil to a plant
        }
        else {//watchtower or fence
            go.AddComponent<Health>();
        }
        constructionProjects.Remove(projectName);
    }

    public void addBuilding(GameObject go,string buildingType,Vector3 placeholderRot)
    {
        //unique identifiers for each 
        go.name = "building " + buildingId;
        Building bldg;
        if (buildingType == "fence")
        {
            bldg = new Fence(go, placeholderRot);
            constructionProjects.Add(go.name, bldg);
        }
        else if (buildingType == "tower")
        {
            bldg = new Watchtower(go, placeholderRot);
            constructionProjects.Add(go.name, bldg);
        }
        else if (buildingType == "field")
        {
            bldg = new Field(go, placeholderRot);
            constructionProjects.Add(go.name, bldg);
        }
        buildingId++;
    }

    public bool constructBuilding(string projectName,int health) //returns false once construction is finished
    {
        bool hasBldg = false;
        try
        {
            hasBldg = constructionProjects.ContainsKey(projectName);
        }
        catch(NullReferenceException e)
        {
            Debug.Log(e.StackTrace);
        }
        if (hasBldg)
        {
            Building bldg = constructionProjects[projectName];
            bldg.addHealth(health);
            return true;
        }
        else
        {
            //removed from constructionProjects dictionary
            Debug.Log("project " + projectName + " does not exist");
            return false;
        }
    }
}
