using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System;

public class Interactable : MonoBehaviour
{

    const string LeftHandTag = "LeftHand";
    const string RightHandTag = "RightHand";

    public float interactionRadius;

    protected static bool canTransfer = true;

    public Dictionary<Transform, EnumList.AdditionMethod> transformLibrary = new Dictionary<Transform, EnumList.AdditionMethod>();

    public enum InteractableType
    {
        None = 0,
        Additive = 1,
        Glass = 2,
        Shaker = 3,
        Stirrer = 4,
        Muddler = 5
    }

    [HideInInspector]
    public OVRGrabbable thisGrabbable;

    public InteractableType thisType;

    public float TransferTimer, TransferThreshold;

    protected Rigidbody interactableRB;

    public enum HoldingStatus
    {
        NotHeld = 0,
        InLeftHand = 1,
        InRightHand = 2
    }

    public HoldingStatus currentHoldingStatus;


    //test out delegates
    public delegate void TransferContents();
    public static event TransferContents transfer;

    public virtual void Start()
    {
        thisGrabbable = GetComponent<OVRGrabbable>();
        currentHoldingStatus = HoldingStatus.NotHeld;
        interactableRB = GetComponent<Rigidbody>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).transform;
            transformLibrary.Add(child, ReturnMethodFromString(child));
            print("Added " + ReturnMethodFromString(child) + " transform to the Transform Dictionary");
        }

        TransferThreshold = 3f;
        transfer += Transfer;

    }
           
    public void CheckOVRHand()
    {
        if (thisGrabbable.grabbedBy != null)
        {
            switch (thisGrabbable.grabbedBy.gameObject.tag)
            {
                case LeftHandTag:
                    currentHoldingStatus = HoldingStatus.InLeftHand;
                    break;
                case RightHandTag:
                    currentHoldingStatus = HoldingStatus.InRightHand;
                    break;
            }
        }
        else
        {
            currentHoldingStatus = HoldingStatus.NotHeld;
        }
    }

    public virtual void Transfer()
    {
        print("Transfer");
    }

    public IEnumerator TransferSteps(GameObject above, GameObject below)
    {
        canTransfer = false;
        print("Starting transfer");
        while (TransferTimer < TransferThreshold)
        {
            yield return new WaitForEndOfFrame();
        }

        transfer();

        yield return new WaitForSeconds(2f);
        print("Completing transfer");
        //canTransfer = true;
    }


    public bool ObjectIsAbove(GameObject above, GameObject below)
    {
        if (above.transform.position.y > below.transform.position.y)
        {
            return true;
        }

        return false;
    }

    public bool NearInteractable()
    {
        Interactable interactable = NearbyInteractableType();
        if (interactable != null && interactable.thisType != InteractableType.None)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool NearInteractable(InteractableType nearThisType)
    {
        Interactable interactable = NearbyInteractableType();
        if (interactable != null && interactable.thisType == nearThisType)
        {
            return true;
        }

        return false;
    }

    public Interactable NearbyInteractableType()
    {
        var interactables = new List<Interactable>(FindObjectsOfType<Interactable>());
        interactables.Remove(this);

        var nearbyIngredient = interactables.Find(delegate (Interactable inter)
        {
            return (transform.position - inter.transform.position).magnitude <= interactionRadius;
        });

        if (nearbyIngredient!=null)
        {
            return nearbyIngredient;
        }

        return null;
    }


    public Drink.RecipeStep[] ClearStepsTaken()
    {
        return new Drink.RecipeStep[10];
    }

    //method that dynamically returns the addition method enum of a transform based on the name of the game object
    public EnumList.AdditionMethod ReturnMethodFromString(Transform t) 
    {
        foreach (var add in System.Enum.GetValues(typeof(EnumList.AdditionMethod))) //iterate through the Addition Method enum
        {
            if (string.Compare(t.gameObject.name, add.ToString()) == 0) //check the string values of the transform against the enum value for a match
            {
                return (EnumList.AdditionMethod)add; //return the enum value if there is a match
            }
        }

        return EnumList.AdditionMethod.None; //return nothing if the name of the transform is not defined in the enum
    }

    public int GetAddedCount(Drink.RecipeStep[] added)
    {
        int toReturn = 0;
        for (int i = 0; i < added.Length; i++)
        {
            if (added[i].addedThisStep != null)
            {
                toReturn++;
            }
        }

        return toReturn;
    }
}
