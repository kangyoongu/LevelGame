using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockMap : MonoBehaviour
{
    [SerializeField] int speed;
    [SerializeField] Transform background;
    [SerializeField] Transform icon;
    [SerializeField] GameObject[] modeIcons;
    private void OnEnable()
    {
        StartCoroutine(Effect());
    }
    IEnumerator Effect()
    {
        yield return new WaitForSeconds(1.5f);
        background.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack);
        icon.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack).SetDelay(0.3f);
        icon.DOScale(Vector3.zero, 1f).SetEase(Ease.InBack).SetDelay(2.7f);
        background.DOScale(Vector3.zero, 1f).SetEase(Ease.InBack).SetDelay(3f).OnComplete(() =>
        {
            UIManager.Instance.StageClearUIIn();
            gameObject.SetActive(false);
        });
    }
    private void Update()
    {
        background.Rotate(Vector3.forward * speed * Time.deltaTime);
    }
    public void SetModeIcon(int index)
    {
        for(int i = 0; i < modeIcons.Length; i++)
        {
            modeIcons[i].SetActive(false);
        }
        modeIcons[index].SetActive(true);
        gameObject.SetActive(true);
    }
}
