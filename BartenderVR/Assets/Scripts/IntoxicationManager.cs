using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrinkManagement;

public class IntoxicationManager : MonoBehaviour
{
    static IntoxicationManager im;
    public float IntoxicationLevel;
    public Material DrunkMaterial;
   static List<GameObject> FuckUpThemVertices = new List<GameObject>();

    private void Awake()
    {
        im = this;
    }

    private void Start()
    {
        FuckUpThemVertices = FindFuckedUpVertices();
    }

    private void Update()
    {
        foreach (var f in FuckUpThemVertices)
        {
            f.GetComponent<MeshRenderer>().material.SetFloat("_Amount", IntoxicationLevel);
        }
    }

    List<GameObject> FindFuckedUpVertices()
    {
        var tempList = new List<GameObject>(FindObjectsOfType<GameObject>());
        foreach (var obj in tempList)
        {
            if (obj.GetComponent<MeshRenderer>().material != DrunkMaterial)
            {
                tempList.Remove(obj);
            }
        }
        return tempList;
    }

}
