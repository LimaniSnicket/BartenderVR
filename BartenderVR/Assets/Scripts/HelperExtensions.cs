using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DrinkManagement
{

    public static class HelperExtensions
    {
        public static void AddMethods(this Drink.RecipeStep[] steps, params EnumList.AdditionMethod[] toAdd)
        {
            foreach (var a in steps)
            {
                for (int i = 0; i < toAdd.Length; i++)
                {
                    if (!a.methodsPerformedOn.Contains(toAdd[i]))
                    {
                        a.methodsPerformedOn.Add(toAdd[i]);
                    }
                }
            }
        }

        public static float SumList(this List<float> floats)
        {
            float total = 0f;

            foreach (float f in floats)
            {
                total += f;
            }

            return total;
        }

        public static float Rating(this List<float> floats, float modifier)
        {
            float av = 0f;

            foreach (float f in floats)
            {
                av += f;
            }

            return (av / floats.Count) * modifier;
        }

        public static bool ArrayContains(this Drink.RecipeStep[] check, Additive additive)
        {
            foreach (var c in check)
            {
                if (c.addedThisStep == additive)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ArrayContains(this Drink.RecipeStep[] check, Additive additive, EnumList.AdditionMethod method)
        {
            foreach (var c in check)
            {
                if (c.addedThisStep == additive && c.additionMethod == method)
                {
                    return true;
                }
            }
            return false;
        }

        public static int GetNextEmptyIndex(this Drink.RecipeStep[] stepCheck)
        {
            if (stepCheck[0].addedThisStep != null)
            {
                for (int i = 0; i< stepCheck.Length; i++) { 
                    if(stepCheck[i].addedThisStep == null)
                    {
                        return i;
                    }
                }
            }
            return 0;
        }

        public static bool ArrayContains(this Object[] check, Object lookFor)
        {
            for (int i = 0; i < check.Length; i++)
            {
                if (check[i] == lookFor)
                {
                    return true;
                }
            }

            return false;
        }


        public static float GetAddedTotal(this Drink.RecipeStep[] added)
        {
            float total = 0;

            foreach (var r in added)
            {
                if (r.addedThisStep != null)
                {
                    total += r.amountToAdd;
                }
            }

            return total;
        }

        public static float GetAddedTotal(this Drink.RecipeStep[] added, EnumList.AdditionMethod addCheck)
        {
            float total = 0f;

            foreach (var a in added)
            {
                if (a.additionMethod == addCheck)
                {
                    total += a.amountToAdd;
                }
            }

            return total;
        }


        public static int GetAddedCount(this Drink.RecipeStep[] added)
        {
            int toReturn = 0;
            for (int i = 0; i < added.Length; i++)
            {
                if (added[i].addedThisStep != null)
                {
                    toReturn++;
                }
            }

            return toReturn;
        }

        public static int GetAddedCount(this Drink.RecipeStep[] added, EnumList.AdditionMethod addCheck)
        {
            int toReturn = 0;
            for (int i = 0; i < added.Length; i++)
            {
                if (added[i].addedThisStep != null && added[i].additionMethod == addCheck)
                {
                    toReturn++;
                }
            }

            return toReturn;
        }
    }
}
