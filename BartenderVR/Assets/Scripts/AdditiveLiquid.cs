using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrinkManagement;
using UnityEngine.XR;

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
        //CheckOVRHand();
        CheckHands();

        zRotation = transform.eulerAngles.z;
        PourRate = CalculatePourRate();

        if (Input.GetKey(pourKey))
        {
            if (!XRDevice.isPresent)
            {
                NonOVRPour();
            }
        }
        else if (Input.GetKeyUp(pourKey))
        {
            parent.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        }

        if (toAddTo != null)
        {
            print("Adding to glass");
            AddToGlass((Glass)toAddTo, EnumList.AdditionMethod.Pour);

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

    public override void OnTriggerStay(Collider other)
    {
        base.OnTriggerStay(other);
    }

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }

}
