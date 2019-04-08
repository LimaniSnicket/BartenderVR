using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public static class EnumList 
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

    static Dictionary<GlassTypes, float> GlassHeightModifiers = new Dictionary<GlassTypes, float>
    {
        {GlassTypes.Basic, 1f},
        {GlassTypes.Lowball, 1f},
        {GlassTypes.Highball, 10f},
        {GlassTypes.Martini, 5f},
        {GlassTypes.Shot, 1f}
    };

    public static float GlassHeightModifier(GlassTypes glass)
    {
        foreach (var key in GlassHeightModifiers)
        {
            if (key.Key == glass)
            {
                return key.Value;
            }
        }

        return 1f;
    }
}
