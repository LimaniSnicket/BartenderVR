using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabLibrary : MonoBehaviour
{
    private static PrefabLibrary pl;

    const string GlassModelResourcePath = "GlassPrefabs";
    const string AdditivePrefabResourcePath = "AdditivePrefabs";

    static GameObject[] GlassPrefabs;
    static GameObject[] AdditivePrefabs;

    static Dictionary<Interactable.InteractableType, GameObject[]> PrefabDictionary = new Dictionary<Interactable.InteractableType, GameObject[]>();

    private void Awake()
    {
        pl = this;
        GlassPrefabs = Resources.LoadAll<GameObject>(GlassModelResourcePath);
        //foreach (var a in GlassPrefabs)
        //{
        //    print(a.name);
        //}
        AdditivePrefabs = Resources.LoadAll<GameObject>(AdditivePrefabResourcePath);

        PrefabDictionary.Add(Interactable.InteractableType.Glass, GlassPrefabs);
        PrefabDictionary.Add(Interactable.InteractableType.Additive, AdditivePrefabs);
    }

    public static GameObject[] GetPrefabDictionary(Interactable.InteractableType toReturn)
    {
        foreach (var pair in PrefabDictionary)
        {
            if (pair.Key == toReturn)
            {
                return pair.Value;
            }
        }

        return null;
    }

    public static GameObject FindGlassOfEnum(GameObject[] arr, EnumList.GlassTypes type)
    {
        foreach (var a in arr)
        {
            try
            {
                Glass g = a.GetComponentInChildren<Glass>();
                if (g.thisGlassType == type)
                {
                    return a;
                }
            }
            catch (MissingComponentException) { Debug.Log("Bitch what the fucK"); return null; }
        }
        return null;
    }

    public static GameObject FindAdditiveOfType(GameObject[] arr, Additive additive)
    {
        foreach (var a in arr)
        {
            try
            {
                AdditiveObject ao = a.GetComponentInChildren<AdditiveObject>();
                if (ao.thisAdditive == additive)
                {
                    return a;
                }
            } catch (System.NullReferenceException) { return null; }
        }

        return null;
    }

}

