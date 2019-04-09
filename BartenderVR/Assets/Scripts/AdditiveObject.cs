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

    private void OnTriggerStay(Collider collider)
    {
        switch (thisAdditive.additionMethod)
        {
            case EnumList.AdditionMethod.Garnish:
                try
                {
                    Glass glass = collider.transform.gameObject.GetComponentInParent<Glass>();

                    if (glass.transformLibrary.TransformValid(EnumList.AdditionMethod.Garnish))
                    {
                        if (glass.transformLibrary.TargetTransform(EnumList.AdditionMethod.Garnish) == collider.transform
                        && currentHoldingStatus != HoldingStatus.NotHeld)
                        {
                            if (transform.parent == null)
                            {
                                AddToGlass(glass, EnumList.AdditionMethod.Garnish);
                            }

                            transform.SetParent(collider.transform.parent);
                            transform.position = collider.transform.position;
                        }
                    }

                }
                catch (System.NullReferenceException) { return; }
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (transform.parent != null)
        {

        }

        toAddTo = null;
    }
}
