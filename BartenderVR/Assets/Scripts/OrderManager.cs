using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using DrinkManagement;

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

    [System.Serializable]
    public struct OrderTab
    {
        public GameObject order;
        public TextMeshProUGUI orderDisplay;

        public OrderTab(DrinkOrder orderToFIll, GameObject tab, int QueueSpot)
        {
            order = tab;
            orderDisplay = tab.GetComponentInChildren<TextMeshProUGUI>();
            orderDisplay.text = orderToFIll.drinkToMake.drinkName + ": \n" + "Spot on Queue: " + QueueSpot;
        }
    }

    public static DrinkOrder currentOrder;

    public static bool s_debuggingMode;
    public bool debuggingMode;

    static List<DrinkOrder> orderQueue = new List<DrinkOrder>();
    List<Drink> menuItems = new List<Drink>();

    static List<OrderTab> currentTabs = new List<OrderTab>();
    public int maxTabsToDisplay;

    public static List<float> AccuracyHistory = new List<float>();
    public float BarRating;

    private void Awake()
    {
        orderManager = this;
    }

    private void Start()
    {
        GenerateMenu(DrinkResourcesPath);
        s_debuggingMode = debuggingMode;
        currentTabs.Capacity = maxTabsToDisplay;
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

        if (AccuracyHistory.Count > 0)
        {
            BarRating = AccuracyHistory.Rating(5f);
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

    public static void LeaveReview(float toAdd)
    {
        AccuracyHistory.Add(toAdd);
    }

}
