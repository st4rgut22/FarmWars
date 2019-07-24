using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldController : MonoBehaviour
{
    // controls the appearance of the world (explored areas), eventually day/night. 
    //tile based system. unexplored tiles are dark. explore tiles are light. 
    private float width, length, height; //size of terrain is 500x68x500 (width,height,length)
    private float screenWidth, screenHeight;
    public int mapHeight,mapWidth;
    float tileWidth;
    public Image mapTile;
    public Camera mapCam;
    public GameObject monster;
    List<List<Image>> unchartedList;
    Vector3 screenPoint;
    public Terrain terrain;
    GameObject character;
    private void Awake()
    {
        //Bounds bounds = gameObject.GetComponent<Terrain>().terrainData.bounds;
        //Vector3 mapSize = bounds.size;
        //width = mapSize.x;
        //height = mapSize.y;
        //length = mapSize.z;
        unchartedList = new List<List<Image>>();
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        mapHeight = 9;
        mapWidth = 16;
        tileWidth = screenHeight / mapHeight;
        character = GameObject.Find("duckofficer");
        blackOutMap();

    }

    private void Start()
    {
        Debug.Log("screen height is " + screenHeight);
        Debug.Log("screen width is " + screenWidth);
    }

    public IEnumerator exploreArea()
    {
        while (true)
        {
            screenPoint = mapCam.WorldToScreenPoint(character.transform.position);
            Vector3 vec3 = screenPosToTileCoords(screenPoint);
            try {
                Image exploredTile = unchartedList[(int)vec3.y][(int)vec3.x];
                if (exploredTile != null) Destroy(exploredTile);
            } //and destroy
            catch (ArgumentOutOfRangeException e) { 
                Debug.Log("row " + vec3.y + " col " + vec3.x + " does not exist"); 
            }
            //Debug.Log("try to destroy tile at row " + (int)vec3.y + " col " + (int)vec3.x);
            yield return new WaitForSeconds(10f); //every 10 seconds return new world coordinates of duck 
        }
    }

    void blackOutMap() 
    {
         //(0,0) is the bottom left in screen overlay 
        for (int r = 0; r < mapHeight; r++)
        {
            List<Image> imageList = new List<Image>();
            unchartedList.Add(imageList);
            for (int c = 0; c<mapWidth; c++)
            {
                Image tile = Instantiate(mapTile);
                tile.transform.parent = gameObject.transform;
                RectTransform tileTransform = tile.GetComponent<RectTransform>();
                tileTransform.sizeDelta = new Vector2(tileWidth, tileWidth);
                tileTransform.position = new Vector2(tileWidth*c + tileWidth/2, tileWidth * r + tileWidth/2);
                tile.GetComponent<Image>().color = Color.black;
                imageList.Add(tile);
            }
        }
    }

    Vector3 screenPosToTileCoords(Vector3 screenPos)
    {
        int row, col;
        row = (int) (screenPos.x / tileWidth);
        col = (int)(screenPos.y / tileWidth);
        return new Vector2(row, col);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
