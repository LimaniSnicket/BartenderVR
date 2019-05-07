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
        Shot = 4, 
        OldFashioned = 5
    }

    public struct HeightMod {
        public float min, max;
        public HeightMod (float mn, float mx) { min = mn; max = mx; }
        public float height() { return max - min; }
    }

    static Dictionary<GlassTypes, float> GlassHeightModifiers = new Dictionary<GlassTypes, float>
    {
        {GlassTypes.Basic, 2f},
        {GlassTypes.Lowball, 2f},
        {GlassTypes.Highball, 10f},
        {GlassTypes.Martini, 3f},
        {GlassTypes.Shot, 2f},
        {GlassTypes.OldFashioned, 3f}
    };

    static Dictionary<GlassTypes, HeightMod> HeightMods = new Dictionary<GlassTypes, HeightMod>
    {
        {GlassTypes.Basic, new HeightMod(0.6f, 0.44f)}
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

    public static float MinHeight(GlassTypes glass)
    {
        foreach (var g in HeightMods)
        {
            if (g.Key == glass)
            {
                return g.Value.min;
            }
        }

        return 1f;
    }
}
