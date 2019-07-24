using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Living
{
    public int herdId;
    public int monsterId;
    public Vector3 dest;
    public Field field;
    string setDestinationReason;
    HerdController herdController;
    FieldControl fieldControl;
    bool eat;
    // Start is called before the first frame update
    private void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
        distanceFromDestination = 1;
    }

    void Start()
    {
        herdController = GameObject.Find("HerdController").GetComponent<HerdController>();
        fieldControl = GameObject.Find("FieldController").GetComponent<FieldControl>();
        field = null;
        eat = false;
    }

    void stopAllActions()
    {
        anim.SetBool("isWalking", false);
        anim.SetBool("isRunning", false);
        anim.SetBool("isAttacking", false);
        anim.SetBool("isEating", false);
    }

    public void goToField(Field field)
    {
        setDestination(field.position);
    }

    public override void reachDestination()
    {
        anim.SetBool("isWalking", false);
        if (setDestinationReason == "eat")
        {
            eat = true;
        }
        else if (setDestinationReason == "attack")
        {
            Debug.Log("reach destination attack reason");
            anim.SetBool("isAttacking",true);
        }
    }

    public void setTripReason(string reason)
    {
        setDestinationReason = reason;
        if (reason=="eat") distanceFromDestination = 3f;
        if (reason=="attack") distanceFromDestination = 3f;
    }

    public override void setDestination(Vector3 dest)
    {
        dest = new Vector3(dest.x, transform.position.y, dest.z); //maintain character's y position or else it will lean forward
        target = dest;
        rotateBody(dest, .5f); //rotate complete in half a second
        anim.SetBool("isWalking", true);
    }

    public void goToCharacter(Vector3 characterPos) //want to update more frequently then field
    {
        //Debug.Log("monster go to character");
        setDestination(characterPos);
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        Debug.Log("monster health is " + health);
        if (health < float.Epsilon)
        {
            Debug.Log("destroy self");
            anim.SetBool("isAttacking", false);
            anim.SetBool("isDying", true);
            Destroy(gameObject, 5);
        }
        if (anim.GetBool("isAttacking"))
        {
            bird.inflictDamage(.1f);
        }
        if (isReachDest) anim.SetBool("isWalking", false);
        else { 
            stopAllActions();
            anim.SetBool("isWalking", true); 
        }
        if (field != null && eat) //eat means close enough to field to eat
        {
            anim.SetBool("isEating", true);
            fieldControl.inflictDamage(1f, field);
        }
        else if (field == null) // the field has been eaten/destroyed
        {
            anim.SetBool("isEating", false);
            eat = false;
        }
    }
}
