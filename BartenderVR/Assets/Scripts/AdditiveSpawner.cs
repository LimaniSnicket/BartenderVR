using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditiveSpawner : ObjectSpawner
{

    public Additive thisAdditiveToSpawn;
    private void Start()
    {
        thisType = Interactable.InteractableType.Additive;
        objectToSpawn = PrefabLibrary.FindAdditiveOfType(PrefabLibrary.GetPrefabDictionary(thisType), thisAdditiveToSpawn);
        print(objectToSpawn.name);
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
