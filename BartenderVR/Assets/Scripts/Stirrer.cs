using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrinkManagement;

public class Stirrer : Interactable
{

    float acceleration, velocity;
    public float accelerationTimer, stirThreshold;

    public Interactable toStir;

    public override void Start()
    {
        base.Start();
        thisType = InteractableType.Stirrer;
    }

    private void Update()
    {
       //CheckOVRHand();

        if (currentHoldingStatus != HoldingStatus.NotHeld)
        {
            GetComponent<CapsuleCollider>().isTrigger = true;
        }
        else
        {
            GetComponent<CapsuleCollider>().isTrigger = false;
        }


    }

    private void FixedUpdate()
    {
        //get the magnitude of the velocity in the horizontal axes only
        Vector2 horizontalVector = new Vector2(interactableRB.velocity.x, interactableRB.velocity.z);
        velocity = horizontalVector.magnitude;
        acceleration = velocity / Time.fixedDeltaTime;

        if (acceleration > stirThreshold)
        {
            accelerationTimer += Time.deltaTime;

        }
        else
        {
            accelerationTimer = 0f;
        }

        if (accelerationTimer > stirThreshold && toStir != null)
        {
            try
            {
                toStir.GetComponent<Glass>().addedToGlass.AddMethods(EnumList.AdditionMethod.Stir);

            } catch (System.NullReferenceException)
            {
                try
                {
                    toStir.GetComponent<CocktailShaker>().addedToShaker.AddMethods(EnumList.AdditionMethod.Stir);
                } catch (System.NullReferenceException) { return; }
            }
        }

    }

    private void OnTriggerStay(Collider collision)
    {
        try
        {
            Interactable inter = collision.gameObject.GetComponent<Interactable>();

            if (inter.thisType == InteractableType.Glass)
            {
                toStir = inter.transform.GetComponent<Glass>();

            }
            else if (inter.thisType == InteractableType.Shaker)
            {
                toStir = inter.transform.GetComponent<CocktailShaker>();
            }

        }
        catch (System.NullReferenceException) { }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (toStir == collision.transform.GetComponent<Interactable>())
        {
            toStir = null;
        }
    }

}
