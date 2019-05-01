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
    public GameObject shakerCap;
    Outline outline;

    bool capOn;

    public override void Start()
    {
        base.Start();
        thisType = InteractableType.Shaker;
        addedToShaker = new Drink.RecipeStep[10];
        liquidColor = GetComponent<LiquidColor>();
        outline = GetComponent<Outline>();
    }

    private void Update()
    {
        //CheckOVRHand();
        CheckHands();

        if (liquidColor != null)
        {
            liquidColor.SetColorsToMix(addedToShaker);
            liquidColor.div = (addedToShaker.GetAddedTotal(EnumList.AdditionMethod.Pour)/addedToShaker.GetAddedCount(EnumList.AdditionMethod.Pour)) / 10f;
        }

        RaycastHit CheckRay;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out CheckRay, 1f))
        {
            if (CheckRaycastComponent(CheckRay, InteractableType.Glass))
            {
                print("Raycast here");
                if (CheckRay.transform.parent.eulerAngles.z.CheckRotationThreshold(45f) && !CheckRay.transform.GetComponent<Glass>().addedToGlass.ContainerEmpty())
                {
                    print("Rotation is above threshold");
                    TransferTimer += Time.deltaTime;
                    if (canTransfer && TransferTimer > TransferThreshold)
                    {
                        //ValidateUseage(outline);
                        StartCoroutine(TransferSteps());
                    }
                }
                else if (CheckRay.transform.GetComponent<Glass>().addedToGlass.ContainerEmpty())
                {
                    TransferTimer = 0f;
                    canTransfer = true;
                }
            }
            else
            {
                UnvalidateUsage(outline);
            }

            if (Input.GetKey(pourKey))
            {
                NonOVRPour();

            }
            else if (Input.GetKeyUp(pourKey) && (currentHoldingStatus != HoldingStatus.AddedToDrink || currentHoldingStatus != HoldingStatus.NotHeld))
            {
                parent.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            }


        }

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward), Color.magenta);
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
        if (collision.gameObject == shakerCap)
        {
            capOn = true;
            shakerCap.transform.SetParent(transform);
            shakerCap.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        //if (collision.gameObject == shakerCap)
        //{
        //    capOn = false;
        //    shakerCap.transform.SetParent(transform.parent);
        //    shakerCap.GetComponent<Rigidbody>().isKinematic = false;
        //}
    }

    public void ValidateUsage()
    {
        ValidateUseage(outline);
    }
}
