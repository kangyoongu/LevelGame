using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeManager : MonoBehaviour
{
    public static ThemeManager instance;
    public Camera background;
    public SpriteRenderer[] blanks;
    public Image[] play;
    public Image[] rank;
    public Image[] sound;
    public Image[] store;
    public Image[] setting;
    public Image[] scoreBack;
    public Image[] icons;
    public TextMeshProUGUI[] scores;
    
    public ThemeData[] everyTheme;
    public Image[] pedigree;
    public Image[] pedigreeIcon;
    public Image pedigreeBackground;
    public Image ring;
    public Image ad;
    public Image heart;
    public Image x;
    public Transform[] buys;
    public Transform get;
    public Transform sell;
    private void Start()
    {
        if (instance == null) instance = this;
        if (!PlayerPrefs.HasKey("Whear"))
        {
            PlayerPrefs.SetInt("Whear", 0);
            for (int i = 0; i < buys.Length; i++)
            {
                if (i < 3)
                {
                    PlayerPrefs.SetInt("Theme" + i, 1);
                }
                else {

                    PlayerPrefs.SetInt("Theme" + i, 0);
                }
            }
        }
        ApplyChange();
    }

    public void ApplyChange()
    {
        for (int i = 0; i < buys.Length; i++)
        {
            if (PlayerPrefs.GetInt("Theme" + i) == 0)
            {
                buys[i].SetParent(sell);
                buys[i].SetAsLastSibling();
            }
            else
            {
                buys[i].SetParent(get);
                buys[i].SetAsLastSibling();
                buys[i].GetChild(0).GetChild(1).gameObject.SetActive(false);
            }
            if(PlayerPrefs.GetInt("Whear") == i)
            {
                buys[i].GetChild(0).GetChild(0).gameObject.SetActive(true);
                ApplyTheme(everyTheme[i]);
            }
            else
            {
                buys[i].GetChild(0).GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    public void ApplyTheme(ThemeData theme)
    {
        background.backgroundColor = theme.backgroundColor;
        NodeManager.instance.nodeColor = theme.blockColor;
        NodeManager.instance.inside = theme.blockSprite;
        NodeManager.instance.warningColor = theme.warning;


        foreach(NodeInfo node in NodeManager.instance.fullNodes)
        {
            if(node.visualmove != null)
            {
                node.visualmove.SetColor();
            }
        }

        for(int i = 0; i < pedigree.Length; i++)
        {
            pedigree[i].color = theme.blockColor[i];
            pedigree[i].transform.GetChild(0).GetComponent<Image>().sprite = theme.blockSprite[i];
        }
        for(int i = 0; i < pedigreeIcon.Length; i++)
        {
            pedigreeIcon[i].color = theme.blockColor[i+1];
            pedigreeIcon[i].transform.GetChild(0).GetComponent<Image>().sprite = theme.blockSprite[i+1];
        }

        foreach(SpriteRenderer sr in blanks)
        {
            sr.color = theme.blankColor;
        }
        foreach(Image image in play)
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
        foreach(TextMeshProUGUI text in scores)
        {
            text.color = theme.icons;
        }
        pedigreeBackground.color = theme.pedigreeBackground;
        x.color = theme.x;
        ad.color = theme.ad;
        ring.color = theme.ring;
        heart.color = theme.heart;
    }
}
