using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Additive", menuName = "Ingredient")]
public class Additive : ScriptableObject
{
    public EnumList.AdditionMethod additionMethod;
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

    public bool Grabbable;

    [System.Serializable]
    public struct LabelInformation
    {
        public Font fontDisplay;
        public Sprite labelImage;
        public Color labelColor;
    }
}
