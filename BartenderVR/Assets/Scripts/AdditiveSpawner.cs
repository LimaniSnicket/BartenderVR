using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrinkManagement;

public class AdditiveSpawner : ObjectSpawner
{
    public Additive thisAdditiveToSpawn;
    private void Start()
    {
        thisType = Interactable.InteractableType.Additive;
        objectToSpawn = PrefabLibrary.FindAdditiveOfType(PrefabLibrary.GetPrefabDictionary(thisType), thisAdditiveToSpawn);
        defaultOutline = defaultOutline.SetNullOutline(GetComponentInChildren<Outline>());
    }

    private void Update()
    {
        hand = TestHand();
        if (hand == null && spawnPerformed)
        {
            spawnPerformed = false;
        }
        Spawn();
        gameObject.SetDefaults(defaultOutline, OrderManager.currentTutorialLine);
    }

}
