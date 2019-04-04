using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OrderManager : MonoBehaviour
{
    private static OrderManager orderManager;

    //the last glass you interacted with will always be the focus glass
    public static Glass focusGlass;

    public static float tipMoney;
    public float timer = 0;
    int check;

    [Range(0,2)]
    public float timeModifier = 0.25f;

    const string DrinkResourcesPath = "DrinkMenu";

    [System.Serializable]
    public struct DrinkOrder 
    {
        public float preparationTime;
        public Drink drinkToMake;

        public DrinkOrder(float pt, Drink dt)
        {
            preparationTime = pt;
            drinkToMake = dt;
        }
    }

    public static DrinkOrder currentOrder;

    public static bool s_debuggingMode;
    public bool debuggingMode;

    static List<DrinkOrder> orderQueue = new List<DrinkOrder>();
    List<Drink> menuItems = new List<Drink>();

    private void Awake()
    {
        orderManager = this;
    }

    private void Start()
    {
        GenerateMenu(DrinkResourcesPath);
        s_debuggingMode = debuggingMode;
    }

    private void Update()
    {
        timer += timeModifier * Time.deltaTime;

        int menuSize = menuItems.Count;

        if (Mathf.FloorToInt(timer) > check)
        {
            check += 1;
            if (!s_debuggingMode)
            {
                orderQueue.Add(new DrinkOrder(0f, GetRandomDrink(menuSize)));
            }
            else
            {
                orderQueue.Add(new DrinkOrder(0f, GetDefaultDrink(0)));
            }
        }

        if (orderQueue.Count > 0)
        {
            currentOrder.drinkToMake = orderQueue.ElementAt(0).drinkToMake;
            Debug.Log(currentOrder.drinkToMake);
        }

        if (currentOrder.drinkToMake != null)
        {
            currentOrder.preparationTime += Time.deltaTime;
        }
    }

    public static bool CanSetNewFocusGlass()
    {
        if (focusGlass.currentHoldingStatus != Interactable.HoldingStatus.NotHeld)
        {
            return false;
        }

        return true;
    }

    public static void UpdateQueue()
    {
        orderQueue.RemoveAt(0);
    }

    Drink GetDefaultDrink(int defaultIndex)
    {
        return menuItems.ElementAt(defaultIndex);
    }

    Drink GetRandomDrink(int size)
    {
        return menuItems.ElementAt(Mathf.FloorToInt(Random.Range(0, size)));
    }

    public void GenerateMenu(string resourcesFolderPath)
    {
        Object[] drinks = Resources.LoadAll(resourcesFolderPath, typeof(Drink));
        foreach (var d in drinks)
        {
            menuItems.Add((Drink)d);
            Debug.Log("Adding " + d.name + " to the menu");
        }
    }
}
