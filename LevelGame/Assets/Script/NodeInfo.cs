using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NodeInfo : MonoBehaviour
{
    public int index;
    public int num = 0;
    public List<NodeInfo> neighbor;
    [HideInInspector] public VisualMove visualMove;
    [HideInInspector] public bool coinNode = false;
    public int blankIndex = 0;

    [HideInInspector] public GameObject coinObject;
    private void Start()
    {
        coinObject = NodeManager.Instance.coins.GetChild(transform.GetSiblingIndex()).gameObject;
    }
    private void Update()
    {
        if(num >= 1 && transform.childCount > 0)
        {
            if (visualMove == null)
            {
                visualMove = transform.GetChild(0).GetComponent<VisualMove>();
                GameManager.Instance.ApplyWarning();
            }
        }
        else
        {
            visualMove = null;
        }
    }

    public void OnCoin()
    {
        coinNode = true;
        coinObject.SetActive(true);
        coinObject.transform.DOScale(new Vector3(0.3f, 0.3f, 0.3f), 0.4f).SetEase(Ease.OutBack);
    }

    public void RemoveCoin()
    {
        coinNode = false;
        coinObject.transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InBack).OnComplete(() =>
        {
            coinObject.SetActive(false);
        });
    }
    public void EatCoin(float delay)
    {
        StartCoroutine(EatCoinCoroutine(delay));
    }
    IEnumerator EatCoinCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        coinNode = false;
        coinObject.transform.localScale = Vector3.zero;
        coinObject.SetActive(false);
        PublicAudio.Instance.coin.PlayOneShot(PublicAudio.Instance.coin.clip);
        QuestManager.Instance.Coin += Random.Range(5, 16);
    }
}
