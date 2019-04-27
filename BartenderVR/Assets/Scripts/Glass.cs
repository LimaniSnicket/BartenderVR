﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using DrinkManagement;

public class Glass : Interactable
{
    public Drink.RecipeStep[] addedToGlass;
    public EnumList.GlassTypes thisGlassType;

    public LiquidColor liquidInGlass;

    AdditiveLiquid bottleToCheck;
    const string servingAreaTag = "ServingArea";
    bool serving;
   
    public override void Start()
    {
        base.Start();

        addedToGlass = new Drink.RecipeStep[10];
        thisType = InteractableType.Glass;

    }

    public void Update()
    {
        //CheckOVRHand();
        CheckHands();

        if (liquidInGlass != null)
        {
            liquidInGlass.SetColorsToMix(addedToGlass);
           //liquidInGlass.div = (addedToGlass.GetAddedTotal(EnumList.AdditionMethod.Pour)/addedToGlass.GetAddedCount(EnumList.AdditionMethod.Pour)) / EnumList.GlassHeightModifier(thisGlassType)*10f;
            liquidInGlass.div = addedToGlass.GetAddedTotal(EnumList.AdditionMethod.Pour) 
            / (addedToGlass.GetAddedCount(EnumList.AdditionMethod.Pour) *1.5f* EnumList.GlassHeightModifier(thisGlassType));

        }

        if (canServe())
        {
            //if (!XRDevice.isPresent)
            //{
            //    if (Input.GetKeyDown(KeyCode.Space))
            //    {
            //        ServeDrink();
            //    }
            //}
            //else
            //{
                if (OrderManager.focusGlass == this && !serving){
                print("TF");
                StartCoroutine(ServeDrinkCo());
            }
            //}

        }
        //if (currentHoldingStatus != HoldingStatus.NotHeld && OrderManager.CanSetNewFocusGlass())
        //{
        //    OrderManager.focusGlass = this;
        //}

        RaycastHit CheckFor;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out CheckFor, 1f))
        {
            if(CheckRaycastComponent(CheckFor, InteractableType.Additive))
            {
                try
                {
                    bottleToCheck = CheckFor.transform.GetComponentInParent<AdditiveLiquid>();
                    bottleToCheck.SetToAdd(this);
                    print(bottleToCheck.name);
                }
                catch (System.NullReferenceException) { }
            }

            if (CheckRaycastComponent(CheckFor, InteractableType.Shaker))
            {
                if (CheckFor.transform.eulerAngles.z.CheckRotationThreshold(45f) && !CheckFor.transform.GetComponent<CocktailShaker>().addedToShaker.ContainerEmpty())
                {
                    print("Rotation is above threshold");
                    TransferTimer += Time.deltaTime;
                    if (canTransfer && TransferTimer > TransferThreshold)
                    {
                        StartCoroutine(TransferSteps());
                    }
                }
                else if (CheckFor.transform.GetComponent<CocktailShaker>().addedToShaker.ContainerEmpty())
                {
                    TransferTimer = 0f;
                    canTransfer = true;
                }
            }
        }
        else
        {
            if (bottleToCheck != null)
            {
                bottleToCheck.SetToAdd(null);
                bottleToCheck = null;
            }
        }

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward), Color.red);

        if (Input.GetKey(pourKey))
        {
            NonOVRPour();

        } else if (Input.GetKeyUp(pourKey)  && canPour() )
        {
            parent.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        }

    }


    public override void Transfer()
    {
       CocktailShaker shaker = NearbyInteractableType().GetComponent<CocktailShaker>();
       Drink.RecipeStep[] temp = shaker.addedToShaker;

        if (!canTransfer)
        {
            foreach (var s in temp)
            {
                addedToGlass.AddStepsToArray(s);
                print("Adding " + s);
            }

            shaker.addedToShaker = ClearStepsTaken();
        }
    }

    public void ServeDrink()
    {
        float newTip = AccuracyToRecipe(addedToGlass, OrderManager.currentOrder.drinkToMake);
        OrderManager.tipMoney += newTip * OrderManager.currentOrder.drinkToMake.maxTip;
        Debug.Log("Drink Accuracy: " + 100f * newTip+ "%");
        Debug.Log("$" + OrderManager.tipMoney + " in tips made so far");
        ReviewManager.CreateNewReview(OrderManager.currentOrder.drinkToMake, newTip*5f);
        OrderManager.UpdateQueue();
        OrderManager.LeaveReview(newTip);
        Destroy(this.gameObject);
    }

    public IEnumerator ServeDrinkCo()
    {
        serving = true;
        //Destroy(GetComponent<Collider>());
        float newTip = AccuracyToRecipe(addedToGlass, OrderManager.currentOrder.drinkToMake);
        OrderManager.tipMoney += newTip * OrderManager.currentOrder.drinkToMake.maxTip;
        Debug.Log("Drink Accuracy: " + 100f * newTip + "%");
        Debug.Log("$" + OrderManager.tipMoney + " in tips made so far");
        ReviewManager.CreateNewReview(OrderManager.currentOrder.drinkToMake, newTip * 5f);
        OrderManager.UpdateQueue();
        OrderManager.LeaveReview(newTip);
        yield return new WaitForSeconds(3f);
        print("Destroy this glass");
        Destroy(parent);
    }


    //<Calculating Accuracy Logic>
    //Correct Glass Type worth 10%
    //Correct Ingredient and amount worth 70%
    //Correct timing of shaking or stirring worth 20%
    //Total = 100%

    public float AccuracyToRecipe(Drink.RecipeStep[] addeds, Drink drinkToMake)
    {
        float accuracy = .7f;
        float trueTotal = drinkToMake.TotalInDrink();
        float preppedTotal = addeds.GetAddedTotal();

        float methodCheck = 0.2f / addeds.GetAddedCount();

        if (preppedTotal > 0f)
        {
            for (int i = 0; i < addeds.Length; i++)
            {
                var drink = addeds[i];
                if (drinkToMake.RecipeContainsIngredient(drink))
                {
                    Debug.Log("Glass contains " + (100f * drink.amountToAdd/preppedTotal)+ "% " + drink.addedThisStep + 
                    ". Recipe contains " + (100f * drinkToMake.FindStep(drink).amountToAdd/trueTotal)+ "% " + drinkToMake.FindStep(drink).addedThisStep);
                    accuracy -= (0.7f * drinkToMake.CorrectPercentage(drink.amountToAdd, preppedTotal, drinkToMake.FindStep(drink).amountToAdd, trueTotal));
                    float additiveMethod = drinkToMake.CheckAdditiveMethods(drinkToMake.FindStep(drink).methodsPerformedOn, drink);
                    accuracy += methodCheck * additiveMethod;
                }
                else
                {
                    accuracy -= (drink.amountToAdd/preppedTotal * 0.7f);
                }
            }

            if (thisGlassType == drinkToMake.properGlass)
            {

                Debug.Log("Correct glass + 10%");

                accuracy += 0.1f;
            }
        }
        else
        {
            return 0;
        }

        if (accuracy < 0f)
        {
            return 0;
        }

        return accuracy;
    }

    void FillDefaultValues(Drink defaultDrink)
    {
        for(int i=0; i< defaultDrink.recipe.Count; i++)
        {
            addedToGlass[i] = defaultDrink.recipe[i];
        }
    }

    bool canServe()
    {
        if (OrderManager.focusGlass == this)
        {
            return true;
        }

        return false;
    }

    public override void OnTriggerStay(Collider other)
    {
        base.OnTriggerStay(other);
    }

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject == OrderManager.servingArea)
        {
            if (OrderManager.CanSetNewFocusGlass() && currentHoldingStatus == HoldingStatus.NotHeld)
            {
                OrderManager.SetAsFocusGlass(this);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == OrderManager.servingArea)
        {
            if (OrderManager.focusGlass == this)
            {
                OrderManager.FocusGlassNull();
            }
        }
    }
}
