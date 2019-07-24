 using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    Camera characterCamera;
    public ParticleSystem muzzleFlash;
    Vector2 centerScreenCoords;

    void Start()
    {
        centerScreenCoords = new Vector2(Screen.width / 2, Screen.height / 2);
        characterCamera = GameObject.Find("POVCamera").GetComponent<Camera>();
    }

    public bool isCrosshairClicked(Vector2 screenPos,float radius)
    {
        float distanceFromCenter = Vector2.Distance(screenPos, centerScreenCoords);
        if (distanceFromCenter < radius)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void shoot() 
    {
        RaycastHit hit;
        muzzleFlash.Play();
        if (Physics.Raycast(characterCamera.transform.position,characterCamera.transform.forward,out hit, range))
        {
            GameObject monster = hit.collider.gameObject;
            monster.GetComponent<Monster>().inflictDamage(50f); //deal 50 damage
            Debug.Log("shot a " + hit.transform.name);
        }
    }
}
