using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class CoinEffect : SingleTon<CoinEffect>
{
    public GameObject coinPref;
    Queue<RectTransform> coins = new Queue<RectTransform>();
    public int startCount;
    public Transform coinPos;
    void Start()
    {
        for(int i = 0; i < startCount; i++)
        {
            coins.Enqueue(SpawnCoin());
        }
    }

    private RectTransform SpawnCoin()
    {
        GameObject coin = Instantiate(coinPref, transform);
        coin.SetActive(false);
        return coin.GetComponent<RectTransform>();
    }

    public void Effect(int coinCoint, int addCoin, Vector3 pos)
    {
        StartCoroutine(DoEffect(coinCoint, addCoin, pos));
    }
    IEnumerator DoEffect(int count, int addCoin, Vector3 pos)
    {
        float delay = 1f / count;
        float onceAdd = (float)addCoin / count;
        float currentCoin = PlayerPrefs.GetInt("Coin");
        PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + addCoin);
        for(int i = 0; i < count; i++)
        {
            RectTransform coin;
            if (coins.Count > 0)
                coin = coins.Dequeue();
            else
                coin = SpawnCoin();

            coin.position = pos;
            coin.gameObject.SetActive(true);
            currentCoin += onceAdd;
            float textCoin = currentCoin;
            PublicAudio.Instance.coin.PlayOneShot(PublicAudio.Instance.coin.clip);
            coin.DOAnchorPos(coin.anchoredPosition + Random.insideUnitCircle * Random.Range(50f, 300f), 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                coin.DOMove(coinPos.position, 0.9f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    PublicAudio.Instance.tic.PlayOneShot(PublicAudio.Instance.tic.clip);
                    foreach (TextMeshProUGUI text in QuestManager.Instance.coinText)
                        text.text = Mathf.RoundToInt(textCoin).ToString("0");
                    coin.gameObject.SetActive(false);
                    coins.Enqueue(coin);
                });
            });
            yield return new WaitForSeconds(Random.Range(delay - 0.01f, delay + 0.005f));
        }
    }
}
