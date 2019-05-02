using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DrinkManagement;

public class Phone : Interactable
{
    private static Phone UserPhone;

    public Canvas tutorialCanvas;
    public Canvas yelpCanvas;
    public enum PhoneState
    {
        Locked = 0,
        YelpRating = 1,
        YelpReview = 2, 
        Tutorial = 3
    }
    public PhoneState currentPhoneState;
    public static HoldingStatus Holding;
    Dictionary<Canvas, PhoneState> canvasPhoneStates = new Dictionary<Canvas, PhoneState>();
    public static GameObject phoneObject;

    public override void Start()
    {
        base.Start();
        thisType = InteractableType.Phone;
        phoneObject = this.gameObject;
        if (OrderManager.tutorialActive)
        {
            currentPhoneState = PhoneState.Tutorial;
        }
        UserPhone = this;

        canvasPhoneStates.Add(yelpCanvas, PhoneState.YelpReview);
        canvasPhoneStates.Add(tutorialCanvas, PhoneState.Tutorial);
      
    }

    // Update is called once per frame
    void Update()
    {
        CheckHands();
        Holding = currentHoldingStatus;
        gameObject.SetDefaults(defaultOutline, OrderManager.currentTutorialLine);

        if (currentPhoneState==PhoneState.Tutorial && HeldByUser())
        {

        }

       

    }

    public static bool HeldByUser()
    {
        if (Holding == HoldingStatus.InLeftHand || Holding == HoldingStatus.InRightHand)
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
}
