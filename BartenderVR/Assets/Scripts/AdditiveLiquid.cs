using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrinkManagement;

public class AdditiveLiquid : AdditiveObject
{
    GameObject PouringPoint;

    public float PourRate = 0f;
    float zRotation;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        thisType = InteractableType.Additive;
        try
        {

            PouringPoint = transform.GetChild(0).gameObject;

        } catch (System.NullReferenceException)
        {
            PouringPoint = this.gameObject;
        }

    }

    // Update is called once per frame
    void Update()
    {   
        zRotation = transform.eulerAngles.z;
        PourRate = CalculatePourRate();

        if (toAddTo != null)
        {
            if (Input.GetKey(KeyCode.L))
            {

                AddToGlass((Glass)toAddTo);
            }
        }
    }

    public override void AddToGlass(Glass glass)
    {
        int index = GetIndex(glass.addedToGlass, thisAdditive);
        glass.addedToGlass[index].addedThisStep = thisAdditive;
        glass.addedToGlass[index].additionMethod = EnumList.AdditionMethod.Pour;


        glass.addedToGlass[index].amountToAdd += Time.deltaTime * PourRate;
    }

    public float CalculatePourRate()
    {
        float z = (zRotation <= 180) ? zRotation : 360f - zRotation;
        return z / 180f;
    }

}
