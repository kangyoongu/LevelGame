using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeManager : SingleTon<ThemeManager>
{
    public SpriteRenderer[] blanks;
    public Image[] play;
    public Image[] background;
    public Image[] rank;
    public Image[] sound;
    public Image[] store;
    public Image[] setting;
    public Image[] scoreBack;
    public Image[] icons;
    public TextMeshProUGUI[] scoresText;
    public TextMeshProUGUI[] blackText;

    public ThemeData[] everyTheme;
    public Image[] pedigree;
    public Image[] pedigreeIcon;
    public Image pedigreeBackground;
    public Image ring;
    public Image[] ad;
    public Image[] heart;
    public Image[] x;
    public Transform[] themes;
    public Transform get;
    public Transform sell;

    public Action OnChangeTheme;
    public ThemeData CurrentTheme => everyTheme[PlayerPrefs.GetInt("Whear")];
    private void Start()
    {
        if (!PlayerPrefs.HasKey("Whear"))
        {
            PlayerPrefs.SetInt("Whear", 0);
            PlayerPrefs.SetInt("Theme" + 0, 1);
            for (int i = 1; i < themes.Length; i++)
            {
                PlayerPrefs.SetInt("Theme" + i, 0);
            }
        }
        ApplyChange();
    }

    public void ApplyChange()
    {
        for (int i = 0; i < themes.Length; i++)
        {
            if (PlayerPrefs.GetInt("Theme" + i) == 0)
            {
                themes[i].SetParent(sell);
                themes[i].SetAsLastSibling();
                themes[i].GetChild(0).GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                themes[i].SetParent(get);
                themes[i].SetAsLastSibling();
                themes[i].GetChild(0).GetChild(1).gameObject.SetActive(false);
            }
            /*if (i > 0)
            {
                themes[i].SetParent(sell);
                themes[i].SetAsLastSibling();
            }
            themes[i].GetChild(0).GetChild(1).gameObject.SetActive(false);*/
            if (PlayerPrefs.GetInt("Whear") == i)
            {
                themes[i].GetChild(0).GetChild(0).gameObject.SetActive(true);
                ApplyTheme(everyTheme[i]);
            }
            else
            {
                themes[i].GetChild(0).GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    public void ApplyTheme(ThemeData theme)
    {
        NodeManager.Instance.nodeColor = theme.blockColor;
        NodeManager.Instance.currentMat = theme.material;
        NodeManager.Instance.warningColor = theme.warning;
        OnChangeTheme?.Invoke();

        foreach (NodeInfo node in NodeManager.Instance.fullNodes)
        {
            if(node.visualMove != null)
            {
                node.visualMove.SetColor();
            }
        }

        for(int i = 0; i < pedigree.Length; i++)
        {
            pedigree[i].color = theme.blockColor[i];
            pedigree[i].sprite = theme.blockImage;
            pedigree[i].transform.GetChild(0).GetComponent<Image>().sprite = theme.sprites[i];
        }
        for(int i = 0; i < pedigreeIcon.Length; i++)
        {
            pedigreeIcon[i].color = theme.blockColor[i+1];
            pedigreeIcon[i].sprite = theme.blockImage;
            pedigreeIcon[i].transform.GetChild(0).GetComponent<Image>().sprite = theme.sprites[i];
        }

       /* foreach(SpriteRenderer sr in blanks)
        {
            sr.color = theme.blankColor;
        }*/
        foreach(Image image in background)
        {
            image.color = new Color(theme.backgroundColor.r, theme.backgroundColor.g, theme.backgroundColor.b, image.color.a);
        }
        foreach (Image image in play)
        {
            image.color = theme.play;
        }
        foreach (Image image in rank)
        {
            image.color = theme.rank;
        }
        foreach (Image image in sound)
        {
            image.color = theme.sound;
        }
        foreach (Image image in store)
        {
            image.color = theme.store;
        }
        foreach (Image image in setting)
        {
            image.color = theme.setting;
        }
        foreach (Image image in scoreBack)
        {
            image.color = theme.scoreBack;
        }
        foreach (Image image in icons)
        {
            image.color = theme.icons;
        }
        foreach(TextMeshProUGUI text in scoresText)
        {
            text.color = theme.icons;
        }
        foreach (TextMeshProUGUI text in blackText)
        {
            text.color = new Color(theme.scoreBack.r, theme.scoreBack.g, theme.scoreBack.b, text.color.a);
        }
        pedigreeBackground.color = theme.pedigreeBackground;
        foreach (Image image in x)
        {
            image.color = theme.x;
        }
        ring.color = theme.ring;
        foreach (Image image in heart)
        {
            image.color = new Color(theme.heart.r, theme.heart.g, theme.heart.b, image.color.a);
        }
        foreach (Image image in ad)
        {
            image.color = theme.ad;
        }
        //heart[1].color = new Color(heart[1].color.r, heart[1].color.g, heart[1].color.b, 1);
    }
}
