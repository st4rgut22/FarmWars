using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Character : Living
{
    GameObject leftHand;
    string action; //action chosen in game controller
    Camera birdCam;
    string objectName;
    bool move; //while placing a new building
    bool moveThisFrame;
    GameObject itemInUse;
    public GameObject hammer, axe, gun;
    bool foundHammer, foundGun, foundAxe;
    bool followMonster;
    GameObject monsterToFollow;
    bool onWatchTower;
    bool isMuzzleFlash;
    Vector3 falsyVector;
    Vector3 idlingLocation;
    string setDestinationReason; //trigger an animation on reaching destination
    string saveProjectName;
    Triangle upArrow;
    float beginShoot; // bad but whatever
    float endShoot; 
    bool onTower;
    bool weaponEquipped;
    bool shotSomething;
    string weaponToEquip;
    BuildController buildController;
    TouchController tc;
    CanvasController cc;
    Gun gunController;
    List<string> weaponStash;
    int weaponIndex;

    private void Awake()
    {
        base.Awake();
        shotSomething = true;
        weaponEquipped = false;
        isMuzzleFlash = false;
        foundHammer = false;
        foundGun = false;
        foundAxe = false;
        onWatchTower = false;
        setDestinationReason = "";
        falsyVector = new Vector3(-1, -1, -1);
        idlingLocation = falsyVector;
        anim = GetComponent<Animator>();
        leftHand = gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;
        upArrow = GameObject.Find("Triangle").GetComponent<Triangle>();
        onTower = false;
        followMonster = false;
        monsterToFollow = null;
    }

    void Start()
    {
        weaponIndex = 0;
        weaponStash = new List<string>();
        weaponStash.Add(null);
        birdCam = gameObject.GetComponent<CameraController>().birdCam;
        WorldController wc = GameObject.Find("MapView").GetComponent<WorldController>();
        StartCoroutine(wc.exploreArea());
        stopAllActions();
        move = true;
        moveThisFrame = true;
        buildController = GameObject.Find("BuildController").GetComponent<BuildController>();
        tc = GameObject.Find("Touch Controller").GetComponent<TouchController>();
        cc = GameObject.Find("ActionsCanvas").GetComponent<CanvasController>();
        gunController = GameObject.Find("glock").GetComponent<Gun>();
        weaponStash.Add(gun.name); //TEMPORARY, for testing swivel 
    }

    public bool isOnTower()
    {
        return onWatchTower;
    }

    public void dontMove() 
    {
        move = false;
    }

    public void dontMoveThisFrame()
    {
        moveThisFrame = false;
    }

    public void buildingOver() //move 
    {
        move = true;
    }

    void stopAllActions()
    {
        anim.SetBool("isSowing", false);
        anim.SetBool("isWalking", false);
        anim.SetBool("isRunning", false);
        anim.SetBool("isHacking", false);
        anim.SetBool("isBuilding", false);
        anim.SetBool("isShooting", false);
        anim.SetBool("isClimbing", false);
    }

    void putAwayItem()
    {
        if (itemInUse != null) Destroy(itemInUse); //switch tools 
    }

    public void equipWeapon()
    {
        weaponIndex = weaponIndex % weaponStash.Count;
        weaponToEquip = weaponStash[weaponIndex];
        weaponIndex++;
        weaponEquipped = true;
        tc.turnOffDot();
        if (tc.isSwivel) tc.turnOffSwivel(); //if we were just using a gun. exit swivel position
        if (weaponToEquip == "hammer")
        {
            pickUpObject(hammer);
            GameObject.Find("POVCamera").GetComponent<Camera>().fieldOfView = 94f; //original field of view
        }
        else if (weaponToEquip == "axe")
        {
            pickUpObject(axe);
            GameObject.Find("POVCamera").GetComponent<Camera>().fieldOfView = 94f; //original field of view
        }
        else if (weaponToEquip == "glock")
        {
            pickUpObject(gun);
            tc.turnOnDot(); //red accuracy dot
            if (!tc.isSwivelOver()) return; //if not done rotating to original rotation, quit early
            if (onTower)
            {
                tc.turnOnSwivel(90, 90, 90, 10);
                GameObject.Find("POVCamera").GetComponent<Camera>().fieldOfView = 16.9f; //weapon scope, zoomed in 
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            }
            else
            {
                tc.turnOnSwivel(45, 45, 15, 10); //adjusting field of view (boundaries eg how far you can turn left, right, up, down) 
            }
        }
        else
        {
            GameObject.Find("POVCamera").GetComponent<Camera>().fieldOfView = 94f; //original field of view
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            weaponEquipped = false;
            putAwayItem();
        }
    }

    public void pickUpObject(GameObject item)
    {
        putAwayItem(); //put away the previous item 
        itemInUse = Instantiate(item);
        itemInUse.GetComponent<Rigidbody>().isKinematic = true;
        itemInUse.GetComponent<MeshCollider>().enabled = false;
        itemInUse.transform.parent = leftHand.transform;
        itemInUse.transform.localPosition = new Vector3(0,0,0);
        if (itemInUse.name.Substring(0,6) == "hammer")
        {
            itemInUse.transform.localEulerAngles = new Vector3(0, 90, 180);
        }
        else if (itemInUse.name.Substring(0,3) == "axe")
        {
            itemInUse.transform.localEulerAngles = new Vector3(170, -80, 0);
        }
        else if (itemInUse.name.Substring(0,5) == "glock")
        {
            gunController = itemInUse.GetComponent<Gun>();
            itemInUse.transform.localEulerAngles = new Vector3(-41.3f, 80, 30.7f);
        }
    }

    override public void setDestination(Vector3 dest)
    {
        //Debug.Log("set destination, rotate bod");
        idlingLocation = falsyVector;
        //Debug.Log("dest is " + dest + " and move is " + move);
        if (move) 
        {
            stopAllActions();
            target = dest;
            //Debug.Log("move to " + target);
            anim.SetBool("isWalking", true);
            rotateBody(dest,1);
        }
    }

    void walkBackwardsToDestination(Vector3 dest)
    {
        stopAllActions();
        target = dest;
        anim.SetBool("isWalking", true);
    }

    void climbToDestination(Vector3 dest)
    {
        stopAllActions();
        anim.SetBool("isClimbing", true);
        target = dest;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("collided with a " + collision.gameObject.name);
        if (setDestinationReason == "goUpLadder" || setDestinationReason == "goDownLadder") //we touched the watchpost 
        {
            Debug.Log("set destination reason is perchTower");
            if (collision.gameObject.name == "ladder") //colliding with ladder
            {
                Debug.Log("collided with ladder");
                //begin climb animation & disappear arrow
                upArrow.disableArrow();
                Vector3 dest = new Vector3(-1,-1,-1);
                gameObject.GetComponent<Rigidbody>().useGravity = false; //disable gravity for climbin
                if (setDestinationReason == "goUpLadder")
                {
                    dest = new Vector3(transform.position.x, transform.position.y + 8.5f, transform.position.z);
                    distanceFromDestination = .1f;
                    setDestinationReason = "perchTower";
                    climbToDestination(dest);
                }
                else if (setDestinationReason == "goDownLadder")
                {
                    setRotate(); //makes the player face the opposite direction on y axis
                    Vector3 unitForward = -transform.forward;
                    dest = new Vector3(transform.position.x, transform.position.y, transform.position.z) - unitForward*4; // -7.5f
                    distanceFromDestination = .2f; //so we don't try to go into the collider
                    setDestinationReason = "climbToGround";
                    collision.collider.enabled = false;
                    walkBackwardsToDestination(dest);
                }
            }
        }
    }

    public override void reachDestination()
    {
        //Debug.Log("derived class reach destination method called");
        stopAllActions(); //result in idling animation if not reason is set for destination
        //Debug.Log("reached somewhere. idling. ");
        if (setDestinationReason == "onTower")
        {
            Debug.Log("on Tower");
            gameObject.GetComponent<Rigidbody>().useGravity = true;
            onTower = true;
            cc.toggleMapBtn(true);
            //idlingLocation = gameObject.transform.position; //prevent character drift. problem is use gravity on does not work if we maintain character psosition. ditch4now
            return;
        }
        else if (setDestinationReason == "build")
        {
            Debug.Log("building");
            buildSomething();
            pickUpObject(hammer);
        }
        else if (setDestinationReason == "sow")
        {
            anim.SetBool("isSowing", true);
        }
        else if (setDestinationReason == "perchTower")
        {
            Debug.Log("perching tower");
            anim.SetBool("isWalking", true);
            Vector3 forwardDir = gameObject.transform.forward;
            //idlingLocation = falsyVector;
            setDestination(gameObject.transform.position + forwardDir*3); //move in the direction the character is facing
            setDestinationReason = "onTower";
        }
        else if (setDestinationReason == "climbToGround")
        {
            climbToDestination(new Vector3(transform.position.x, transform.position.y - 7.5f, transform.position.z));
            setDestinationReason = "onGround";
        }
        else if (setDestinationReason == "onGround")
        {
            gameObject.GetComponent<Rigidbody>().useGravity = true; //turn gravity back on
            onTower = false;
            cc.toggleMapBtn(false);
        }
        else if (setDestinationReason == "attack")
        {
            if (weaponToEquip=="hammer" || weaponToEquip == "axe")
            {
                hackSomething();
                if (monsterToFollow!=null) //monster is still alive
                {
                    Monster m = monsterToFollow.GetComponent<Monster>();
                    m.inflictDamage(.5f);
                }
            }
        }
        else //walking/running to a destination. idle after reaching
        {
            idlingLocation = gameObject.transform.position; //prevent character drift
        }
    }

    public void hackSomething()
    {
        anim.SetBool("isHacking", true);
        anim.SetBool("isShooting", false);
        anim.SetBool("isBuilding", false);
    }

    public void buildSomething()
    {
        anim.SetBool("isBuilding", true);
        anim.SetBool("isShooting", false);
        anim.SetBool("isHacking", false);
    }

    public void shootSomething()
    {
        beginShoot = Time.time;
        Debug.Log("begin shoot time is  " + beginShoot);
        anim.SetBool("isShooting", true);
    }

    void showBuildIcon(GameObject hitObject)
    {
        GameObject tool = Instantiate(hammer);
        tool.GetComponent<Rigidbody>().isKinematic = true;
        tool.transform.position = new Vector3(hitObject.transform.position.x, hitObject.transform.position.y + 3, hitObject.transform.position.z);
        Destroy(tool, 5); //destroy the tool after 5 seconds
    }

    void climbWatchpost(GameObject hitObject)
    {
        upArrow.enableArrow(hitObject); //move the arrow to the watchpost location
        distanceFromDestination = 1; //stop in front of the ladder
        setDestinationReason = "goUpLadder";
    }

    void constructProject(string actionType,GameObject hitObject)
    {
        showBuildIcon(hitObject);
        saveProjectName = hitObject.name;
        setDestinationReason = actionType;
        distanceFromDestination = 10;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update(); //move function
        if (anim.GetBool("isHacking") && monsterToFollow == null)
        {
            stopAllActions(); //killed monster. stop attacking
        }
        if (health < float.Epsilon)
        {
            anim.SetBool("isDead", true);
            //END GAME
        }
        if (anim.GetBool("isHacking") && monsterToFollow!=null)
        {
            Monster m = monsterToFollow.GetComponent<Monster>();
            m.inflictDamage(1);
        }
        if (idlingLocation != falsyVector)
        {
            transform.position = idlingLocation; //keep character still in idle mode
        }
        if (anim.GetBool("isBuilding") || anim.GetBool("isSowing")) //doing work
        {
            bool construct = buildController.constructBuilding(saveProjectName, 1); //returns a bool checking if construction is complete. This will stop building action. 
            if (!construct)
            {
                putAwayItem();
                stopAllActions();
            }
        }

        else if (anim.GetBool("isWalking") || anim.GetBool("isRunning")) //how to get default state? 
        {
            if (!weaponEquipped) putAwayItem();
        }
    }

    private void FixedUpdate()
    {
        if (weaponEquipped && weaponToEquip == "glock")
        {
            if (Input.touchCount == 1 && weaponToEquip == "glock" && gunController.isCrosshairClicked(Input.GetTouch(0).position, 15) && !anim.GetBool("isShooting")) //2nd parameter is the radius of the crosshair
            {
                Debug.Log("shoot");
                shootSomething(); //set beginShoot
                isMuzzleFlash = true;
            }
            if (Time.time - beginShoot > 1.3f && isMuzzleFlash) //approximate time to reach shoot 
            {
                isMuzzleFlash = false;
                gunController.shoot(); //muzzle flash animation, finds target
            }
            if (Time.time - beginShoot > 1.5f) //approximate duration fo animation
            {
                Debug.Log("set shoot to false");
                anim.SetBool("isShooting", false);
            }
        }
    }

    void LateUpdate()
    {
        if (weaponToEquip != "glock" && Input.touchCount==1 && Input.GetTouch(0).phase==TouchPhase.Began) //we are not firing a glock, requires stable aiming
        {
            RaycastHit hit;
            setDestinationReason = ""; //reset 
            Ray ray = birdCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.collider.gameObject;
                objectName = hitObject.name;
                Debug.Log("hit a " + objectName + " at " + hit.transform.position);

                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) //prevents character from moving when clicking UI elements thanks unity
                {
                    return;
                }
                else if (onTower) //dont let other hit objects affect character movement if on tower
                {
                    if (hitObject.name == "ladder")//descend
                    {
                        distanceFromDestination = .5f;
                        setDestinationReason = "goDownLadder";
                        setDestination(hit.point);
                    }
                    else if (hitObject.tag == "watchpost")
                    {
                        BoxCollider ladderCollider = GameObject.Find("ladder").GetComponent<BoxCollider>();
                        if (!ladderCollider.enabled) ladderCollider.enabled = true; //enable ladder collider if it has been turned off accidentally
                        distanceFromDestination = 3.5f;
                        Debug.Log("hit object tag is watchpost move to " + hit.point);
                        setDestination(hit.point);
                    }
                    return;
                }
                else if (objectName == "Terrain")
                {
                    followMonster = false;
                    setSpeed(3); //walking speedt
                    distanceFromDestination = 1f;
                    setDestination(hit.point);
                }
                else if (objectName == "axe" || objectName == "hammer" || objectName == "glock")
                {
                    followMonster = false;
                    if (Vector3.Distance(transform.position,hitObject.transform.position)<10)
                    {
                        if (objectName == "axe")
                        {
                            foundAxe = true; //equip
                        }
                        else if (objectName == "hammer")
                        {
                            foundHammer = true;
                        }
                        else if (objectName == "glock")
                        {
                            foundGun = true;
                        }
                        weaponStash.Add(hitObject.name); //add weapon to our arsenal
                        weaponIndex = weaponStash.Count - 1; //make the newest weapon the default wewapon
                        Destroy(hitObject); //equip and destroy. object disappears into your napsack 
                    }
                    else
                    {
                        //Debug.Log("distance to " + objectName + " is " + Vector3.Distance(transform.position, hitObject.transform.position));
                        setSpeed(3);
                        distanceFromDestination = 1f;
                        setDestination(hitObject.transform.position);
                    }
                }
                else if ((objectName.Length > 8 && objectName.Substring(0,8)=="building") || objectName=="ladder") //separate collider
                {
                    followMonster = false;
                    if (hitObject.tag == "field")
                    {
                        if (buildController.isBldgUnderConstruction(hitObject.name)) constructProject("sow", hitObject);
                        distanceFromDestination = 10;
                    }
                    else
                    {
                        Debug.Log("hit object is a " + hitObject.tag);
                        //Building bldg = buildController.GetTower(objectName); //get building reference so you can change construction progress
                        if (hitObject.tag == "watchpost")
                        {

                            if (buildController.isBldgUnderConstruction(hitObject.name))
                            {
                                constructProject("build", hitObject);
                            }
                            else
                            {
                                Debug.Log("climb the watchpost");
                                //climb the watchpost if it is finished
                                climbWatchpost(hitObject);

                            }

                        }
                        if (hitObject.tag=="fence") // a tower or a fence, ignore found hammer for now
                        {
                            if (buildController.isBldgUnderConstruction(hitObject.name)) constructProject("build", hitObject);
                            distanceFromDestination = 10;
                        }
                        //if (!foundHammer) { Debug.Log("you haven't discovered the hammer yet. Good luck building things"); }
                        
                    }

                    setDestination(hitObject.transform.position); //walk to the hit object regardless of whether its under construction
                }
                else if (objectName.Length>7 && objectName.Substring(0, 7) == "monster")
                {
                    Debug.Log("move to the monster");
                    setDestinationReason = "attack";
                    monsterToFollow = hitObject;
                    followMonster = true; //save the monster
                    distanceFromDestination = 4;
                }
                if (followMonster)
                {
                    setDestination(monsterToFollow.transform.position);
                }
                else
                {
                    monsterToFollow = null;
                }
            }
        }
    }
}
