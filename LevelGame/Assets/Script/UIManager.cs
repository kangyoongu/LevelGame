using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;
using TMPro;

public enum Dir : short
{
    x,
    y
}
[Serializable]
public struct UI
{
    public RectTransform changeUI;
    public Image fadeUI;
    public TextMeshProUGUI fadeText;
    public Dir dir;
    public Vector2 inAndOut;
    public float time;
    public bool setActive;
    public float fadeFloat;
}
public class UIManager : MonoBehaviour
{
    public UI[] gameOverUI;
    public UI[] mainUI;
    public UI[] playUI;
    public UI[] survivalUI;

    public static UIManager instance;
    bool digree = false;
    public RectTransform digreePack;
    public Transform cam;

    public Sprite[] mute;
    public Image muteImage;
    
    private void Awake()
    {
        if (instance == null) instance = this;
        if (!PlayerPrefs.HasKey("Sound"))
        {
            PlayerPrefs.SetInt("Sound", 1);
        }
        MuteCheck();
    }

    public void GameOverUIIn()
    {
        In(gameOverUI);
        Out(playUI);
    }
    public void GameOverUIOut()
    {
        Out(gameOverUI);
    }
    public void MainUIIn()
    {
        cam.DOMoveY(-1.5f, 0.5f);
        In(mainUI);
    }

    public void MainUIOut()
    {
        cam.DOMoveY(0, 0.5f);
        Out(mainUI);
    }
    public void PlayUIIn()
    {
        In(playUI);
    }
    public void PlayUIOut()
    {
        Out(playUI);
        digreePack.DOScaleX(0, 0.4f).SetEase(Ease.InBack);
        digree = false;
    }
    public void SurvivalUIIn()
    {
        In(survivalUI);
    }
    public void SurvivalUIOut()
    {
        Out(survivalUI);
    }
    private void In(UI[] lst)
    {
        for (int i = 0; i < lst.Length; i++)
        {
            if (lst[i].changeUI != null)
            {
                if (lst[i].setActive) lst[i].changeUI.gameObject.SetActive(true);
                if (lst[i].dir == Dir.y) lst[i].changeUI.DOAnchorPosY(lst[i].inAndOut.x, lst[i].time).SetEase(Ease.Linear);
                else lst[i].changeUI.DOAnchorPosX(lst[i].inAndOut.x, lst[i].time).SetEase(Ease.Linear);
            }
            else if (lst[i].fadeUI != null)
            {
                if (lst[i].setActive) lst[i].fadeUI.gameObject.SetActive(true);
                lst[i].fadeUI.DOFade(lst[i].fadeFloat / 255f, lst[i].time).SetEase(Ease.Linear);
            }
            else
            {
                if (lst[i].setActive) lst[i].changeUI.gameObject.SetActive(true);
                lst[i].fadeText.DOFade(lst[i].fadeFloat / 255f, lst[i].time).SetEase(Ease.Linear);
            }
        }
    }

    private void Out(UI[] lst)
    {
        for (int i = 0; i < lst.Length; i++)
        {
            if (lst[i].changeUI != null)
            {
                if (lst[i].dir == Dir.y)
                {
                    lst[i].changeUI.DOAnchorPosY(lst[i].inAndOut.y, lst[i].time).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        if (lst[i].setActive) lst[i].changeUI.gameObject.SetActive(false);
                    });
                }
                else
                {
                    lst[i].changeUI.DOAnchorPosX(lst[i].inAndOut.y, lst[i].time).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        if (lst[i].setActive) lst[i].changeUI.gameObject.SetActive(false);
                    });
                }
            }
            else if (lst[i].fadeUI != null)
            {
                lst[i].fadeUI.DOFade(0, lst[i].time).SetEase(Ease.Linear).OnComplete(() =>
                {
                    if (lst[i].setActive) lst[i].fadeUI.gameObject.SetActive(false);
                });
            }
            else
            {
                lst[i].fadeText.DOFade(0, lst[i].time).SetEase(Ease.Linear).OnComplete(() =>
                {
                    if (lst[i].setActive) lst[i].changeUI.gameObject.SetActive(false);
                });
            }
        }
    }

    public void OnClickDigree()
    {
        digree = !digree;
        if (digree)
        {
            digreePack.DOScaleX(1, 0.4f).SetEase(Ease.OutBack);
        }
        else
        {
            digreePack.DOScaleX(0, 0.4f).SetEase(Ease.InBack);
        }
    }
    public void OnClickGoToMain()//메인화면으로 가는 버튼 누르면
    {
        GameOverUIOut();
        NodeManager.instance.RemoveVisual();
        StartCoroutine(NodeManager.instance.GoToMain());
    }
    public void OnClickAgain()//다시하기 누르면
    {
        GameOverUIOut();
        NodeManager.instance.RemoveVisual();
        StartCoroutine(AgainDelay());
    }
    IEnumerator AgainDelay()
    {
        yield return new WaitForSeconds(2f);
        PlayUIIn();
        StartCoroutine(NodeManager.instance.StartWork());
    }
    public void OnClickMute()
    {
        PlayerPrefs.SetInt("Sound", -PlayerPrefs.GetInt("Sound"));
        MuteCheck();
    }

    private void MuteCheck()
    {
        if (PlayerPrefs.GetInt("Sound") == 1)
        {
            muteImage.sprite = mute[0];
        }
        else
        {
            muteImage.sprite = mute[1];
        }
    }
}