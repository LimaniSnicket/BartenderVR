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
                AddToGlass((Glass)toAddTo);
            }
        }
    }

    public virtual void AddToGlass(Glass glass)
    {
        int indexToAdd = GetIndex(glass.addedToGlass, thisAdditive);

        glass.addedToGlass[indexToAdd].addedThisStep = thisAdditive;
        glass.addedToGlass[indexToAdd].amountToAdd++;

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
        try
        {
           

        }
        catch (System.NullReferenceException) { return; }
    }

    private void OnTriggerExit(Collider other)
    {
        toAddTo = null;
    }
}
