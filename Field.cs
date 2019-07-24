using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : Building
{
    public bool fullyEvolved = false;
    int stageRequirement = 100; //initial health during construction stage. 
    public float fullHealth; //used for comparison 
    public float realHealth;
    public float nextEvolutionTime;
    public int healthCategory; //different from current stage, indicates health of crop. 3 is full health, 2 is medium, 1 is low
    public string nameOfStage;
    FieldControl fieldControl;
    public Field(GameObject field,Vector3 rot)
    {
        health = 0;
        buildingRotation = rot;
        healthCategory = 3;
        currentPhase = field;
        position = field.transform.position;
        buildId = field.name;
        fieldControl = GameObject.Find("FieldController").GetComponent<FieldControl>();
        nameOfStage = "baby";
    }

    public void addRealHealth(int health)
    {
        fullHealth += health;
        realHealth = fullHealth;
    }

    public void takeDamage(float damage)
    {
        realHealth -= damage;
    }

    public override void addHealth(int health) //construction phase
    {
        base.addHealth(health);
        if (base.health > stageRequirement)
        {
            upgradeStage(buildController.field); //upgrade to a field, there's only one stage
            buildController.finishConstructionProject(currentPhase.name);
        }
    }

    void Update()
    {
        Debug.Log("field update called");
    }
}
