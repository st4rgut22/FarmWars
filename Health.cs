using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour //for the character
{

    int health;
    Character bird;
    GameObject blueBar;
    GameObject greenBar;
    RawImage healthBar;
    RawImage staminaBar;
    RectTransform healthInfo;
    RectTransform staminaInfo;
    float greenBarRatio; //scaling the damage according to green bar's size

    // Start is called before the first frame update
    void Start()
    {
        bird = GameObject.Find("duckofficer").GetComponent<Character>();
        blueBar = GameObject.Find("BlueBar");
        staminaBar = blueBar.GetComponent<RawImage>();
        staminaInfo = blueBar.GetComponent<RectTransform>();

        greenBar = GameObject.Find("GreenBar");
        healthBar = greenBar.GetComponent<RawImage>();
        healthInfo = greenBar.GetComponent<RectTransform>();

        greenBarRatio = healthInfo.sizeDelta.x / 100f;
    }

    public void setHealthBar(float healthChange)
    {
        Vector2 size = healthInfo.sizeDelta;
        float width = size.x;
        width += healthChange * greenBarRatio;
        if (width > 100) width = 100;
        if (width < 0) width = 0;
        healthInfo.sizeDelta = new Vector2(width, size.y);
    }

    public void setStaminaBar(float healthChange)
    {
        Vector2 size = healthInfo.sizeDelta;
        float width = size.x;
        width += healthChange * greenBarRatio;
        if (width > 100) width = 100;
        if (width < 0) width = 0;
        healthInfo.sizeDelta = new Vector2(width, size.y);
    }

}
