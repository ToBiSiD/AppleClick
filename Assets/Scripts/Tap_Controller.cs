using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tap_Controller : MonoBehaviour
{
    [Header("Music")]
    public AudioSource music;

    [Header("Показывает яблоки")]
    public Text apples;

    [Header("Показывает spikes")]
    public Text Spikes;

    [Header("ShopPanel")]
    public GameObject Shop;

    [Header("Кнопки товаров")]
    public Button[] shopButtons;

    [Header("Текст кнопок")]
    public Text[] ButText;

    [Header("Shop")]
    public List<Product> ShopProducts = new List<Product>();

    [Header("Pony")]
    public GameObject Pony;
    [Header("Tree")]
    public GameObject Tree;

    public static int bonus = 1;// click bonus
    private int SpikeCount = 0;

    public static Save save = new Save();

    public void Awake()
    {
        if (PlayerPrefs.HasKey("sv"))
        {
            save = JsonUtility.FromJson<Save>(PlayerPrefs.GetString("sv"));
            Tree.gameObject.GetComponent<Game>().apple_score = save.apples;

            for (int i =0; i<ShopProducts.Count ;i++)
            {
                ShopProducts[i].lvlOfProduct = save.lvlOfItem[i];
                ShopProducts[i].bonusCount = save.spikes[i];
                if (ShopProducts[i].bonusCount > 0)
                    SpikeCount = ShopProducts[i].bonusCount;
                if (ShopProducts[i].BoolCostMultiplier)
                {
                    ShopProducts[i].cost *= (int)Mathf.Pow(ShopProducts[i].CostMultiplier, ShopProducts[i].lvlOfProduct);
                }
                if (ShopProducts[i].BonusClick != 0)
                    bonus += (ShopProducts[i].BonusClick * ShopProducts[i].lvlOfProduct);
            }
            DateTime Date = new DateTime(save.Date[0], save.Date[1], save.Date[2], save.Date[3], save.Date[4], save.Date[5]);
            TimeSpan appsent = DateTime.Now - Date;
            int doxod = (int)appsent.TotalSeconds * SpikeCount;
            Tree.gameObject.GetComponent<Game>().apple_score += doxod;
            print("Appsent " + appsent.TotalSeconds + "sec");
            print("DOxod: " + doxod);
        }
    }
    private void Start()
    {
        music.Play();
        CostsUpdate();
        StartCoroutine(BonusFromSpike());
    }
    public void Update()
    {
        apples.text = Tree.gameObject.GetComponent<Game>().apple_score.ToString();
        Spikes.text = SpikeCount.ToString();
    }

    public void ButtonPresed(int index)
    {
        int cost = ShopProducts[index].cost * ShopProducts[ShopProducts[index].ProductIndex].bonusCount;// цена в зависимости от спайков 
        if (ShopProducts[index].BoolTimeBonus && Tree.gameObject.GetComponent<Game>().apple_score >= ShopProducts[index].cost)
        {
            if (cost > 0)
            {
                Tree.gameObject.GetComponent<Game>().apple_score -= ShopProducts[index].cost * ShopProducts[ShopProducts[index].ProductIndex].bonusCount;
                StartCoroutine(BonusTime(ShopProducts[index].TimeOfBonus, index));//Start bonus timer
                Debug.Log("SpikeX2");
            }

            else print("ZERO Spikes");
        }
        else if (Tree.gameObject.GetComponent<Game>().apple_score >= ShopProducts[index].cost)
        {
            if (ShopProducts[index].BoolPerSecond)
            {
                ShopProducts[index].bonusCount++;
                SpikeCount = ShopProducts[index].bonusCount;
            }
            else bonus += ShopProducts[index].BonusClick;
            Tree.gameObject.GetComponent<Game>().apple_score -= ShopProducts[index].cost;

            if (ShopProducts[index].BoolCostMultiplier)
                ShopProducts[index].cost *= ShopProducts[index].CostMultiplier;
            ShopProducts[index].lvlOfProduct++;
        }
        else print("Not enought apples");
        CostsUpdate();
    } 

    private void CostsUpdate()
    {
        for(int i = 0;i<ShopProducts.Count;i++)
        {
            if (ShopProducts[i].BoolTimeBonus)
            {
                int cost = ShopProducts[i].cost * ShopProducts[ShopProducts[i].ProductIndex].bonusCount;
                ButText[i].text = ShopProducts[i].name + "\n Cost: " + cost;
            }

            else ButText[i].text = ShopProducts[i].name + "\n Cost: " + ShopProducts[i].cost;
        }
    }


    public void ShopVisible()
    {
        Shop.SetActive(!Shop.activeSelf);
        Pony.SetActive(!Pony.activeSelf);
        Tree.SetActive(!Tree.activeSelf);
        Debug.Log("work");
    }

    IEnumerator BonusTime(float time,int index)
    {
        shopButtons[index].interactable = false;
        ShopProducts[ShopProducts[index].ProductIndex].bonusPerSecond *= ShopProducts[index].TimeMultiplier;
        yield return new WaitForSeconds(time);
        ShopProducts[ShopProducts[index].ProductIndex].bonusPerSecond /= ShopProducts[index].TimeMultiplier;
        shopButtons[index].interactable = true;
    }

   
    IEnumerator BonusFromSpike()
    {
        while (true)
        {
            for(int i = 0;i< ShopProducts.Count;i++)
            Tree.gameObject.GetComponent<Game>().apple_score += (ShopProducts[i].bonusCount*ShopProducts[i].bonusPerSecond);
            
            yield return new WaitForSeconds(1);
        }
        
    }

#if UNITY_ANDROID && UNITY_EDITOR
    private void OnApplicationPause(bool pause)
    {
        if(pause)
        {
            save.apples = Tree.gameObject.GetComponent<Game>().apple_score;
            save.lvlOfItem = new int[ShopProducts.Count];
            save.spikes = new int[ShopProducts.Count];
            for (int i = 0; i < ShopProducts.Count; i++)
            {
                save.lvlOfItem[i] = ShopProducts[i].lvlOfProduct;
                save.spikes[i] = ShopProducts[i].bonusCount;
            }
            save.Date[0] = DateTime.Now.Year;
            save.Date[1] = DateTime.Now.Month;
            save.Date[2] = DateTime.Now.Day;
            save.Date[3] = DateTime.Now.Hour;
            save.Date[4] = DateTime.Now.Minute;
            save.Date[5] = DateTime.Now.Second;
            PlayerPrefs.SetString("sv", JsonUtility.ToJson(save));
            Debug.Log("save here");
        }
    }
#else
    private void OnApplicationQuit()
    {
        save.apples = Tree.gameObject.GetComponent<Game>().apple_score;
        save.lvlOfItem = new int[ShopProducts.Count];
        save.spikes = new int[ShopProducts.Count];
        for(int i = 0;i<ShopProducts.Count;i++)
        {
            save.lvlOfItem[i] = ShopProducts[i].lvlOfProduct;
            save.spikes[i] = ShopProducts[i].bonusCount;
        }
        save.Date[0] = DateTime.Now.Year;
        save.Date[1] = DateTime.Now.Month;
        save.Date[2] = DateTime.Now.Day;
        save.Date[3] = DateTime.Now.Hour;
        save.Date[4] = DateTime.Now.Minute;
        save.Date[5] = DateTime.Now.Second;
        PlayerPrefs.SetString("sv", JsonUtility.ToJson(save));
    }
#endif


}

[Serializable]
public class Product  //товар
{
    [Tooltip("Button title")]
    public string name;
    [Tooltip("Product`s cost")]
    public int cost;
    [Tooltip("+Bonus OnClick")]
    public int BonusClick;

    [HideInInspector]
    public int lvlOfProduct;
    [Space]
    [Tooltip("Нужен ли множитель на цену")]
    public bool BoolCostMultiplier;
    [Tooltip("Mножитель на цену")]
    public int CostMultiplier;

    [Space]
    [Tooltip("Bonus per second?")]
    public bool BoolPerSecond;
    [Tooltip("_Bonus per second_")]
    public int bonusPerSecond;

    [HideInInspector]
    public int bonusCount;// kolvo bonuses
    [Space]
    [Tooltip("TimeBonus?")]
    public bool BoolTimeBonus;
    [Tooltip("TimeBonus`s Multiplier")]
    public int TimeMultiplier;
    [Tooltip("Product`s Index")]
    public int ProductIndex;
    [Tooltip("Time of Bonus")]
    public float TimeOfBonus;
}

[Serializable]
public class Save
{
    public int apples;
    public int[] lvlOfItem;
    public int[] spikes;
    public int[] Date = new int[6];
}