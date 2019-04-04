using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CocktailShaker : Interactable
{

    public Drink.RecipeStep[] addedToShaker;

    public float acceleration;
    float lastVelocity;

    public float accelerationThreshold;

    public float shakeTimer, shakeTimerThreshold;

    public int Shaken;

    public override void Start()
    {
        base.Start();
        thisType = InteractableType.Shaker;
        addedToShaker = new Drink.RecipeStep[10];
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

        if (shakeTimer > shakeTimerThreshold && GetAddedCount(addedToShaker) > 0)
        {
            Shaken++;

            foreach (var a in addedToShaker)
            {
                if (!a.methodsPerformedOn.Contains(EnumList.AdditionMethod.Shake))
                {
                    a.methodsPerformedOn.Add(EnumList.AdditionMethod.Shake);
                } 
            }
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
                StorageArray = addedToShaker;
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
