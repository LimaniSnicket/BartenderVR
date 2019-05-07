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

    public GameObject textContainer;

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
    bool enlargeText;
    Vector3 originalScale;

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

        originalScale = textContainer.GetComponent<RectTransform>().localScale;
      
    }

    // Update is called once per frame
    void Update()
    {
        //CheckHands();
        Holding = currentHoldingStatus;
        gameObject.SetDefaults(defaultOutline, OrderManager.currentTutorialLine);

        if (!enlargeText && RaycastDisplay.gazeTech)
        {
            StartCoroutine(EnlargeTextObject(textContainer, .25f, 2f));
        } else if (!RaycastDisplay.gazeTech)
        {
            enlargeText = false;
            textContainer.GetComponent<RectTransform>().localScale = originalScale;
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

    public IEnumerator EnlargeTextObject(GameObject toEnlarge, float lerpSpeed, float newScaleMult)
    {
        enlargeText = true;
        Vector3 enlargeVector = toEnlarge.GetComponent<RectTransform>().localScale;
        Vector3 scaleUpTo = enlargeVector * newScaleMult;
        while (!enlargeVector.SqueezeVectors(scaleUpTo, 0.1f))
        {
            enlargeVector = Vector3.Lerp(enlargeVector, scaleUpTo, Time.deltaTime * lerpSpeed);
            toEnlarge.GetComponent<RectTransform>().localScale = enlargeVector;
        }

        enlargeVector = scaleUpTo;
        yield return null;
    }
}
