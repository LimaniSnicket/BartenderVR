using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System;
using DrinkManagement;

public class Interactable : MonoBehaviour
{

    const string LeftHandTag = "LeftHand";
    const string RightHandTag = "RightHand";
    const string TempHandTag = "goHand";

    public float interactionRadius;

    protected static bool canTransfer = true;
    protected static bool startTransfer = false;
    public GameObject parent;

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

    [HideInInspector]
    public bool HeldByTempHand;
    [HideInInspector]
    public GameObject TempHandFollow;

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
            if (ValidString(child.name))
            {
                transformLibrary.Add(child, ReturnMethodFromString(child));
            }
            print("Added " + ReturnMethodFromString(child) + " transform to the Transform Dictionary");
        }

        parent = (transform.parent != null) ? transform.parent.gameObject : gameObject;

        TransferThreshold = 3f;
       //transfer += Transfer;
    }

    public void CheckHands()
    {
        if (!XRDevice.isPresent)
        {
            CheckTempHand();
        }
        else
        {
            CheckOVRHand();
        }
    }

    public void CheckTempHand()
    {
        if (HeldByTempHand && Input.GetKeyUp(KeyCode.Z))
        {
            print("Janky Drop Item");
            HeldByTempHand = false;
        }

        if (HeldByTempHand)
        {
            if (TempHandFollow != null)
            {
                transform.position = TempHandFollow.transform.position;
            }
            currentHoldingStatus = HoldingStatus.InRightHand;
            interactableRB.isKinematic = true;
        } else if (!HeldByTempHand)
        {
            interactableRB.isKinematic = false;
            TempHandFollow = null;
            currentHoldingStatus = HoldingStatus.NotHeld;
        }
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

    public bool CheckRaycastComponent(RaycastHit outHit,  InteractableType typeToCheck)
    {
        try
        {
            if (outHit.transform.GetComponentInParent<Interactable>().thisType == typeToCheck)
            {
                return true;
            }

        } catch (System.NullReferenceException) { return false; }

        return false;
    }

    public virtual void Transfer()
    {
        print("Transfer");
    }

    public IEnumerator TransferSteps()
    {
        canTransfer = false;
        print("Starting transfer");
   
        Transfer();

        yield return new WaitForSeconds(2f);
        print("Completing transfer");

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

    bool ValidString(string st)
    {
        foreach (var am in System.Enum.GetNames(typeof(EnumList.AdditionMethod)))
        {
            if (string.Compare(st, am) == 0)
            {
                return true;
            }
        }

        return false;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, new Vector3(1, 1, 1));
        Gizmos.DrawWireSphere(Vector3.zero, interactionRadius);
    }

    public void OnTriggerStay(Collider other)
    {
        print("working");
        if (other.gameObject.tag == TempHandTag)
        {
            print("test hand in range of interactable");

            if (Input.GetKey(KeyCode.Z) && !HeldByTempHand)
            {
                if (HandCanGrab(other.gameObject))
                {
                    HeldByTempHand = true;
                    TempHandFollow = other.gameObject;
                    parent.transform.SetParent(TempHandFollow.transform);
                }
            }

        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == TempHandTag)
        {
            other.transform.DetachChildren();
            HeldByTempHand = false;
            TempHandFollow = null;
        }
    }

    bool HandCanGrab(GameObject hand)
    {
        if (hand.transform.childCount > 0)
        {
            return false;
        }
        return true;
    }
}
