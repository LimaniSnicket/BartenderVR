using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditiveObject : Interactable
{


    public override void Start()
    {
        base.Start();
        thisType = InteractableType.Additive;
    }
}
