using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Quest : MonoBehaviour
{
    public GameObject[] modeIcons;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI rewardText;
    public TextMeshProUGUI countText;
    public Image bar;
    public Image buttonImage;
    [HideInInspector] public QData questData = new QData();

    RectTransform rectTransform;
    public Image[] icons;
    public Image scoreBg;
    public Image modeImage;
    public Image image;
    public Image[] bomb;
    public Transform coin;
    private void Awake()
    {
        SetRectTransform();
    }
    public int Count { 
        get => questData.count;
        set {
            if (questData.state < 2)
            {
                questData.count = value;
                countText.text = $"{questData.count} / {questData.targetCount}";
                bar.fillAmount = (float)questData.count / questData.targetCount;
                if (questData.count >= questData.targetCount)
                {
                    questData.state = 1;
                    buttonImage.color = ThemeManager.Instance.CurrentTheme.ring;
                    buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 1);
                    QuestManager.Instance.SetGuide();
                }
            }
        }
    }

    public void Init(int mode, int score, int count, int targetCount, int reward, int state)
    {
        gameObject.SetActive(true);
        SetRectTransform();
        rectTransform.localScale = Vector3.one;
        rectTransform.sizeDelta = new Vector2(886.488f, 337.4f);
        questData.mode = mode;
        questData.score = score;
        questData.targetCount = targetCount;
        questData.reward = reward;
        questData.state = state;
        Count = count;
        if (state == 2)
        {
            gameObject.SetActive(false);
            return;
        }
        SetButtonThemeColor();
        scoreText.text = score.ToString();
        for (int i = 0; i < modeIcons.Length; i++)
        {
            modeIcons[i].SetActive(false);
        }
        modeIcons[mode].SetActive(true);
        rewardText.text = $"+    {reward}";
    }

    private void SetRectTransform()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
    }

    public void SetButtonThemeColor()
    {
        if (questData.state == 1)
        {
            buttonImage.color = ThemeManager.Instance.CurrentTheme.ring;
            buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 1f);
        }
        else
        {
            buttonImage.color = ThemeManager.Instance.CurrentTheme.x;
        }
        foreach(Image i in icons)
        {
            i.color = ThemeManager.Instance.CurrentTheme.icons;
        }
        foreach (Image i in bomb)
        {
            i.color = ThemeManager.Instance.CurrentTheme.scoreBack;
        }
        scoreText.color = ThemeManager.Instance.CurrentTheme.icons;
        rewardText.color = ThemeManager.Instance.CurrentTheme.scoreBack;
        countText.color = ThemeManager.Instance.CurrentTheme.scoreBack;
        scoreBg.color = ThemeManager.Instance.CurrentTheme.scoreBack;
        modeImage.color = ThemeManager.Instance.CurrentTheme.play;
        bar.color = ThemeManager.Instance.CurrentTheme.ad;
        image.color = ThemeManager.Instance.CurrentTheme.setting;
    }

    public void EndGame(int mode, int score)
    {
        if(mode == questData.mode && questData.score <= score)
        {
            Count++;
            JsonManager.Instance.SaveData();
        }
    }
    public void OnClickCheck()
    {
        if(questData.state == 1)
        {
            questData.state = 2;
            JsonManager.Instance.SaveData();
            PublicAudio.Instance.click.Play();
            CoinEffect.Instance.Effect((int)(questData.reward * 0.08f), questData.reward, coin.position);
            rectTransform.DOScaleY(0f, 0.4f);
            rectTransform.DOSizeDelta(new Vector2(rectTransform.sizeDelta.x, -54.3f), 0.5f).SetDelay(0.4f).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
            QuestManager.Instance.SetGuide();
        }
        else
        {
            PublicAudio.Instance.beebeeb.PlayOneShot(PublicAudio.Instance.beebeeb.clip);
        }
    }
}
