using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleColor : MonoBehaviour
{
    Additive additive;
    Material bottleMaterial;
    public int materialIndex;

    private void Start()
    {
        additive = GetComponentInChildren<AdditiveLiquid>().thisAdditive;
        bottleMaterial = GetComponent<MeshRenderer>().materials[materialIndex];
        bottleMaterial.SetColor("_Tint", additive.AdditiveColor.color);
    }

}
