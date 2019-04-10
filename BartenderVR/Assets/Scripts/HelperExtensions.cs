using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DrinkManagement
{
    //honestly, we learned how to make extensions in intermediate programming on 4/05 and i went to town with it

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

        //check the arrays for specific ingredients
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

        //check the arrays for different ingredients and also specific method in which they were added?
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

        public static bool ContainerEmpty(this Drink.RecipeStep[] check)
        {
            if (check.GetAddedCount() == 0)
            {
                return true;
            }

            return false;
        }

        //Get the next empty slot in the recipe step arrays
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

        public static void AddStepsToArray(this Drink.RecipeStep[] toAddto, Drink.RecipeStep toAdd)
        {
            if (toAddto.ArrayContains(toAdd.addedThisStep))
            {
                int index = toAddto.GetAdditiveIndex(toAdd.addedThisStep);
                toAddto[index].amountToAdd += toAdd.amountToAdd;
            }
            else
            {
                int index = toAddto.GetNextEmptyIndex();
                toAddto[index] = toAdd;
            }
        }

        public static Drink.RecipeStep NullStep()
        {
            return new Drink.RecipeStep(null, 0f, EnumList.AdditionMethod.None);
        }

        public static void RemoveFromArray(this Drink.RecipeStep[] toRemoveFrom, Additive toRemove)
        {
            if (toRemoveFrom.ArrayContains(toRemove))
            {
                int indexOfRemoval = toRemoveFrom.GetAdditiveIndex(toRemove);
                if (toRemoveFrom[indexOfRemoval].amountToAdd > 1)
                {
                    toRemoveFrom[indexOfRemoval].amountToAdd -= 1f;
                }
                else
                {
                    toRemoveFrom[indexOfRemoval] = NullStep();
                }
            }
            else
            {
                return;
            }

        }

        //Get the index of a specific ingredient in the array
        public static int GetAdditiveIndex(this Drink.RecipeStep[] stepCheck, Additive additive)
        {
            for (int i =0; i< stepCheck.Length; i++)
            {
                if (stepCheck[i].addedThisStep == additive)
                {
                    return i;
                }
            }

            return -1;
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

        //Get the total amount of stuff or something in the array
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
        //Get the total amount of stuff added in a specific way in the array
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

        //Get the amount of stuff in the array, different from the length
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

        public static Interactable GetInteractableType(this Interactable interactable)
        {
            switch (interactable.thisType)
            {
                case Interactable.InteractableType.Glass:
                    return interactable.GetComponent<Glass>();
                case Interactable.InteractableType.Shaker:
                    return interactable.GetComponent<CocktailShaker>();
                case Interactable.InteractableType.Additive:
                    return interactable.GetComponent<AdditiveObject>();
            }
            return null;
        }

        public static Transform TargetTransform(this Dictionary<Transform, EnumList.AdditionMethod> dict, EnumList.AdditionMethod addMethod)
        {
            foreach (var k in dict)
            {
                if (k.Value == addMethod && k.Key.childCount == 0)
                {
                    return k.Key;
                }
            }

            return null;
        }

        public static bool TransformValid(this Dictionary<Transform, EnumList.AdditionMethod> dict, EnumList.AdditionMethod additionMethod)
        {
            if (dict.ContainsValue(additionMethod))
            {
                return true;
            }

            return false;
        }

        public static bool CheckRotationThreshold(this float rotationAxis, float threshold)
        {
            float rotation = (rotationAxis <= 180) ? Mathf.Abs(rotationAxis) : 360 - rotationAxis;

            if (rotation >= threshold)
            {
                return true;
            }

            return false;
        }
    }
}
