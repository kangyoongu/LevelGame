using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrefabThemeColor : MonoBehaviour
{
    public Image[] setting;
    public Image[] play;
    public TextMeshProUGUI[] whiteText;
    public Image[] gray;

    private void OnEnable()
    {
        SetColors();
    }

    private void SetColors()
    {
        foreach (Image image in setting)
        {
            image.color = ThemeManager.Instance.CurrentTheme.setting;
        }
        foreach (Image image in play)
        {
            image.color = ThemeManager.Instance.CurrentTheme.play;
        }
        foreach (TextMeshProUGUI image in whiteText)
        {
            image.color = ThemeManager.Instance.CurrentTheme.icons;
        }
        foreach (Image image in gray)
        {
            image.color = ThemeManager.Instance.CurrentTheme.x;
        }
    }
}
