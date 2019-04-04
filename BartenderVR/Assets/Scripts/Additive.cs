using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Additive", menuName = "Add to Drink")]
public class Additive : ScriptableObject
{
    public List<EnumList.AdditionMethod> possibleAdditionMethods;
}
