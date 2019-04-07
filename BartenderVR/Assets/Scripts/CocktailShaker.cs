using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrinkManagement;

public class CocktailShaker : Interactable
{

    public Drink.RecipeStep[] addedToShaker;

    public float acceleration;
    float lastVelocity;

    public float accelerationThreshold;

    public float shakeTimer, shakeTimerThreshold;

    public int Shaken;

    LiquidColor liquidColor;

    public override void Start()
    {
        base.Start();
        thisType = InteractableType.Shaker;
        addedToShaker = new Drink.RecipeStep[10];
        liquidColor = GetComponent<LiquidColor>();
    }

    private void Update()
    {
        if (liquidColor != null)
        {
            liquidColor.SetColorsToMix(addedToShaker);
            liquidColor.div = (addedToShaker.GetAddedTotal(EnumList.AdditionMethod.Pour)/addedToShaker.GetAddedCount(EnumList.AdditionMethod.Pour)) / 10f;
        }
    }

    private void FixedUpdate()
    {
        lastVelocity = interactableRB.velocity.magnitude;
        acceleration = (lastVelocity) / Time.deltaTime;

        if (acceleration >= accelerationThreshold)
        {
            shakeTimer += Time.deltaTime;

        }
        else
        {
            shakeTimer = 0f;
        }

        if (shakeTimer > shakeTimerThreshold && addedToShaker.GetAddedCount() > 0)
        {
            addedToShaker.AddMethods(EnumList.AdditionMethod.Shake);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            interactableRB.AddForce(Vector3.up * 15f, ForceMode.Impulse);
        }

    }

    public override void Transfer()
    {
        //if (startTransfer)
        //{
            if (ObjectIsAbove(this.gameObject, NearbyInteractableType().gameObject))
            {
            //StartCoroutine(CheckTransfer(addedToShaker, NearbyInteractableType().GetComponent<Glass>().addedToGlass));
                addedToShaker = ClearStepsTaken();
            }
            else
            {
            //StartCoroutine(CheckTransfer(NearbyInteractableType().GetComponent<Glass>().addedToGlass, addedToShaker));
            //addedToShaker = NearbyInteractableType().GetComponent<Glass>().addedToGlass;
            addedToShaker = NearbyInteractableType().GetComponent<Glass>().addedToGlass;
            }

        //}

        print("Cocktail shaker transfer");
    }

}
