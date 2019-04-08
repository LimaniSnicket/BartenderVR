using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Additive", menuName = "Add to Drink")]
public class Additive : ScriptableObject
{
    public List<EnumList.AdditionMethod> possibleAdditionMethods;
    public ColorStrength AdditiveColor;

    public enum AdditiveType
    {
        None = 0,
        Alcoholic = 1,
        Mixer = 2,
        Other = 3
    }

    public AdditiveType thisAdditiveType;

    [System.Serializable]
    public struct ColorStrength
    {
        public Color color;
        public float strength;
    }

    public bool IsLiquid()
    {
        if (thisAdditiveType == AdditiveType.Alcoholic || thisAdditiveType == AdditiveType.Mixer)
        {
            return true;
        }

        return false;
    }

    public bool IsAlcoholic()
    {
        if (thisAdditiveType == AdditiveType.Alcoholic)
        {
            return true;
        }

        return false;
    }
}
