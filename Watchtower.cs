using System;
using System.Collections.Generic;
using UnityEngine;

public class Watchtower : Building
{
    double evolveInterval;
    double evolveTime;
    List<int> stageRequirements = new List<int> { 100, 200, 350, 400, 500 };//1. log phase / 2. tower1phase / 3. tower2phase / 4. towercompleted

    public Watchtower(GameObject tower, Vector3 rot)
    {
        buildingRotation = rot;
        currentPhase = tower;
        position = tower.transform.position;
        buildId = tower.name;
    }

    public override void addHealth(int health)
    {
        base.addHealth(health);
        Debug.Log("current stage is " + currentStage);
        try
        {
            if (base.health > stageRequirements[currentStage])
            {
                currentStage++;
                GameObject towerLevelUp = null;
                switch (currentStage)
                {
                    case 1:
                        towerLevelUp = buildController.stage1Tower;
                        break;
                    case 2:
                        towerLevelUp = buildController.stage2Tower;
                        break;
                    case 3:
                        towerLevelUp = buildController.stage3Tower;
                        break;
                    case 4:
                        towerLevelUp = buildController.stage4Tower;
                        break;
                    case 5:
                        towerLevelUp = buildController.stage5Tower;
                        buildController.finishConstructionProject(currentPhase.name);
                        break;
                }
                Debug.Log("upgrade stage");
                upgradeStage(towerLevelUp);
            }
        } catch(NullReferenceException e) { 
            Debug.Log(e.StackTrace);
        }
    }
}
