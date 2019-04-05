using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditiveObject : Interactable
{
    Interactable toAddTo;

    public Additive thisAdditive;

    public override void Start()
    {
        base.Start();
        thisType = InteractableType.Additive;
    }

    private void Update()
    {
        if (NearInteractable(InteractableType.Glass)) //|| NearInteractable(InteractableType.Shaker))
        {
            toAddTo = NearbyInteractableType();
        }
        else
        {
            toAddTo = null;
        }
    }
}
