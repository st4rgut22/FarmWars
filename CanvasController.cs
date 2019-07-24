using UnityEngine;
using UnityEngine.UI;

// Handles interactions with the UI, and tracks inventory
public class CanvasController : MonoBehaviour
{
    Button storeBtn;
    Button buildBtn;
    Button weaponBtn;
    GameObject buildCanvas;

    GameObject storeCanvas;
    GameObject buildBtnObject, storeBtnObject,weaponButtonObject,mapButtonObject; //for hiding,showing
    public int seed, corn, bullets, wood;
    int seed4Corn, bullet4Corn, wood4Corn;
    int fenceCost, watchpostCost, fieldCost;
    public Button exitStoreBtn, exitBuildBtn, buyBulletBtn, buyWoodBtn, buySeedBtn, mapBtn;
    public Text bulletCount, woodCount, seedCount,cornCount;
    public Text fenceDesc, watchpostDesc, fieldDesc;
    Character character; //for toggling weapons
    TouchController tc;//TEMPORARY

    private void Awake()
    {
        seed = 10000;
        corn = 1000;
        bullets = 0;
        wood = 1000;
        //exchange rates
        seed4Corn = 9;
        bullet4Corn = 6;
        wood4Corn = 5;

        //build costs
        fenceCost = 100; //15 wood for 1 fence
        watchpostCost = 200; //60 wood for 1 watchpost
        fieldCost = 50; //50 seeds for 1 field

        mapButtonObject = GameObject.Find("MapButton");

        storeBtnObject = GameObject.Find("TradeButton");
        storeBtn = storeBtnObject.GetComponent<Button>();

        buildBtnObject = GameObject.Find("BuildButton");
        buildBtn = buildBtnObject.GetComponent<Button>();

        weaponButtonObject = GameObject.Find("WeaponButton");
        weaponBtn = weaponButtonObject.GetComponent<Button>();
    }

    void Start()
    {
        character = GameObject.Find("duckofficer").GetComponent<Character>();
        buildCanvas = GameObject.Find("BuildCanvas");
        storeCanvas = GameObject.Find("StoreCanvas");
        //closeBuildMenu();
        closeStore();
        closeBuildMenu();
        disableAllDescription();

        storeBtn.onClick.AddListener(() => openStore());

        buildBtn.onClick.AddListener(() => openBuildMenu());

        weaponBtn.onClick.AddListener(() => character.equipWeapon()); 

        exitStoreBtn.onClick.AddListener(() => closeStore());  //TEMPORARY
        exitBuildBtn.onClick.AddListener(() => closeBuildMenu());
        mapBtn.onClick.AddListener(() => character.GetComponent<CameraController>().toggleCamera()); //change to ortho mode

        buyBulletBtn.onClick.AddListener(() => buyBullet());
        buyWoodBtn.onClick.AddListener(() => buyWood());
        buySeedBtn.onClick.AddListener(() => buySeed());
        mapButtonObject.SetActive(false);
    }

    public void moveMapBtnToActions()
    {
        mapBtn.transform.parent = GameObject.Find("ActionsCanvas").transform;
    }

    public void moveMapBtnToTiles()
    {
        mapBtn.transform.parent = GameObject.Find("MapView").transform;
    }

    public void toggleMapBtn(bool val)
    {
        mapButtonObject.SetActive(val);
    }

    public void showDescription(string objectName)
    {
        disableAllDescription();
        if (objectName == "fence")
        {
            fenceDesc.enabled = true;
        }
        else if (objectName == "watchpost")
        {
            watchpostDesc.enabled = true;
        }
        else if (objectName == "field")
        {
            fieldDesc.enabled = true;
        }
    }

    public void disableAllDescription()
    {
        fieldDesc.enabled = false;
        fenceDesc.enabled = false;
        watchpostDesc.enabled = false;
    }

    public bool isSufficientFund(string buildObject)
    {
        if (buildObject == "imgFence")
        {
            if (wood < fenceCost) return false;
            wood -= fenceCost;
            return true;
        }
        if (buildObject == "imgPost")
        {
            if (wood < watchpostCost) return false;
            wood -= watchpostCost;
            return true;
        }
        if (buildObject == "imgField")
        {
            if (seed < fieldCost) return false;
            seed -= fieldCost;
            return true;
        }
        return false;
    }

    void setCornText() 
    {
        cornCount.text = "Corn (" + corn + ")";  
    }

    void hideBuildStoreBtns()
    {
        storeBtnObject.SetActive(false);
        buildBtnObject.SetActive(false);
    }

    void showBuildStoreBtns()
    {
        storeBtnObject.SetActive(true);
        buildBtnObject.SetActive(true);
    }

    void buyWood()
    {
        if (corn > 0)
        {
            wood += wood4Corn;
            woodCount.text = "Wood (" + wood + ")";
            corn--;
            setCornText();
        }
    }

    void buySeed()
    {
        if (corn>0)
        {
            seed += seed4Corn;
            seedCount.text = "Seed (" + seed + ")";
            corn--;
            setCornText();
        }
    }

    void buyBullet()
    {
        if (corn > 0)
        {
            bullets += bullet4Corn;
            bulletCount.text = "Bullets (" + bullets + ")";
            corn--;
            setCornText();
        }
    }

    void openBuildMenu()
    {
        buildCanvas.SetActive(true);
        hideBuildStoreBtns();
    }

    void closeBuildMenu()
    {
        //Character c = GameObject.Find("duckofficer").GetComponent<Character>();
        //c.setRotate(); //flip 
        buildCanvas.SetActive(false);
        showBuildStoreBtns();
    }

    void openStore()
    {
        storeCanvas.SetActive(true);
        hideBuildStoreBtns();
    }

    void closeStore()
    {
        storeCanvas.SetActive(false);
        showBuildStoreBtns();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
