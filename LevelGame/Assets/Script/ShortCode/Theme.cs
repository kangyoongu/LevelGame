using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Theme : MonoBehaviour
{
    public int price;
    public TextMeshProUGUI priceText;
    public int num;
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClickBuy);
        priceText.text = price.ToString();
    }
    public void OnClickBuy()
    {
        if (transform.parent.name == "Buy")
        {
            Buy();
            //ShopScript.instance.NonConsumable_Press(Buy);
        }
        else
        {
            Whear();
        }
    }

    private void Buy()
    {
        if (QuestManager.Instance.Coin >= price)
        {
            QuestManager.Instance.Coin -= price;
            PlayerPrefs.SetInt("Theme" + num, 1);
            Whear();
        }
    }

    private void Whear()
    {
        PublicAudio.Instance.click.Play();
        PlayerPrefs.SetInt("Whear", num);
        ThemeManager.Instance.ApplyChange();
    }
}
