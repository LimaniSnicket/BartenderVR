using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Additive", menuName = "Add to Drink")]
public class Additive : ScriptableObject
{
    public List<EnumList.AdditionMethod> possibleAdditionMethods;
    public ColorStrength AdditiveColor;

    [System.Serializable]
    public struct ColorStrength
    {
        public Color color;
        public float strength;
    }
}
