using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Controls basic movements of all living things (eg ducks, monsters)
*/
public class Living : MonoBehaviour
{
    public float health;
    protected Vector3 destination;
    protected Animator anim;
    float speed = 1.0f;
    float rotateTime = 1.0f;
    float timeStartedSlerping;
    protected Vector3 target;
    Vector3 lookTarget;
    protected Quaternion lookQuaternion;
    protected float distanceFromDestination;
    float percentageComplete;
    protected bool isReachDest;
    protected Character bird;
    Health birdHealth;

    protected void setSpeed(float speed)
    {
        this.speed = speed;
    }

    protected void Awake()
    {
        lookQuaternion = gameObject.transform.rotation; //dont rotate on start
        distanceFromDestination = 1f;
        target = new Vector3(-1, -1, -1); //default value, indicates destination is not set o
        percentageComplete = 1f;
        isReachDest = false;
        health = 100;
        bird = GameObject.Find("duckofficer").GetComponent<Character>();
        birdHealth = GameObject.Find("duckofficer").GetComponent<Health>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void inflictDamage(float damage)
    {
        health -= damage;
        if (gameObject.name == "duckofficer")
        {
            birdHealth.setHealthBar(-damage);
        }
        else
        {
            Debug.Log("monster health is " + health);
        }
    }
     
    public virtual void setDestination(Vector3 dest) {
        dest = new Vector3(dest.x, transform.position.y, dest.z); //maintain character's y position or else it will lean forward
        target = dest;
        rotateBody(dest, 1);
    }

    public void setRotate()
    {
        timeStartedSlerping = Time.time;
        percentageComplete = 0;
        rotateTime = 3;
        Debug.Log("rotate 180 degrees along y axis");
        lookQuaternion = transform.rotation*Quaternion.AngleAxis(180, Vector3.up);
    }

    protected void rotateBody(Vector3 lookAt,float rotTime) //clicking on terrain
    {
        percentageComplete = 0; //used for condition checking when slerp is done, so other rotations can be
        rotateTime = rotTime;
        timeStartedSlerping = Time.time;
        lookTarget = lookAt;
        lookQuaternion = Quaternion.LookRotation(lookAt - transform.position);
        lookQuaternion = Quaternion.Euler(transform.rotation.eulerAngles.x, lookQuaternion.eulerAngles.y, transform.rotation.eulerAngles.z); //freeze x and z axis
    }

    public void rotateBody(Quaternion lookAt,float rotTime)
    {
        percentageComplete = 0;
        timeStartedSlerping = Time.time;
        lookQuaternion = lookAt;
        rotateTime = rotTime;
    }

    public virtual void reachDestination()
    {
        //Debug.Log("base class reach destination method called");
    }

    // Update is called once per frame
    protected void Update()
    {
        //moving algorithm
        if (target!=new Vector3(-1, -1, -1))// && rotateTime>.5f) ? ? ?  //half-way finished rotating, move to next target
        {
            Debug.Log("still moving to destination");
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);
            if (Vector3.Distance(transform.position, target) < distanceFromDestination) //reached the destination
            {
                //Debug.Log("reached destination" + transform.position);
                target = new Vector3(-1, -1, -1); //reset the destination to default value, meaning stopped
                reachDestination();
                isReachDest = true;
            }
            else
            {
                //Debug.Log("go to destination " + target);
                //Debug.Log( gameObject.name + "'s distance from destination is " + Vector3.Distance(transform.position, target) + "stop " + distanceFromDestination + " from destination ");
                isReachDest = false; 
            }
        }
        if (percentageComplete <= 1f)
        {
            float timeSinceStarted = Time.time - timeStartedSlerping;
            percentageComplete = timeSinceStarted / rotateTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, lookQuaternion, percentageComplete);
        }
        else
        {
        }
    }
}
