using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrinkManagement;

public class GlassSpawner : ObjectSpawner
{
    public EnumList.GlassTypes glassType;

    public override void Start()
    {

        thisType = Interactable.InteractableType.Glass;
        objectToSpawn = PrefabLibrary.FindGlassOfEnum(PrefabLibrary.GetPrefabDictionary(thisType), glassType);
        defaultOutline = new DefaultOutline();
        base.Start();
        defaultOutline = defaultOutline.SetGlassWhite(spawned[0].GetComponent<Outline>());
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
