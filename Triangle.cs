using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle : MonoBehaviour
{
    // Start is called before the first frame update
    float width;
    float height;

    public void enableArrow(GameObject post) //place arrow in front of the watchpost
    {
        MeshRenderer seeArrow = gameObject.GetComponent<MeshRenderer>();
        BoxCollider postCollider = post.GetComponent<BoxCollider>();
        seeArrow.enabled = true;
        GameObject ladder = null;
        if (post.name == "ladder") ladder = post;
        else { ladder = post.transform.GetChild(1).gameObject; }
        gameObject.transform.parent = ladder.transform; //parent the arrow to the ladder
        gameObject.transform.localPosition = new Vector3(2,11,1.27f);
        gameObject.transform.localEulerAngles = new Vector3(-179.89f,-124.096f,495.807f);
        gameObject.transform.localScale = new Vector3(2.14f, 2.14f, -42.79f);
    }


    public void disableArrow()
    {
        MeshRenderer seeArrow = gameObject.GetComponent<MeshRenderer>();
        seeArrow.enabled = false;
    }

    private void Awake()
    {
        width = 100;
        height = 100;
    }

    private void Start()
    {
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        // make changes to the Mesh by creating arrays which contain the new values
        mesh.vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(.25f,.25f,0),new Vector3(.75f,.75f,0), new Vector3(1.57288f,-1.07288f,0), new Vector3(2.07288f,-.57288f,0)};
        mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(.25f,.25f),new Vector2(.75f,.75f),new Vector2(1.57288f, -1.07288f),new Vector2(2.07288f, -.57288f) };
        mesh.triangles = new int[] { 0, 1, 2, 4, 6, 5, 5, 3, 4 };
        gameObject.transform.eulerAngles = new Vector3(0, 0, -45);
        disableArrow();
    }

    void Update() //up, down animation here
    {
        
    }
}
