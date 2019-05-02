using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrinkManagement;

public class SpawnerManager : MonoBehaviour
{
    private static SpawnerManager spawnerManager;

    public static Dictionary<GameObject, GlassSpawner> GlassSpawners = new Dictionary<GameObject, GlassSpawner>();
    public static Dictionary<GameObject, AdditiveSpawner> AdditiveSpawners = new Dictionary<GameObject, AdditiveSpawner>();
    public static Dictionary<GameObject, AdditiveLiquid> InteractableLiquids = new Dictionary<GameObject, AdditiveLiquid>();

  
    private void Start()
    {
        print("What the fuck");
        spawnerManager = this;
        List<GlassSpawner> objspwns = new List<GlassSpawner>(FindObjectsOfType<GlassSpawner>());
        List<AdditiveSpawner> addspwns = new List<AdditiveSpawner>(FindObjectsOfType<AdditiveSpawner>());
        List<AdditiveLiquid> addlqds = new List<AdditiveLiquid>(FindObjectsOfType<AdditiveLiquid>());

        foreach (var o in objspwns)
        {
            GlassSpawners.Add(o.gameObject, o);
        }

        foreach(var asp in addspwns)
        {
            AdditiveSpawners.Add(asp.gameObject, asp);
        }

        foreach (var a in addlqds)
        {
            InteractableLiquids.Add(a.parent.gameObject, a);
        }

    }

    private void Update()
    {
        //foreach (var c in GlassSpawners)
        //{
        //    print(c.Key.name);
        //}
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
        if (toAdd.IsLiquid())
        {
            foreach(var k in InteractableLiquids)
            {
                if (k.Value.thisAdditive == toAdd)
                {
                    return k.Key;
                }
            }
        }
        else
        {
            foreach (var k in AdditiveSpawners)
            {
                if (k.Value.thisAdditiveToSpawn == toAdd)
                {
                    return k.Key;
                }
            }
        }
        return null;
    }
}
