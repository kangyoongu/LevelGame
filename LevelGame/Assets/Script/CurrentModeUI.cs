using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentModeUI : MonoBehaviour
{
    GameObject[] images;
    int modeCount;
    Image background;
    public Image[] whiteImage;
    public Image[] blackImage;
    private void Awake()
    {
        modeCount = transform.childCount;
        background = GetComponent<Image>();
        images = new GameObject[modeCount];
        for(int i = 0; i < modeCount; i++)
        {
            images[i] = transform.GetChild(i).gameObject;
        }
    }
    private void OnEnable()
    {
        NodeManager.Instance.OnSetStageMode += CurrentImage;
        ApplyCurrentTheme();
    }
    private void CurrentImage(int index)
    {
        for(int i = 0; i < modeCount; i++)
        {
            if (i == index) 
                images[i].SetActive(true);
            else 
                images[i].SetActive(false);
        }
    }
    private void ApplyCurrentTheme()
    {
        background.color = ThemeManager.Instance.CurrentTheme.play;
        foreach (Image image in blackImage)
        {
            image.color = ThemeManager.Instance.CurrentTheme.scoreBack;
        }
        foreach (Image image in whiteImage)
        {
            image.color = ThemeManager.Instance.CurrentTheme.icons;
        }
    }
}
