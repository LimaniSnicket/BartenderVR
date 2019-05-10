using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System;
using DrinkManagement;

public class Interactable : MonoBehaviour
{

    public const string LeftHandTag = "LeftHand";
    public const string RightHandTag = "RightHand";
    public const string TempHandTag = "goHand";

    public float interactionRadius;

    protected static bool canTransfer = true;
    protected static bool startTransfer = false;
    public GameObject parent;
    public KeyCode pourKey = KeyCode.X;

    public Dictionary<Transform, EnumList.AdditionMethod> transformLibrary = new Dictionary<Transform, EnumList.AdditionMethod>();

    public enum InteractableType
    {
        None = 0,
        Additive = 1,
        Glass = 2,
        Shaker = 3,
        Stirrer = 4,
        Muddler = 5,
        Phone = 6
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
        InRightHand = 2,
        AddedToDrink = 3
    }

    public HoldingStatus currentHoldingStatus;

    public DefaultOutline defaultOutline;
    private Vector3 originalPosition;
    public Vector3 SnapVector;

    //test out delegates
    public delegate void TransferContents();
    public static event TransferContents transfer;

    public virtual void Start()
    {
        gameObject.AddComponent<Outline>();
        thisGrabbable = GetComponentInParent<OVRGrabbable>();
        currentHoldingStatus = HoldingStatus.NotHeld;
        interactableRB = GetComponentInParent<Rigidbody>();
        parent = (transform.parent != null) ? transform.parent.gameObject : gameObject;
        originalPosition = transform.position;
        SnapVector = originalPosition;
        TransferThreshold = 1f;
        for (int i = 0; i < transform.childCount; i++)
        {
            var child =transform.GetChild(i).transform;
            if (ValidString(child.name))
            {
                transformLibrary.Add(child, ReturnMethodFromString(child));
            }
            print("Added " + ReturnMethodFromString(child) + " transform to the Transform Dictionary");
        }

        TransferThreshold = 3f;
        defaultOutline = new DefaultOutline();
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
        if (HeldByTempHand && Input.GetKeyUp(KeyCode.E))
        {
            print("Janky Drop Item");
            HeldByTempHand = false;
        }

        if (HeldByTempHand)
        {
            currentHoldingStatus = HoldingStatus.InRightHand;
            interactableRB.isKinematic = true;
            parent.transform.SetParent(TempHandFollow.transform);
            GetComponent<Collider>().isTrigger = true;
        } else if (!HeldByTempHand && currentHoldingStatus != HoldingStatus.AddedToDrink)
        {
            interactableRB.isKinematic = false;
            if (TempHandFollow != null)
            {
                TempHandFollow.transform.DetachChildren();
                TempHandFollow = null;
            }
            GetComponent<Collider>().isTrigger = false;
            currentHoldingStatus = HoldingStatus.NotHeld;
        }

        if (!HeldByTempHand)
        {
            //if (OrderManager.grabbedObjects.Contains(this))
            //{
            //    print("Removing from grabbed objects");
            //    OrderManager.grabbedObjects.Remove(this);
            //}
        }
        else
        {
            if (!OrderManager.grabbedObjects.Contains(this))
            {
                print("Adding to grabbed objects");
                OrderManager.grabbedObjects.Add(this);
            }
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
            if (currentHoldingStatus != HoldingStatus.AddedToDrink)
            {
                currentHoldingStatus = HoldingStatus.NotHeld;
            }
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

    public void SnapBack()
    {
        transform.position = SnapVector;
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


    public Drink.RecipeStep[] ClearStepsTaken(Drink.RecipeStep[] drs)
    {
        //Drink.RecipeStep[] drs = new Drink.RecipeStep[10];
        for (int i =0; i< drs.Length; i++)
        {
            drs[i].addedThisStep = null;
            drs[i].additionMethod = EnumList.AdditionMethod.None;
            drs[i].amountToAdd = 0f;
        }
        return drs;
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

    public bool OVRFingerPointed()
    {
        return true;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, new Vector3(1, 1, 1));
        Gizmos.DrawWireSphere(Vector3.zero, interactionRadius);
    }

    public virtual void OnCollisionEnter(Collision other)
    {
        try
        {
            InteractableType ty = other.gameObject.GetComponent<Interactable>().thisType;
            if (ty == this.thisType)
            {
                Physics.IgnoreCollision(this.GetComponent<Collider>(), other.collider);
            }
        }
        catch (System.NullReferenceException) { }
    }

    public virtual void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == TempHandTag && HandCanGrab(other.gameObject))
        {
            if (Input.GetKeyDown(KeyCode.Z) && !HeldByTempHand)
            {
                HeldByTempHand = true;
                TempHandFollow = other.gameObject;
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

    public void NonOVRPour()
    {
        if (canPour())
        {
            print("rotating " + this.name);
            parent.transform.localEulerAngles = new Vector3(0f, 0f, 50f);
        }
    }

    public bool canPour()
    {
        if (currentHoldingStatus == HoldingStatus.InRightHand)
        {
            return true;
        }  

        if (currentHoldingStatus == HoldingStatus.InLeftHand)
        {
            return true;
        }

        return false;
    }

   public bool GetOVRButtonsDown()
    {
        return OVRInput.Get(OVRInput.Button.One) || OVRInput.GetDown(OVRInput.Button.Three) || Input.GetKeyDown(KeyCode.Space);
    }

    public void ValidateUseage(Outline ot)
    {
        ot.OutlineWidth = 10f;
        ot.OutlineColor = Color.green;
    }

    public void UnvalidateUsage(Outline ot)
    {
        ot.OutlineWidth = 0f;
    }

    public Transform GetTransformFromLibrary(EnumList.AdditionMethod method)
    {
        foreach (var t in transformLibrary)
        {
            if (t.Value == method)
            {
                return t.Key;
            }
        }

        return null;
    }
}

[System.Serializable]
public class DefaultOutline
{
    public Color defaultColor;
    public float defaultWidth;

    public DefaultOutline() { }

    public DefaultOutline(Color c, float w)
    {
        defaultColor = c;
        defaultWidth = w;
    }

    public DefaultOutline SetGlassWhite(Outline ot)
    {
        return new DefaultOutline(Color.white, 10f);
    }

    public DefaultOutline SetNullOutline(Outline ot)
    {
        return new DefaultOutline(Color.clear, 0f);
    }

    public void SetOutlineToDefault(Outline ot)
    {
        ot.OutlineColor = defaultColor;
        ot.OutlineWidth = defaultWidth;
    }
}


