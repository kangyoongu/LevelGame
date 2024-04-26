using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Theme : MonoBehaviour
{
    public int price;
    public int num;

    Transform parent;
    private void Start()
    {
        parent = transform.parent;
        GetComponent<Button>().onClick.AddListener(OnClickBuy);
    }
    public void OnClickBuy()
    {
        if (parent.name == "Buy")
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
        PlayerPrefs.SetInt("Theme" + num, 1);
        Whear();
    }

    private void Whear()
    {
        PlayerPrefs.SetInt("Whear", num);
        ThemeManager.instance.ApplyChange();
    }
}
