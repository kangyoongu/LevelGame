using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinShopButton : MonoBehaviour
{
    [SerializeField] private bool ad;
    [SerializeField] private int price;
    [SerializeField] private int coinCount;

    [SerializeField] private Transform coinPos;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI coinText;

    private void Start()
    {
        if(ad == false)
            priceText.text = price + "$";
        coinText.text = coinCount.ToString();
    }

    public void OnClickBuy()
    {
        if (ad)
        {

        }
        else
        {

        }
        SuccessBuy();
    }

    private void SuccessBuy()
    {
        CoinEffect.Instance.Effect((int)(coinCount * 0.05f), coinCount, coinPos.position);
    }
}
