using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building
{
    protected int health;
    public Vector3 position;
    public GameObject building;
    public int currentStage; //stage in projects lifecycle
    protected BuildController buildController;
    protected string buildId;
    public Vector3 buildingRotation;
    public GameObject currentPhase;

    public Building() {
        currentStage = 0;
        health = 0; 
        buildController = GameObject.Find("BuildController").GetComponent<BuildController>();
    }

    protected void upgradeStage(GameObject newStage) //upgrade the current construction phase to a new phase once health has been reached
    {
        buildController.upgradeStage(newStage,this);
    }

    public virtual void addHealth(int health)
    {
        this.health += health; ;
    }
}
