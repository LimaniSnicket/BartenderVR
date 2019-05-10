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
    public TextMeshProUGUI tempTutorialText;

    public TextMeshPro MenuListText;

    public GameObject sA;
    public Light focusHighlight;
    public static GameObject servingArea;

    public static TutorialLine currentTutorialLine;

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
        try
        {
            var fader = FindObjectOfType<OVRScreenFade>();
            fader.fadeOnStart = true;
        }
        catch (System.NullReferenceException) { }
        GenerateMenu(DrinkResourcesPath);
        MenuListText.text = GenerateMenuDisplay(menuItems);
        tutDrink = tutorialDrink;
        s_debuggingMode = debuggingMode;
        currentTabs.Capacity = maxTabsToDisplay;
        servingArea = sA;
        prepTutorial = new DrinkPreparationTutorial(tutorialDrink);
        StartCoroutine(TutorialWalkthrough(prepTutorial));
        RunTutorial(prepTutorial);
    }

    private void Update()
    {
        if (!debuggingMode)
        {
            tutorialActive = false;
        }

        if (!tutorialActive)
        {
            timer += timeModifier * Time.deltaTime;
            tempTutorialText.text = (currentOrder.drinkToMake != null) ? tempTutorialText.text = "Now Serving: \n " + currentOrder.drinkToMake.drinkName : " ";
        }
        else
        {
            currentTutorialLine = prepTutorial.Tutorial;
            if (prepTutorial.Tutorial != null)
            {
                prepTutorial.Tutorial.CheckContinuation(prepTutorial.thisDrinkPrep);
                if (prepTutorial.Tutorial.focusGameObject != null)
                {
                    print(prepTutorial.Tutorial.focusGameObject.name);
                    prepTutorial.Tutorial.PulsateOutline(Color.green, prepTutorial.Tutorial.focusGameObject, 10f);   
                }

                tempTutorialText.text = prepTutorial.Tutorial.line;
                if (prepTutorial.Tutorial.nextTutorialStep == null)
                {
                    print("Last node of tutorial");
                    if (prepTutorial.Tutorial.CanContinue )//&& ReturnAdvanceInput())
                    {
                        prepTutorial.Tutorial = null;
                        print("End of tutorial");
                    }
                }
                else
                {
                    if (prepTutorial.Tutorial.CanContinue )//&& ReturnAdvanceInput())
                    {
                        print("Continuing");
                        prepTutorial.Tutorial = prepTutorial.Tutorial.nextTutorialStep;
                        Phone.SetTapFalse();
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
        else if(focusGlass == null || prepTutorial.Tutorial.focusGameObject == null)
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

   string GenerateMenuDisplay(List<Drink> menuList)
    {
        Drink[] items = menuList.ToArray();
        string[] names = new string[items.Length];
        for (int i =0; i<items.Length; i++)
        {
            names[i] = items[i].drinkName;
        }

        return string.Join("---", names);
    }

    public static bool CanSetNewFocusGlass()
    {
        if (focusGlass == null && !tutorialActive)
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

    bool ReturnAdvanceInput()
    {
        return Input.GetKeyDown(KeyCode.H) || Phone.Tapped;//OVRInput.GetDown(OVRInput.Button.One);
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
        new TutorialLine("Why pay a random shmuck to mix your drinks when you can do it in VR for free amiright?"),
        new TutorialLine("Anyway..."),
    };


    public void RunTutorial(DrinkPreparationTutorial tutorial)
    {
        tutorial.Tutorial.SetLinks(tutorialLines);
        tutorial.AddLinks(tutorial.Tutorial, new TutorialLine(PrepStart(tutorial.thisDrinkPrep)));
        //tutorial.AddLinks(tutorial.Tutorial,
        //new TutorialLine(string.Format("The first step to any drink is getting a glass. Go grab a <b>{0} </b> over there on the counter", tutorial.thisDrinkPrep.properGlass), 
         //   TutorialLine.LineConditions.RequireGlass));
        tutorial.AddLinks(tutorial.Tutorial, new TutorialLine("Ok cool, we made it this far! Maybe you're not 100% inept."));
        tutorial.AddLinks(tutorial.Tutorial, new TutorialLine(tutorial.StepTutorial(0)));
    }

    public string PrepStart(Drink d)
    {
        return string.Format("We're gonna whip up a nice <b> {0} </b> for our patron here.", d.drinkName);
    }

    public string PrepGlass(Drink d)
    {
        return string.Format("Go grab a <b>{0}</b> over there on the counter", d.properGlass);
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

    public string StepTutorial(int stepIndex)
    {
        return (string.Format("Now we need to <b>{0}</b> some <b>{1}</b> into this bitch!", thisDrinkPrep.recipe[stepIndex].additionMethod.ToString(), thisDrinkPrep.recipe[stepIndex].addedThisStep));
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

    public TutorialLine nextTutorialStep;
    public LineConditions lineCondition;
    public bool CanContinue;
    public GameObject focusGameObject;

    public float TimerCap = 30f;
    public float currentTime = 30f;

    public TutorialLine() { }
    public TutorialLine(string l)
    {
        line = l;
        lineCondition = LineConditions.Default;

    }
    public TutorialLine(string l, LineConditions lc)
    {
        line = l;
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

    public void CheckContinuation(Drink d)
    {
        switch (lineCondition)
        {
            case LineConditions.RequireGlass:
                CanContinue = GlassReady(d);
                focusGameObject = SpawnerManager.GetGameObjectReference(d.properGlass);
                break;

            case LineConditions.Default:
                CanContinue = Phone.Tapped;
                focusGameObject = Phone.phoneObject;
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

    public GameObject FocusOnGameObject(Drink d)
    {
        switch (lineCondition)
        {
            case LineConditions.RequireGlass:
                return SpawnerManager.GetGameObjectReference(d.properGlass);
            case LineConditions.Default:
                return null;
               
        }

        return null;
    }

    public enum LineConditions
    {
        Default=0,
        RequireGlass=1,
        RequireIngredient=2,
        RequireShake=3,
        RequireStir=4,
        RequireMuddle=5,
        GrabPhone=6,
        ReadyToServe=7,

    }

    public  void PulsateOutline(Color pulsateColor, GameObject toPulse, float pulseSpeed)
    {
        Outline ot = toPulse.GetComponentInChildren<Outline>();
       
        if (!CanContinue)
        {
            ot.OutlineColor = pulsateColor;
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime * Mathf.Pow(pulseSpeed, 1.5f);
                ot.OutlineWidth = currentTime;
            }
            else
            {
                currentTime = TimerCap;
            }
        }
        else
        {
            ot.OutlineColor = Color.clear;
            currentTime = 0;
            return;
        }
    }

}
