using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class QuestManager : SingleTon<QuestManager>
{
    public Quest[] quests;
    public TextMeshProUGUI[] coinText;
    public TextMeshProUGUI[] rewardText;
    public TextMeshProUGUI timeText;
    public GameObject guide;
    [SerializeField] int maxReward = 5;
    public int Coin
    {
        get => PlayerPrefs.GetInt("Coin");
        set
        {
            PlayerPrefs.SetInt("Coin", value);
            foreach (TextMeshProUGUI text in coinText)
                text.text = value.ToString("");
        }
    }
    public int RewardCount
    {
        get => PlayerPrefs.GetInt("RewardCount");
        set
        {
            PlayerPrefs.SetInt("RewardCount", value);
            foreach (TextMeshProUGUI text in rewardText)
                text.text = $"{value}/{maxReward}";
        }
    }
    private void Awake()
    {
        if (!PlayerPrefs.HasKey("Coin"))
        {
            PlayerPrefs.SetInt("Coin", 0);
        }
        //PlayerPrefs.SetInt("Day", DateTime.Now.Day-1);
        Coin = Coin;
        RewardCount = RewardCount;
    }
    private void Start()
    {
        for(int i = 0; i < quests.Length; i++)
        {
            QData qd = JsonManager.Instance.questData.datas[i];
            quests[i].Init(qd.mode, qd.score, qd.count, qd.targetCount, qd.reward, qd.state);
        }
    }
    private void Update()
    {
        if(DateTime.Now.Day != PlayerPrefs.GetInt("Day"))
        {
            RandomQuest();
            RewardCount = 0;
            PlayerPrefs.SetInt("Day", DateTime.Now.Day);
        }
        timeText.text = "Next " + GetTimeUntilNextDay();
    }
    public void RandomQuest()
    {
        for(int i = 0; i < quests.Length; i++)
        {
            quests[i].Init(Random.Range(0, 4), Random.Range(4, 5) * 10, 0, Random.Range(2, 5), Random.Range(10, 31) * 10, 0);
        }
        JsonManager.Instance.SaveData();
    }
    public void EndGame(int mode, int score)
    {
        for(int i = 0; i < quests.Length; i++)
        {
            quests[i].EndGame(mode, score);
        }
    }
    private void OnEnable()
    {
        ThemeManager.Instance.OnChangeTheme += SetButtonsThemeColor;
    }
    private void OnDisable()
    {
        ThemeManager.Instance.OnChangeTheme -= SetButtonsThemeColor;
    }
    private void SetButtonsThemeColor()
    {
        for(int i= 0; i < quests.Length; i++)
        {
            quests[i].SetButtonThemeColor();
        }
    }
    string GetTimeUntilNextDay()
    {
        DateTime currentTime = DateTime.Now;

        DateTime nextMidnight = currentTime.Date.AddDays(1);

        TimeSpan timeUntilNextDay = nextMidnight - currentTime;

        string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                    timeUntilNextDay.Hours,
                                    timeUntilNextDay.Minutes,
                                    timeUntilNextDay.Seconds);

        return formattedTime;
    }
    public void SetGuide()
    {
        bool off = true;
        for (int i = 0; i < quests.Length; i++)
        {
            if(quests[i].questData.state == 1)
            {
                off = false;
                break;
            }
        }
        guide.SetActive(!off);
    }
    public void OnClickRewardAd(Transform effectPos)
    {
        if (RewardCount < maxReward)
        {
            RewardCount++;
            SuccessReward(effectPos);
        }
        else
        {
            PublicAudio.Instance.beebeeb.PlayOneShot(PublicAudio.Instance.beebeeb.clip);
        }
    }
    private void SuccessReward(Transform effectPos)
    {
        PublicAudio.Instance.click.Play();
        CoinEffect.Instance.Effect(10, 70, effectPos.position);
    }
}
