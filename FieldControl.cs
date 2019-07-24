using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldControl : MonoBehaviour
{
    public bool fullyEvolved = false;
    int stageRequirement = 100;
    float growInterval = 10; //in seconds, time that must pass for field to evolve
    public float timeToGrow;
    BuildController buildController;
    public List<Field> fieldList; //for growth stage
    public List<Field> unsproutedList; //for growth stage
    public GameObject stage1Field, stage2Field, stage3Field;
    public GameObject babyHealth2, babyHealth1, adolHealth2, adolHealth1, adultHealth2, adultHealth1;
    HerdController herdController;
    //List<int> healthBars = new List<int> { 100, 250, 450 }; //100%,50%,25% corresponds with 2 levels of health

    // Start is called before the first frame update
    void Start()
    {
        herdController = GameObject.Find("HerdController").GetComponent<HerdController>();
        fieldList = new List<Field>();
        unsproutedList = new List<Field>();
        buildController = GameObject.Find("BuildController").GetComponent<BuildController>();
    }

    void growField(Field field)
    {
        int healthStage = field.healthCategory;
        GameObject newStage = cropStageInLife(healthStage, field.nameOfStage);
        GameObject matureField = Instantiate(newStage);
        matureField.transform.position = field.currentPhase.transform.position;
        matureField.name = field.currentPhase.name; // build Id
        matureField.transform.eulerAngles = field.buildingRotation;
        matureField.AddComponent<Health>();
        Destroy(field.currentPhase);
        field.currentPhase = matureField; //modifies the references in map and list
    }

    GameObject cropStageInLife(int currentStage,string lifestage) 
    {
        if (lifestage == "baby")
        {
            if (currentStage == 3) { return stage1Field; }
            if (currentStage == 2) { return babyHealth2; }
            if (currentStage == 1) { return babyHealth1; }
        }
        else if (lifestage == "juvie")
        {
            if (currentStage == 3) { return stage2Field; }
            if (currentStage == 2) { return adolHealth2; }
            if (currentStage == 1) { return adolHealth1; }
        }
        else if (lifestage=="adult")
        {
            if (currentStage == 3) { return stage3Field; }
            if (currentStage == 2) { return adultHealth2; }
            if (currentStage == 1) { return adultHealth1; }
        }
        else { Debug.Log("returning null, unassigned name or current stage not 3 or 2 or 1");}
        return null;
    }

    public void inflictDamage(float damage,Field field)
    {
        Debug.Log("field taking damage");
        field.takeDamage(damage);
        float percentHurt = field.realHealth / field.fullHealth;
        if (percentHurt <= 0)
        {
            fieldList.Remove(field);
            Destroy(field.currentPhase);
            field = null; // herd and monster should have a null reference to field
        }
        else if (percentHurt < .25f && field.healthCategory != 1)
        {
            field.healthCategory = 1;
            growField(field);
        }
        else if (percentHurt < .75f && field.healthCategory != 2 && field.healthCategory != 1) {
            field.healthCategory = 2;
            growField(field);
        }
    }

    public void setNextEvolutionTime(Field field)
    {
        timeToGrow = Time.time + growInterval;
        field.nextEvolutionTime = timeToGrow;
        switch (field.currentStage)
        {
            case 1:
                field.addRealHealth(100); // 100/100
                growField(field);
                field.nameOfStage = "juvie"; //NEXT STAGE
                break;
            case 2:
                field.addRealHealth(150);// 250/250
                growField(field);
                field.nameOfStage = "adult";
                break;
            case 3:
                field.addRealHealth(200); //plants become more resilient as they grow // 450/450
                growField(field);
                field.nameOfStage = "adult"; //IRRELEVANT
                field.fullyEvolved = true;
                break;
        }
        field.currentStage++;
    }

    private void Update()
    {
        for (int i = 0; i < fieldList.Count; i++)
        { //fieldnot added until sprouting for sake of monsters field detection script
            Field field = fieldList[i];
            if (!field.fullyEvolved && Time.time > field.nextEvolutionTime)
            {
                setNextEvolutionTime(field);
            }
        }
        for (int i = 0; i < unsproutedList.Count; i++)
        { 
            Field unsprouted = unsproutedList[i];
            if (Time.time > unsprouted.nextEvolutionTime)
            {
                unsproutedList.Remove(unsprouted);
                unsprouted.currentStage = 1;
                fieldList.Add(unsprouted);
            }
        }

    }

}
