﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidColor : MonoBehaviour
{
    [System.Serializable]
    public struct Liquid
    {
        public Color colorOfLiquid;
        public float concentration;
    }

    Renderer rend;
    public Liquid[] colorsToMix;

    public Color rendColor;
    public float div;

    private void Start()
    {
        colorsToMix = new Liquid[10];
        rend = GetComponent<Renderer>();
        rend.material.SetFloat("_FillAmount", 1f);
    }

    private void Update()
    {
        rendColor = ColorMixer(colorsToMix);
        rend.material.SetColor("_Tint", rendColor);
        rend.material.SetFloat("_FillAmount", 1f - div);
    }


   
    public Color ColorMixer(Liquid[] colors)
    {
        Color Mixer = new Color(0,0,0,1);

        foreach (var c in colors)
        {
            if (c.concentration > 0)
            {
                Mixer += c.colorOfLiquid * Concentration(colors, c);
            }
        }

        return Mixer/ColorCount(colors);
    }

    int ColorCount(Liquid[] col)
    {
        int count = 0;
        for (int i = 0; i < col.Length; i++)
        {
            if (col[i].concentration > 0)
            {
                count++;
            }
        }

        return count;
    }

    public float Concentration(Liquid[] full, Liquid liquid)
    {
        float total = 0;

        for (int i = 0; i < full.Length; i++)
        {
            total += full[i].concentration;
        }

        if (total > 0)
        {
            return liquid.concentration / total;
        }

        return 0;
    }

    public void SetColorsToMix(Drink.RecipeStep[] recipe)
    {
        List<Drink.RecipeStep> l = new List<Drink.RecipeStep>();

        for (int j = 0; j < recipe.Length; j++)
        {
            if (recipe[j].additionMethod == EnumList.AdditionMethod.Pour && recipe[j].addedThisStep.AdditiveColor.strength > 0)
            {
                l.Add(recipe[j]);
            }
        }

        for (int i = 0; i < l.Count; i++)
        {
            colorsToMix[i].colorOfLiquid = l[i].addedThisStep.AdditiveColor.color;
            colorsToMix[i].concentration = l[i].amountToAdd;
        }
    }

}
