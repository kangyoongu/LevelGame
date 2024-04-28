using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ArrowUI : MonoBehaviour
{
    int index;
    public int Index
    {
        get => index;
        set
        {
            if (!changing)
            {
                int temp = value;
                if (temp > index)
                {
                    changing = true;
                    temp %= options.Count;
                    text2.text = options[temp];
                    text2.rectTransform.anchoredPosition = text1.rectTransform.anchoredPosition + new Vector2(offset, 0);
                    text2.rectTransform.DOAnchorPosX(text1.rectTransform.anchoredPosition.x, 0.5f).SetUpdate(true);
                    text1.rectTransform.DOAnchorPosX(text1.rectTransform.anchoredPosition.x - offset, 0.5f).SetUpdate(true).OnComplete(() =>
                    {
                        TextMeshProUGUI text = text1;
                        text1 = text2;
                        text2 = text;
                        text2.text = "";
                        changing = false;
                    });
                    if (applyWithButton)
                    {
                        if (currentIndex != temp || currentIndex == -1)
                        {
                            button.SetActive(true);
                        }
                        else
                        {
                            button.SetActive(false);
                        }
                    }
                }
                else if (temp < index)
                {
                    changing = true;
                    if (temp < 0) temp += options.Count;
                    text2.text = options[temp];
                    text2.rectTransform.anchoredPosition = text1.rectTransform.anchoredPosition - new Vector2(offset, 0);
                    text2.rectTransform.DOAnchorPosX(text1.rectTransform.anchoredPosition.x, 0.5f).SetUpdate(true);
                    text1.rectTransform.DOAnchorPosX(text1.rectTransform.anchoredPosition.x + offset, 0.5f).SetUpdate(true).OnComplete(() =>
                    {
                        TextMeshProUGUI text = text1;
                        text1 = text2;
                        text2 = text;
                        text2.text = "";
                        changing = false;
                    });
                    if (applyWithButton)
                    {

                        if (currentIndex != temp || currentIndex == -1)
                        {
                            button.SetActive(true);
                        }
                        else
                        {
                            button.SetActive(false);
                        }
                    }
                }
                index = temp;
                if (!applyWithButton)
                {
                    OnChangeValue?.Invoke(index);
                }
            }
        }
    }

    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;
    public int offset = 200;
    public UnityEvent<int> OnChangeValue;
    public List<string> options;
    bool changing = false;
    public bool applyWithButton = false;
    public GameObject button;
    int currentIndex = -1;
    private void Start()
    {
        button.SetActive(false);
    }
    public void SetJustValue(int value)
    {
        index = value;
        text1.text = options[index];
        OnChangeValue?.Invoke(index);
        if (applyWithButton)
        {
            button.SetActive(false);
            currentIndex = value;
        }
    }
    public void OnClickLeft()
    {
        Index--;
    }
    public void OnClickRight()
    {
        Index++;
    }
    public void OnClickApply()
    {
        if (applyWithButton)
        {
            OnChangeValue?.Invoke(index);
            currentIndex = index;
            button.SetActive(false);
        }
    }
}
