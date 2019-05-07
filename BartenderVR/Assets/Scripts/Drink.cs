using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Drink", menuName = "Mixed Drink")]
public class Drink : ScriptableObject
{
    [System.Serializable]
    public struct RecipeStep
    {
        public Additive addedThisStep;
        public float amountToAdd;
        public EnumList.AdditionMethod additionMethod;
        public List<EnumList.AdditionMethod> methodsPerformedOn;

        public RecipeStep(Additive add, float amt, EnumList.AdditionMethod addMethod)
        {
            addedThisStep = add;
            amountToAdd = amt;
            additionMethod = addMethod;
            methodsPerformedOn = new List<EnumList.AdditionMethod>();
        }
    }

    public string drinkName;
    public float maxTip;
    public List<RecipeStep> recipe;
    public EnumList.GlassTypes properGlass;

    public float TotalInDrink()
    {
        float total = 0;
        foreach(var r in recipe)
        {
            total += r.amountToAdd;
        }
        return total;
    }

    public bool RecipeContainsIngredient(RecipeStep step)
    {
        foreach(var r in recipe)
        {
            if (r.addedThisStep == step.addedThisStep 
            && r.additionMethod == step.additionMethod)
            {
                return true;
            }
        }

        return false;
    }

    public RecipeStep FindStep(RecipeStep step)
    {

        foreach(var r in recipe)
        {
            if (r.addedThisStep == step.addedThisStep && r.additionMethod == step.additionMethod)
            {
                return r;
            }
        }

        return new RecipeStep(null, 0f, EnumList.AdditionMethod.None);
    }


    public float CheckAdditiveMethods(List<EnumList.AdditionMethod> baseList, RecipeStep checking)
    {
        List<EnumList.AdditionMethod> toParse = checking.methodsPerformedOn;

        if (baseList.Count <= 0)
        {
            if (toParse.Count <= 0)
            {
                return 1f;
            }
            else
            {
                return 0f;
            }
        }

        float accuracy = 0f;

        if (toParse.Count >= baseList.Count)
        {
            for (int i = 0; i < toParse.Count; i++)
            {
                if (baseList.Contains(toParse[i]))
                {
                    accuracy += 1f / toParse.Count;
                }
            }
        }
        else
        {
            for (int j = 0; j < baseList.Count; j++)
            {
                if (toParse.Contains(baseList[j]))
                {
                    accuracy += 1f / baseList.Count;
                }
            }
        }

        Debug.Log("Should perform: " + (baseList.Count) + " actions; Actually performed: " + (toParse.Count) + " actions. Accuracy = " + accuracy);

        return accuracy;
    }


    public float CorrectPercentage(float toCheck, float checkTotal, float trueValue, float trueTotal)
    {
        return Mathf.Abs((trueValue/trueTotal) - (toCheck/checkTotal));
    }

}
