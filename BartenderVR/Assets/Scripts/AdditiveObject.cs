using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrinkManagement;

public class AdditiveObject : Interactable
{
    protected Interactable toAddTo;

    public Additive thisAdditive;

    public override void Start()
    {
        base.Start();
        thisType = InteractableType.Additive;
    }

    private void Update()
    {
       CheckHands();
        gameObject.SetDefaults(defaultOutline, OrderManager.currentTutorialLine);
        if (NearInteractable(InteractableType.Glass)) //|| NearInteractable(InteractableType.Shaker))
        {
            toAddTo = NearbyInteractableType();
            print(toAddTo.name);
        }
        else
        {
            toAddTo = null;
        }

        if (toAddTo != null)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                AddToGlass((Glass)toAddTo, thisAdditive.additionMethod);
            }
        }

        if (currentHoldingStatus == HoldingStatus.NotHeld)
        {
            GetComponent<Collider>().isTrigger = false;
        }
        else
        {
            GetComponent<Collider>().isTrigger = true;
        }

    }

    public virtual void AddToGlass(Glass glass, EnumList.AdditionMethod additionMethod)
    {
        int indexToAdd = GetIndex(glass.addedToGlass, thisAdditive);

        glass.addedToGlass[indexToAdd].addedThisStep = thisAdditive;
        glass.addedToGlass[indexToAdd].amountToAdd++;
        glass.addedToGlass[indexToAdd].additionMethod = additionMethod;

    }

    public int GetIndex(Drink.RecipeStep[] check, Additive lookfor)
    {
        if (check.ArrayContains(lookfor))
        {
            return check.GetAdditiveIndex(lookfor);
        }

        return check.GetNextEmptyIndex();
    }

    public void SetToAdd(Glass glass)
    {
        toAddTo = glass;
    }

    public override void OnTriggerStay(Collider other)
    {
        base.OnTriggerStay(other);
        switch (thisAdditive.additionMethod)
        {
            case EnumList.AdditionMethod.Garnish:
                try
                {
                    Glass glass = other.transform.gameObject.GetComponent<Glass>();
                    print(glass);

                    if (glass.transformLibrary.TransformValid(EnumList.AdditionMethod.Garnish))
                    {
                        if (glass.transformLibrary.TargetTransform(EnumList.AdditionMethod.Garnish) == other.transform
                        && currentHoldingStatus == HoldingStatus.NotHeld)
                        {
                            AddToGlass(glass, EnumList.AdditionMethod.Garnish);
                            parent.transform.SetParent(glass.parent.transform);
                            parent.transform.position = other.transform.position;
                            //parent.transform.rotation = other.transform.rotation;
                            currentHoldingStatus = HoldingStatus.AddedToDrink;
                            GetComponent<Rigidbody>().isKinematic = true;
                        }
                    }
                }
                catch (System.NullReferenceException) { return; }
                break;
            case EnumList.AdditionMethod.CoatRim:
                print("Salt that rim boio");
                break;
        }
    }

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        if (toAddTo != null && other.gameObject.tag != TempHandTag)
        {
            if (!thisAdditive.IsLiquid())
            {
                toAddTo.GetComponent<Glass>().addedToGlass.RemoveFromArray(thisAdditive);
            }
            parent.transform.SetParent(null);
            toAddTo = null;
            GetComponent<Collider>().isTrigger = false;
        }

    }
}
