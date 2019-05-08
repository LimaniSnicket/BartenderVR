using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrinkManagement;

public class AdditiveSpawner : ObjectSpawner
{
    public Additive thisAdditiveToSpawn;
    public override void Start()
    {
        //base.Start();
        thisType = Interactable.InteractableType.Additive;
        objectToSpawn = PrefabLibrary.FindAdditiveOfType(PrefabLibrary.GetPrefabDictionary(thisType), thisAdditiveToSpawn);
        defaultOutline = new DefaultOutline();
        base.Start();
        defaultOutline = defaultOutline.SetNullOutline(spawned[0].GetComponent<Outline>());
    }

    private void Update()
    {
        hand = TestHand();
        if (hand == null && spawnPerformed)
        {
            spawnPerformed = false;
        }
        //Spawn();
        UpdateSpawnedItem();
        try
        {
            gameObject.SetDefaults(defaultOutline, OrderManager.currentTutorialLine);
        }
        catch (System.NullReferenceException) { }
    }

}
