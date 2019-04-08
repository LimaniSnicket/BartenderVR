using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DrinkManagement;

public class Glass : Interactable
{
    public Drink.RecipeStep[] addedToGlass;
    public EnumList.GlassTypes thisGlassType;

    public LiquidColor liquidInGlass;

    AdditiveLiquid bottleToCheck;

    public override void Start()
    {
        base.Start();

        addedToGlass = new Drink.RecipeStep[10];
        thisType = InteractableType.Glass;

    }

    public void Update()
    {
        CheckOVRHand();

        //if (OrderManager.s_debuggingMode && Input.GetKeyDown(KeyCode.S))
        //{
        //    FillDefaultValues(OrderManager.currentOrder.drinkToMake);
        //}

        if (liquidInGlass != null)
        {
            liquidInGlass.SetColorsToMix(addedToGlass);
            //liquidInGlass.div = (GetAddedTotal(addedToGlass, EnumList.AdditionMethod.Pour)/GetAddedCount(addedToGlass, EnumList.AdditionMethod.Pour))/10f;
            liquidInGlass.div = addedToGlass.GetAddedTotal(EnumList.AdditionMethod.Pour) / addedToGlass.GetAddedCount(EnumList.AdditionMethod.Pour) / EnumList.GlassHeightModifier(thisGlassType);
        }

        if (NearInteractable(InteractableType.Shaker))
        {
            if (Input.GetKey(KeyCode.M))
            {
                TransferTimer += Time.deltaTime;

                if (canTransfer)
                {
                    StartCoroutine(TransferSteps(this.gameObject, NearbyInteractableType().gameObject));
                }
            } else if (Input.GetKeyUp(KeyCode.M))
            {
                //StopAllCoroutines();
                TransferTimer = 0f;
                canTransfer = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ServeDrink();
        }

        if (currentHoldingStatus != HoldingStatus.NotHeld && OrderManager.CanSetNewFocusGlass())
        {
            OrderManager.focusGlass = this;
        }


        RaycastHit CheckForBottle;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out CheckForBottle, 1f))
        {

            try
            {
                bottleToCheck = CheckForBottle.transform.gameObject.GetComponentInParent<AdditiveLiquid>();
                bottleToCheck.SetToAdd(this);
                print(bottleToCheck.name);

            } catch (System.NullReferenceException) { }
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

    }

   

    public override void Transfer()
    {
        addedToGlass = (ObjectIsAbove(gameObject, NearbyInteractableType().gameObject) ? ClearStepsTaken() : NearbyInteractableType().GetComponent<CocktailShaker>().addedToShaker);


        //if (ObjectIsAbove(this.gameObject, NearbyInteractableType().gameObject))
        //{
        //    addedToGlass = ClearStepsTaken();
        //}
        //else
        //{
        //    print("FUCK");
        //    addedToGlass = NearbyInteractableType().GetComponent<CocktailShaker>().addedToShaker;
        //}

        print("fuck but in the glass script this time, inheritance is fuckin wild");
    }

    public void ServeDrink()
    {
        float newTip = AccuracyToRecipe(addedToGlass, OrderManager.currentOrder.drinkToMake);
        OrderManager.tipMoney += newTip * OrderManager.currentOrder.drinkToMake.maxTip;
        Debug.Log("Drink Accuracy: " + 100f * newTip+ "%");
        Debug.Log("$" + OrderManager.tipMoney + " in tips made so far");
        OrderManager.UpdateQueue();
        OrderManager.LeaveReview(newTip);
        Destroy(this.gameObject);
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
}
