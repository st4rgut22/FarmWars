using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Herd
{
    public Herd(int id)
    {
        herdId = id;
        herdList = new List<GameObject>();
        field = null; //contain a reference to the field
    }
    public Field field; //field assigned to herd
    public List<GameObject> herdList;
    public int herdId;
}
