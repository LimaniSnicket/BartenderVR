using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrinkManagement;

public class AdditiveLiquid : AdditiveObject
{
    GameObject PouringPoint;

    public float PourRate = 0f;
    float zRotation;
    public float zRotationMax;

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

        zRotationMax = 240;

    }

    // Update is called once per frame
    void Update()
    {
        CheckOVRHand();

        zRotation = transform.eulerAngles.z;
        PourRate = CalculatePourRate();

        if (toAddTo != null)
        {
            if (Input.GetKey(KeyCode.L))
            {
                AddToGlass((Glass)toAddTo, EnumList.AdditionMethod.Pour);
            }
        }
    }

    public override void AddToGlass(Glass glass, EnumList.AdditionMethod additionMethod)
    {
        int index = GetIndex(glass.addedToGlass, thisAdditive);
        glass.addedToGlass[index].addedThisStep = thisAdditive;
        glass.addedToGlass[index].additionMethod = additionMethod;
        glass.addedToGlass[index].amountToAdd += Time.deltaTime * PourRate;
    }

    public float CalculatePourRate()
    {
        float z = (zRotation <= 180) ? Mathf.Abs(zRotation) : 360f - zRotation;
        if (z < zRotationMax)
        {
            return z / zRotationMax;
        }
        return 1f;
    }

}
