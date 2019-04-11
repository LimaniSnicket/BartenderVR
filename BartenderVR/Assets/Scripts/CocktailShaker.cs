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
        CheckOVRHand();

        if (liquidColor != null)
        {
            liquidColor.SetColorsToMix(addedToShaker);
            liquidColor.div = (addedToShaker.GetAddedTotal(EnumList.AdditionMethod.Pour)/addedToShaker.GetAddedCount(EnumList.AdditionMethod.Pour)) / 10f;
        }

        RaycastHit CheckRay;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out CheckRay, 1f))
        {
            if (CheckRaycastComponent(CheckRay, InteractableType.Glass))
            {
                if (CheckRay.transform.parent.eulerAngles.z.CheckRotationThreshold(45f) && !(CheckRay.transform.GetComponent<Glass>().addedToGlass.ContainerEmpty()))
                {
                    print("Rotation is above threshold");
                    TransferTimer += Time.deltaTime;
                    if (canTransfer && TransferTimer > TransferThreshold)
                    {
                        StartCoroutine(TransferSteps());
                    }
                }
                else if (CheckRay.transform.GetComponent<Glass>().addedToGlass.ContainerEmpty())
                {
                    TransferTimer = 0f;
                    canTransfer = true;
                }
            }

        }

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up), Color.magenta);
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

        if (shakeTimer > shakeTimerThreshold && !addedToShaker.ContainerEmpty())
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
        Glass glass = NearbyInteractableType().InteractableGlass();
        Drink.RecipeStep[] temp = glass.addedToGlass;

        if (!canTransfer)
        {
            foreach (var s in temp)
            {
                addedToShaker.AddStepsToArray(s);
                print("Adding " + s);
            }

            glass.addedToGlass = ClearStepsTaken();
        }

        print("Cocktail shaker transfer");
    }

}
