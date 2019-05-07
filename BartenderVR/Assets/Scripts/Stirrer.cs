using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrinkManagement;

public class Stirrer : Interactable
{

    float acceleration, velocity;
    public float accelerationTimer, stirThreshold;

    public Interactable toStir;
    public GameObject stirPoint;

    public override void Start()
    {
        base.Start();
        thisType = InteractableType.Stirrer;
        defaultOutline = defaultOutline.SetNullOutline(GetComponentInChildren<Outline>());
    }

    private void Update()
    {
        //CheckOVRHand();
        CheckHands();
        gameObject.SetDefaults(defaultOutline, OrderManager.currentTutorialLine);
        if (toStir!=null)
        {
            print(toStir.name);
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

                print("UGHGJHFJUF");
                try
                {
                    toStir.GetComponent<CocktailShaker>().addedToShaker.AddMethods(EnumList.AdditionMethod.Stir);
                } catch (System.NullReferenceException) { return; }
            }
        }

    }

    public void OnCollisionEnter(Collision collision)
    {
        try
        {
            Glass g = collision.gameObject.GetComponent<Glass>();
            toStir = g;
        } catch (MissingComponentException) { }
    }

    public override void OnTriggerStay(Collider collision)
    {
        base.OnTriggerStay(collision);
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

    public override void OnTriggerExit(Collider collision)
    {
        base.OnTriggerExit(collision);

        if (toStir == collision.transform.GetComponent<Interactable>())
        {
            toStir = null;
        }
    }

}
