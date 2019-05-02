using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrinkManagement;

public class GlassSpawner : ObjectSpawner
{
    public EnumList.GlassTypes glassType;

    private void Start()
    {
        thisType = Interactable.InteractableType.Glass;
        objectToSpawn = PrefabLibrary.FindGlassOfEnum(PrefabLibrary.GetPrefabDictionary(thisType), glassType);
        defaultOutline = new DefaultOutline();
        defaultOutline = defaultOutline.SetGlassWhite(GetComponentInChildren<Outline>());
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
