using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stirrer : Interactable
{
    public override void Start()
    {
        base.Start();
        thisType = InteractableType.Stirrer;
    }

    private void Update()
    {
        CheckOVRHand();
        
    }
}
