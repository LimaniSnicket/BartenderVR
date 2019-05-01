using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Phone : Interactable
{
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

    public override void Start()
    {
        base.Start();
        thisType = InteractableType.Phone;
        currentPhoneState = PhoneState.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        CheckHands();

        if (currentHoldingStatus == HoldingStatus.NotHeld)
        {
            currentPhoneState = PhoneState.Locked;
        }


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
