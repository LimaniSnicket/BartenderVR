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
            InteractableLiquids.Add(a.gameObject, a);
        }

    }

    private void Update()
    {

    }
    public static GameObject GetGameObjectReference(EnumList.GlassTypes gt)
    {
        foreach (var k in GlassSpawners)
        {
            if (k.Value.glassType == gt)
            {
                var comp = k.Key.GetComponent<GlassSpawner>();
                if (comp.existing.Count > 0)
                {
                    return k.Key.GetComponent<GlassSpawner>().existing[0];
                }
                else
                {
                    return k.Key.GetComponent<GlassSpawner>().spawned[0];
                }
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
                    return k.Key.GetComponent<AdditiveSpawner>().spawned[0];
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

    static GlassSpawner GetGlassSpawnerType(EnumList.GlassTypes gt)
    {
        foreach (var g in GlassSpawners)
        {
            if (g.Value.glassType == gt)
            {
                return g.Value;
            }
        }

        return null;
    }

    public static void RemoveFromSpawner(GameObject toRemove)
    {
        try
        {
            Interactable inter = toRemove.GetComponent<Interactable>();
            switch (inter.thisType)
            {
                case Interactable.InteractableType.Glass:
                    Glass g = toRemove.GetComponent<Glass>();
                    GlassSpawner gs = GetGlassSpawnerType(g.thisGlassType);
                    gs.RemoveFromExisting(g.gameObject);
                    break;
            }
        }
        catch (System.NullReferenceException)
        {
            print("honestly just fucking put a " +
            	"bullet in my head at this point");
        }
    }

}
