using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    private static SpawnerManager spawnerManager;

    public static Dictionary<GameObject, GlassSpawner> GlassSpawners = new Dictionary<GameObject, GlassSpawner>();
    public static Dictionary<GameObject, AdditiveSpawner> AdditiveSpawners = new Dictionary<GameObject, AdditiveSpawner>();

    private void Start()
    {
        spawnerManager = this;
        List<ObjectSpawner> objspwns = new List<ObjectSpawner>(FindObjectsOfType<ObjectSpawner>());
        foreach (var o in objspwns)
        {
            if (o.thisType == Interactable.InteractableType.Glass)
            {
                GlassSpawners.Add(o.gameObject, o.GetComponent<GlassSpawner>());
            } else if(o.thisType == Interactable.InteractableType.Additive)
            {
                AdditiveSpawners.Add(o.gameObject, o.GetComponent<AdditiveSpawner>());
            }
        }
    }
    public static GameObject GetGameObjectReference(EnumList.GlassTypes gt)
    {
        foreach (var k in GlassSpawners)
        {
            if (k.Value.glassType == gt)
            {
                return k.Key;
            }
        }

        return null;
    }

    public static GameObject GetGameObjectReference(Additive toAdd)
    {
        foreach (var k in AdditiveSpawners)
        {
            if (k.Value.thisAdditiveToSpawn == toAdd)
            {
                return k.Key;
            }
        }
        return null;
    }
}
