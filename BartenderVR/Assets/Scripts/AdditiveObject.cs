using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrinkManagement;

public class AdditiveObject : Interactable
{
    [SerializeField]
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

    public override void OnCollisionEnter(Collision other)
    {
        base.OnCollisionEnter(other);
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

                    if (glass.transformLibrary.TransformValid(EnumList.AdditionMethod.Garnish))
                    {
                        Transform garnishPoint = glass.GetTransformFromLibrary(EnumList.AdditionMethod.Garnish);

                        if (currentHoldingStatus != HoldingStatus.AddedToDrink || currentHoldingStatus != HoldingStatus.NotHeld)
                        {
                            AddToGlass(glass, EnumList.AdditionMethod.Garnish);
                            currentHoldingStatus = HoldingStatus.AddedToDrink;


                            transform.position = garnishPoint.position;
                            transform.SetParent(garnishPoint);
                            GetComponent<Collider>().isTrigger = true;
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
