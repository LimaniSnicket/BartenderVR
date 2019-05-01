using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class ReviewManager : MonoBehaviour
{
    private static ReviewManager review;
    string path, jsonData;
    [HideInInspector]
    public static ReviewerData jsonReviewData;
    public GameObject RV;
    public GameObject EP;

    static GameObject ReviewViewport;
    static GameObject EntryPrefab;

    public static List<Review> ReviewsLeft = new List<Review>();

    [System.Serializable]
    public struct ReviewEntry
    {
        public GameObject EntryGameObject;
        public Image ProfileImage;
        public TextMeshProUGUI infoTMP, NameTMP, ReviewTMP, DateTMP;
        public Slider RatingLeft;

        public ReviewEntry(Review parse, GameObject instantiateAs)
        {
            EntryGameObject = instantiateAs;
            ProfileImage = instantiateAs.GetImageFromTransform("ProfilePicture");
            ProfileImage.sprite = parse.UserProfilePicture;

            infoTMP = instantiateAs.GetTMPFromTransform("Ratings");
            infoTMP.text = parse.ReviewerTown.Bold() + "\n" + parse.NumberOfFriends + " Friends \n" + parse.NumberOfReviews + " Reviews";
            NameTMP = instantiateAs.GetTMPFromTransform("Name");
            NameTMP.text = parse.ReviewerName.Bold() + "<size=50%> says...";
            ReviewTMP = instantiateAs.GetTMPFromTransform("Rating");
            ReviewTMP.text = parse.WrittenReview;
            DateTMP = instantiateAs.GetTMPFromTransform("DateTime");
            DateTMP.text = parse.TimeOfReview;
            RatingLeft = instantiateAs.transform.GetComponentInChildren<Slider>();
            RatingLeft.value = parse.rating;//Random.Range(0, 5f); //test ratings for now

        }
    }

    private void Awake()
    {
        review = this;
        path = Application.streamingAssetsPath + "/ReviewDataBase.json";
        jsonData = File.ReadAllText(path);
        jsonReviewData = JsonUtility.FromJson<ReviewerData>(jsonData);
        ReviewViewport = RV;
        EntryPrefab = EP;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && OrderManager.s_debuggingMode)
        {
            CreateNewReview(OrderManager.tutDrink, 0f);
        }
    }

    public static void CreateNewReview(Drink drink, float accuracy)
    {
        Review newReview = new Review(jsonReviewData.FirstNames, jsonReviewData.Hometowns, drink, accuracy);
        GameObject e = Instantiate(EntryPrefab, ReviewViewport.transform);
        e.transform.SetAsFirstSibling();
        ReviewEntry rev = new ReviewEntry(newReview, e);
        ReviewsLeft.Add(newReview);
    }
}

[System.Serializable]
public class ReviewerData
{
    public string[] FirstNames;
    public string[] Hometowns;
}

[System.Serializable]
public class Review
{
    public string ReviewerName;
    public string ReviewerTown;
    public int NumberOfFriends;
    public int NumberOfReviews;

    public string WrittenReview;
    public Sprite UserProfilePicture;
    public string TimeOfReview;

    int maxFriends = 300;
    int maxReviews = 100;

    public float rating;

    public Review() { }
    public Review(string[] fn, string[] ht, Drink drink, float acc)
    {
        ReviewerName = fn.PullRandomString() + " " + RandomInitial();
        ReviewerTown = ht.PullRandomString();
        NumberOfFriends = maxFriends.RandomFromMax();
        NumberOfReviews = maxReviews.RandomFromMax();
        UserProfilePicture = Resources.Load<Sprite>("ProfilePics/TestPic");
        WrittenReview = GenerateReview(drink, acc);//"The bartender was on their phone the whole time, 1/5";//"This bar is bad and you should feel bad.";
        TimeOfReview = System.DateTime.Now.ToString();
        rating = acc;
    }

    public static string RandomInitial()
    {
        char ch = (char)('A' + Mathf.FloorToInt(Random.Range(0f, 26f)));
        return ch + ".";
    }

    public string GenerateReview()
    {
        return "Meh.";
    }

    public string GenerateReview(Drink order, float accuracy)
    {
        string drinkName = order.drinkName;
        string begin = "My " + drinkName + " was ";
        string end = "";

        if (accuracy.SqueezeFloat(-1f,1f))
        {
            end = " completely wrong!!! The bartender is stupid or something!";
        } 
        else if (accuracy.SqueezeFloat(1f, 3f))
        {
            end = " mediocre at best. Absolutely nothing to write home about. The stale white bread of mixed drinks.";
        } 
        else if (accuracy.SqueezeFloat(3f, 4f))
        {
            end = " pretty alright! Not perfect but can't complain too much.";
        }
        else if (accuracy >4f)
        {
            end = " perfect! Bartender made it just the way I like it";
        }
        else
        {
            end = " absolutely terrible! Completely wrong! The bartender should be fired! What an idiot!!";
        }

        return begin + end;
    }
}

static class ReviewHelperFunctions
{
    public static string PullRandomString(this string[] generateFrom)
    {
        return generateFrom[Mathf.FloorToInt(Random.Range(0, generateFrom.Length - 1))];
    }

    public static int RandomFromMax(this int max)
    {
        return Mathf.FloorToInt(Random.Range(0, max));
    }

    public static string Bold(this string toBold)
    {
        return "<b>" + toBold + "</b>";
    }

    public static Image GetImageFromTransform(this GameObject go, string n)
    {
        return go.transform.Find(n).GetComponent<Image>();
    }

    public static TextMeshProUGUI GetTMPFromTransform(this GameObject go, string n)
    {
        return go.transform.Find(n).GetComponent<TextMeshProUGUI>();
    }

    public static bool SqueezeFloat(this float squeeze, float min, float max)
    {
        if (squeeze > min && squeeze <= max)
        {
            return true;
        }

        return false;
    }

}
