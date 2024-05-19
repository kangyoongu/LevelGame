using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinShopButton : MonoBehaviour
{
    [SerializeField] private int price;
    [SerializeField] private int coinCount;

    [SerializeField] private Transform coinPos;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI coinText;

    private void Start()
    {
        priceText.text = price + "$";
        coinText.text = coinCount.ToString();
    }

    public void OnClickBuy()
    {
        SuccessBuy();
    }

    private void SuccessBuy()
    {
        PublicAudio.Instance.click.Play();
        CoinEffect.Instance.Effect((int)(coinCount * 0.05f), coinCount, coinPos.position);
    }
}
