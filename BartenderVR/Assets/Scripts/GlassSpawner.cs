using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassSpawner : ObjectSpawner
{
    public EnumList.GlassTypes glassType;

    private void Start()
    {
        thisType = Interactable.InteractableType.Glass;
        objectToSpawn = PrefabLibrary.FindGlassOfEnum(PrefabLibrary.GetPrefabDictionary(thisType), glassType);
    }

    private void Update()
    {
        hand = TestHand();
        if (hand == null && spawnPerformed)
        {
            spawnPerformed = false;
        }
        Spawn();
    }
}
