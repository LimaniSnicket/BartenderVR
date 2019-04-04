using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class EnumList : MonoBehaviour
{ 
   public enum AdditionMethod
    {
        None = 0,
        Add = 1,
        Stir = 2,
        Shake = 3,
        CoatRim = 4,
        Garnish = 5,
        Pour = 6,
        Muddle = 7
    }

    public enum GlassTypes
    {
        Basic = 0,
        Lowball = 1,
        Highball = 2,
        Martini = 3,
        Shot = 4
    }
}
