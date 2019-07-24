using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HerdController : MonoBehaviour
{
    // stores all the monnsters. controls spawning, and pack behavior .
    //waves
    int wave;
    public GameObject monster;
    public int sizeOfHerd;
    List<Herd> herdList;
    List<GameObject> herd;
    List<Field> foodPlots;
    BuildController buildController;
    FieldControl fieldControl;
    GameObject bird; 
    float radiusSpawn;
    int monsterId;
    int herdId;
    float herdAngle; //herd along circle enclosing map 
    float monsterAngle; //position of monsters within herd
    Vector2 centerScreen;
    bool followingPlayer;
    Coroutine followPlayer;
    Camera MapCamera;


    void Start()
    {
        wave = 1;
        herdId = 0;
        monsterId = 0;
        sizeOfHerd = 1;
        monsterAngle = 360f / sizeOfHerd;
        radiusSpawn = 10;//Screen.height / 1.7f; //creatures will spawn just outside this circle
        Debug.Log("radius spawn is " + radiusSpawn);
        centerScreen = new Vector2(Screen.width / 2, Screen.height / 2);
        MapCamera = GameObject.Find("MapCamera").GetComponent<Camera>();
        buildController = GameObject.Find("BuildController").GetComponent<BuildController>();
        fieldControl = GameObject.Find("FieldController").GetComponent<FieldControl>();
        bird = GameObject.Find("duckofficer");
        herdList = new List<Herd>();
        followingPlayer = true;

        StartCoroutine(spawnWave());
        StartCoroutine(EatOrPursue());
    }

    Field findDestination() //eat a random field
    {
        foodPlots = fieldControl.fieldList; //update the field list
        int randomField = UnityEngine.Random.Range(0, foodPlots.Count - 1);
        return foodPlots[randomField];
    }

    void goToPlayer() //why is herd's 'field' field null and not monster's? 
    {
        Vector3 birdPos = bird.transform.position; 
        for (int i = 0; i < herdList.Count; i++)
        {
            for (int m = 0; m < herdList[i].herdList.Count; m++)
            {
                GameObject oneMonster = herdList[i].herdList[m];
                if (oneMonster == null) //monster is dead
                {
                    herdList[i].herdList.Remove(oneMonster);
                    Debug.Log("monster is null. removed");
                    continue;
                }
                Monster monsterControl = oneMonster.GetComponent<Monster>();
                monsterControl.field = null; 
                monsterControl.setDestination(birdPos);
                monsterControl.setTripReason("attack");
                //otherwise continue to already assigned field
            }
        }
    }

    void goToField()
    {
        try {
            for (int i = 0; i < herdList.Count; i++)
            {
                //int idOfHerd = herdList[i].herdId;u
                Herd herd = herdList[i];
                if (herd.field == null) //no field assigned yet 
                {
                    Field changeField = findDestination(); //reassign, if changeField is null means there are no fields
                    string nameOfField = changeField.currentPhase.name;
                    if (herd.field == null)
                    {
                        herd.field = changeField;
                    }
                    for (int m = 0; m < herd.herdList.Count; m++)
                    {
                        GameObject oneMonster = herd.herdList[m];
                        Monster monsterControl = oneMonster.GetComponent<Monster>();
                        monsterControl.field = herd.field;
                        monsterControl.setDestination(changeField.position);
                        monsterControl.setTripReason("eat");
                    }
                }
            }
        } catch (MissingReferenceException e)
        {
            Debug.Log(e.StackTrace);
        }
    }

    IEnumerator EatOrPursue() //every few seconds update behavior based on changing scenarios
    {
        while (true)
        {
            if (fieldControl.fieldList.Count == 0) //no fields
            {
                //Debug.Log("no fields. chase the player");
                goToPlayer();
                yield return new WaitForSeconds(1);
            }
            else
            {
                //Debug.Log("field found. eat field");
                try
                {
                    goToField();
                } catch (MissingReferenceException e)
                {
                    Debug.Log(e.StackTrace);
                }
                yield return new WaitForSeconds(10);
            }
        }
    }

    IEnumerator spawnWave()
    {
        while (true)
        {
            for (int i = 0; i < wave; i++)
            {
                herdAngle = 360f / wave;
                Vector2 spawnPoint = getSpawnPoint(radiusSpawn, centerScreen.x, centerScreen.y, herdAngle);//DEBUGGING (herd spawn point)! getSpawnPoint(radiusSpawn, centerScreen.x, centerScreen.y, herdAngle);
                Debug.Log("herd spawn point is " + spawnPoint);
                Herd oneHerd = new Herd(herdId);
                List<GameObject> monsterInHerd = oneHerd.herdList;
                for (int m = 0; m < sizeOfHerd; m++)
                {
                    GameObject gremlin = Instantiate(monster);
                    gremlin.name = "monster" + monsterId;
                    Monster monsterComponent = gremlin.GetComponent<Monster>();
                    monsterComponent.herdId = herdId;
                    monsterComponent.monsterId = monsterId;
                    monsterId++;
                    Vector2 monsterSpawnPoint = getSpawnPoint(0, spawnPoint.x, spawnPoint.y, 360 / (m + 1)); //50 is entirely arbitrary. experiment
                    Debug.Log("monster spawn point is " + monsterSpawnPoint);
                    RaycastHit hit;
                    Ray ray = MapCamera.ScreenPointToRay(new Vector2(spawnPoint.x,spawnPoint.y));
                    if (Physics.Raycast(ray, out hit))
                    {
                        gremlin.transform.position = hit.point;
                        Debug.Log("monster spawned at " + hit.point);
                    }
                    monsterInHerd.Add(gremlin);
                }
                herdList.Add(oneHerd);
                herdId++;
            }
            wave++;
            yield return new WaitForSeconds(120);
        }
    }

    Vector2 getSpawnPoint(float radius,float h, float k, float angle) //return vector showing position along circle
    {
        float x = h + radius * Mathf.Cos(angle);
        float y = k + radius * Mathf.Sin(angle);
        return new Vector2(x, y);
    }
}
