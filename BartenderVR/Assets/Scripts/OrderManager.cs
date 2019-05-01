using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using DrinkManagement;

public class OrderManager : MonoBehaviour
{
    private static OrderManager orderManager;

    public static Glass focusGlass;

    public static bool tutorialActive = true;

    public Drink tutorialDrink;
    public static Drink tutDrink;
    public DrinkPreparationTutorial prepTutorial;
    public TextMeshPro tempTutorialText;

    public GameObject sA;
    public Light focusHighlight;
    public static GameObject servingArea;

    public static float tipMoney;
    public float timer = 1;
    int check;

    [Range(0, 2)]
    public float timeModifier = 0.25f;

    const string DrinkResourcesPath = "DrinkMenu";

    public static List<Interactable> grabbedObjects = new List<Interactable>();

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

    static Queue<DrinkOrder> orderQueue = new Queue<DrinkOrder>();
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
        tutDrink = tutorialDrink;
        s_debuggingMode = debuggingMode;
        currentTabs.Capacity = maxTabsToDisplay;
        servingArea = sA;
        prepTutorial = new DrinkPreparationTutorial(tutorialDrink);
        StartCoroutine(TutorialWalkthrough(prepTutorial));
        //StartCoroutine(TutorialLineWalkthrough(prepTutorial));
        RunTutorial(prepTutorial);
    }

    private void Update()
    {
        if (!tutorialActive)
        {
            timer += timeModifier * Time.deltaTime;
            tempTutorialText.gameObject.SetActive(false);
        }
        else
        {

            if (prepTutorial.Tutorial != null)
            {
                prepTutorial.Tutorial.CheckContinuation(prepTutorial.thisDrinkPrep);
                print(prepTutorial.Tutorial.lineCondition);
                tempTutorialText.text = prepTutorial.Tutorial.line;
                if (prepTutorial.Tutorial.nextTutorialStep == null)
                {
                    print("Last node of tutorial");
                    if (prepTutorial.Tutorial.CanContinue && Input.GetKeyDown(KeyCode.H))
                    {
                        prepTutorial.Tutorial = null;
                        print("End of tutorial");
                    }
                }
                else
                {
                    print("Next tutorial step is not null");

                    if (prepTutorial.Tutorial.CanContinue && Input.GetKeyDown(KeyCode.H))
                    {
                        print("Continuing");
                        prepTutorial.Tutorial = prepTutorial.Tutorial.nextTutorialStep;
                    }
                }
            }
            else
            {
                tutorialActive = false;
            }
            print("Still in Tutorial Phase");
        }

        int menuSize = menuItems.Count;

        if (Mathf.FloorToInt(timer) > check)
        {
            check += 1;
            if (!s_debuggingMode)
            {
                orderQueue.Enqueue(new DrinkOrder(0f, GetRandomDrink(menuSize)));
            }
            else
            {
                orderQueue.Enqueue(new DrinkOrder(0f, GetDefaultDrink(0)));
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

        if (focusGlass != null)
        {
            SetFocusHighlight(focusGlass.parent, 0f);
        }
        else
        {
            focusHighlight.gameObject.SetActive(false);
        }

    }

    void SetFocusHighlight(GameObject over, float offset)
    {
        focusHighlight.gameObject.SetActive(true);
        Vector3 focusPos = new Vector3(over.transform.position.x, over.transform.position.y + offset, over.transform.position.z);
        focusHighlight.transform.position = focusPos;
        over.GetComponentInChildren<Outline>().OutlineColor = focusHighlight.color;
    }

    public static bool CanSetNewFocusGlass()
    {
        if (focusGlass == null)
        {
            return true;
        }

        return false;
    }

    public static void SetAsFocusGlass(Glass toset)
    {
        focusGlass = toset;
    }

    public static void FocusGlassNull()
    {
        focusGlass = null;
    }

    public static void UpdateQueue()
    {
        orderQueue.Dequeue();
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
            //Debug.Log("Adding " + d.name + " to the menu");
        }
    }

    public static void LeaveReview(float toAdd)
    {
        AccuracyHistory.Add(toAdd);
    }

    public IEnumerator TutorialWalkthrough(DrinkPreparationTutorial tutorial)
    {
        tutorial.EnqueueLines("fuck", "bitchh", "hahaha kill me", PrepStart(tutorial.thisDrinkPrep));
        tutorial.EnqueueLines("The first step in making any drink is getting a glass!",
        string.Format("Go grab a <b> {0} </b> over there on the counter.", tutorial.thisDrinkPrep.properGlass.ToString()));
        while (tutorial.tutorialLines.Count != 0)
        {
            tempTutorialText.gameObject.SetActive(true);
            //tempTutorialText.text = tutorial.tutorialLines.First();
            yield return new WaitForEndOfFrame();
        }

        tutorialActive = false;
        yield return null;
    }

    TutorialLine[] tutorialLines =
{
        new TutorialLine("Welcome to Shitty Bartender VR! The most immersive drinking experience your tiny budget can afford."),
        new TutorialLine("Why pay some random shmuck you don't know to mix your drinks when you can make pretend drinks in VR for free amiright?"),
        new TutorialLine("Anyway..."),
    };


    public void RunTutorial(DrinkPreparationTutorial tutorial)
    {
        tutorial.Tutorial.SetLinks(tutorialLines);
        tutorial.AddLinks(tutorial.Tutorial, new TutorialLine(PrepStart(tutorial.thisDrinkPrep)));
        tutorial.AddLinks(tutorial.Tutorial,
        new TutorialLine(string.Format("The first step to any drink is getting a glass. Go grab a <b>{0} </b> over there on the counter", tutorial.thisDrinkPrep.properGlass), 
            TutorialLine.LineConditions.RequireGlass));
    }

    public string PrepStart(Drink d)
    {
        return string.Format("We're gonna whip up a nice <b> {0} </b> for our patron over there.", d.drinkName);
    }

}

[System.Serializable]
public class DrinkPreparationTutorial
{
    public Drink thisDrinkPrep;
    public string drinkName;
    public int numberOfSteps;

    public Queue<string> tutorialLines = new Queue<string>();
    public TutorialLine Tutorial;
    string defaultLine = "Alright asshole, let's get this show on the road.";

    public DrinkPreparationTutorial() { }
    public DrinkPreparationTutorial(Drink learning)
    {
        thisDrinkPrep = learning;
        drinkName = learning.drinkName;
        numberOfSteps = learning.recipe.Count;
        Tutorial = new TutorialLine(defaultLine);
    }

    public void SetDefault()
    {
        Tutorial.SetLinks(new TutorialLine(defaultLine));
    }

    public void AddLinks(TutorialLine tutToCheck, TutorialLine tutToAdd)
    {
        TutorialLine current = tutToCheck;
        if (tutToCheck.nextTutorialStep != null)
        {
            AddLinks(tutToCheck.nextTutorialStep, tutToAdd);
        }
        else
        {
            tutToCheck.nextTutorialStep = tutToAdd;
            return;
        }
    }

    public void EnqueueLines(params string[] stringsToEnqueue)
    {
        for (int i = 0; i < stringsToEnqueue.Length; i++)
        {
            tutorialLines.Enqueue(stringsToEnqueue[i]);
        }
    }
}

[System.Serializable]
public class TutorialLine
{
    public string line;
    public List<bool> ConditionsToAdvance = new List<bool>();
    public bool[] advanceConditions = new bool[0];

    public TutorialLine nextTutorialStep;
    public LineConditions lineCondition;
    public bool CanContinue;

    public TutorialLine() { }
    public TutorialLine(string l, params bool[] conditions)
    {
        line = l;
        AddConditions(conditions);
        advanceConditions = new bool[conditions.Length];
        System.Array.Copy(conditions, advanceConditions, conditions.Length);
        lineCondition = LineConditions.Default;

    }
    public TutorialLine(string l, LineConditions lc, params bool[] conditions)
    {
        line = l;
        AddConditions(conditions);
        advanceConditions = new bool[conditions.Length];
        System.Array.Copy(conditions, advanceConditions, conditions.Length);
        lineCondition = lc;

    }

    public TutorialLine(string l, TutorialLine tl, LineConditions lc)
    {
        line = l;
        nextTutorialStep = tl;
        lineCondition = lc;
    }

    public void SetLinks(params TutorialLine[] lines)
    {
        for (int i = 0; i < lines.Length - 1; i++)
        {
            lines[i].nextTutorialStep = lines[i + 1];
        }

        nextTutorialStep = lines[0];
    }

    public void AddConditions(params bool[] conditions)
    {
        for (int i = 0; i < conditions.Length; i++)
        {
            ConditionsToAdvance.Add(conditions[i]);
        }
    }

    public bool Continue()
    {
        foreach (bool b in advanceConditions)
        {
            if (b == false)
            {
                Debug.Log("Cannot continue yet");
                return false;
            }
        }
        Debug.Log("Advance");
        return true;
    }

    public void CheckContinuation(Drink d)
    {
        switch (lineCondition)
        {
            case LineConditions.RequireGlass:
                CanContinue = GlassReady(d);
                break;

            case LineConditions.Default:
                CanContinue = true;
                break;
        }

    }

    public bool GlassReady(Drink d)
    {
        foreach (var g in OrderManager.grabbedObjects)
        {
            if (g.thisType == Interactable.InteractableType.Glass)
            {
                if (g.GetComponent<Glass>().thisGlassType == d.properGlass)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public enum LineConditions
    {
        Default=0,
        RequireGlass=1,
        RequireIngredient=2,
        RequireShake=3,
        RequireStir=4,
        RequireMuddle=5,
        ReadyToServe=6

    }



}
