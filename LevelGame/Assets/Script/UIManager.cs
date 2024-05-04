using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public enum GameMode : short
{
    Stage,
    Infinity,
}
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
public class UIManager : SingleTon<UIManager>
{
    public UI[] gameOverUI;
    public UI[] mainUI;
    public UI[] playUI;
    public UI[] stagePlayUI;
    public UI[] survivalUI;
    public UI[] themeUI;
    public UI[] menuUI;
    public UI[] stageUI;
    public UI[] selectModeUI;
    public UI[] stageClearUI;
    public UI[] stageFailUI;

    bool digree = false;
    public RectTransform digreePack;
    //public Transform cam;

    public Sprite[] mute;
    public Image muteImage;

    public GameObject[] block;

    [Header("Sounds")]
    public AudioSource mainSound;
    public AudioSource gameoverSound;

    public AudioMixer mixer;

    public GameObject ad;
    private void Awake()
    {
        //PlayerPrefs.DeleteAll();
        if (!PlayerPrefs.HasKey("Sound"))
        {
            PlayerPrefs.SetInt("Sound", 1);
        }
        if (!PlayerPrefs.HasKey("Ad"))
        {
            PlayerPrefs.SetInt("Ad", 1);
        }
    }
    private void Start()
    {
        //if (PlayerPrefs.GetInt("Ad") == 0) ad.SetActive(false);
        MuteCheck();
    }
    public void GameOverUIIn()
    {
        gameoverSound.Play();
        In(gameOverUI);
        Out(playUI);
    }
    public void GameOverUIOut() => Out(gameOverUI);
    public void MainUIIn()
    {
        mainSound.Play();
        //cam.DOMoveY(-1.45f, 0.5f);
        In(mainUI);
    }
    public void MainUIOut() => Out(mainUI);
        //cam.DOMoveY(0, 0.5f);
    public void PlayUIIn() => In(playUI);
    public void PlayUIOut()
    {
        Out(playUI);
        digreePack.DOScaleX(0, 0.4f).SetEase(Ease.InBack);
        digree = false;
    }
    public void StagePlayUIIn() => In(stagePlayUI);
    public void StagePlayUIOut()
    {
        Out(stagePlayUI);
        digreePack.DOScaleX(0, 0.4f).SetEase(Ease.InBack);
        digree = false;
    }
    public void SurvivalUIIn() => In(survivalUI);
    public void SurvivalUIOut() => Out(survivalUI);
    public void ThemeUIIn() => In(themeUI);
    public void ThemeUIOut() => Out(themeUI);
    public void MenuUIIn() => In(menuUI);
    public void MenuUIOut() => Out(menuUI);
    public void StageUIIn() => In(stageUI);
    public void StageUIOut() => Out(stageUI);
    public void SelectModeUIIn() => In(selectModeUI);
    public void SelectModeUIOut() => Out(selectModeUI);
    public void StageClearUIIn() => In(stageClearUI);
    public void StageClearUIOut() => Out(stageClearUI);
    public void StageFailUIIn() => In(stageFailUI);
    public void StageFailUIOut() => Out(stageFailUI);
    private void In(UI[] lst)
    {
        block[0].SetActive(true);
        float max = 0;
        for (int i = 0; i < lst.Length; i++)
        {
            if (max < lst[i].time) max = lst[i].time;
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
        StartCoroutine(BlockTime(max));
    }

    private void Out(UI[] lst)
    {
        block[0].SetActive(true);
        float max = 0;
        for (int i = 0; i < lst.Length; i++)
        {
            if (max < lst[i].time) max = lst[i].time;
            int index = i;
            if (lst[i].changeUI != null)
            {
                if (lst[i].dir == Dir.y)
                {
                    lst[i].changeUI.DOAnchorPosY(lst[i].inAndOut.y, lst[i].time).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        if (lst[index].setActive) lst[index].changeUI.gameObject.SetActive(false);
                    });
                }
                else
                {
                    lst[i].changeUI.DOAnchorPosX(lst[i].inAndOut.y, lst[i].time).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        if (lst[index].setActive) lst[index].changeUI.gameObject.SetActive(false);
                    });
                }
            }
            else if (lst[i].fadeUI != null)
            {
                lst[i].fadeUI.DOFade(0, lst[i].time).SetEase(Ease.Linear).OnComplete(() =>
                {
                    if (lst[index].setActive) lst[index].fadeUI.gameObject.SetActive(false);
                });
            }
            else
            {
                lst[i].fadeText.DOFade(0, lst[i].time).SetEase(Ease.Linear).OnComplete(() =>
                {
                    if (lst[index].setActive) lst[index].changeUI.gameObject.SetActive(false);
                });
            }
        }
        StartCoroutine(BlockTime(max));
    }
    IEnumerator BlockTime(float time)
    {
        yield return new WaitForSeconds(time);
        block[0].SetActive(false);
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
        MenuUIOut();
        StageClearUIOut();
        StageFailUIOut();
        StartCoroutine(NodeManager.Instance.ResetNodes(() =>
        {
            NodeManager.Instance.Score = 0;
            MainUIIn();
            PlayUIOut();
            StagePlayUIOut();
        }));
    }
    public void OnClickNextStage()
    {
        StageClearUIOut();
        NodeManager.Instance.OnClickStartStage(++NodeManager.Instance.stageIndex);
    }
    public void OnClickAgain()//다시하기 누르면
    {
        GameOverUIOut();
        MenuUIOut();
        StageClearUIOut();
        StageFailUIOut();
        StartCoroutine(NodeManager.Instance.ResetNodes(AgainDelay));
    }
    void AgainDelay()
    {
        if(GameManager.Instance.stage)
            StagePlayUIIn();
        else
            PlayUIIn();
        StartCoroutine(NodeManager.Instance.StartWork(GameManager.Instance.stage, GameManager.Instance.mode));
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
            mixer.SetFloat("SFX", 0f);
        }
        else
        {
            muteImage.sprite = mute[1];
            mixer.SetFloat("SFX", -80f);
        }
    }
    /*public void OnClickRemoveAd()
    {
        ShopScript.instance.NonConsumableRemoveAd_Press(RemoveAd);
    }

    private void RemoveAd()
    {
        PlayerPrefs.SetInt("Ad", 0);
        AdmobAdsScript.instance.DestroyBannerAd();
        ad.SetActive(false);
    }*/
}