using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fence : Building
{
    int stageRequirement = 100;

    public Fence(GameObject fenceObject,Vector3 rot) 
    {
        buildingRotation = rot;
        currentPhase = fenceObject;
        position = fenceObject.transform.position;
        buildId = fenceObject.name;
    }

    public override void addHealth(int health)
    {
        base.addHealth(health);
        //Debug.Log("fence health is " + base.health); 
        if (base.health > stageRequirement)
        {
            upgradeStage(buildController.fence);
            buildController.finishConstructionProject(currentPhase.name);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
