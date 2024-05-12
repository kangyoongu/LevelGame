using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Block : MonoBehaviour
{
    [HideInInspector] public int startIndex;
    [HideInInspector] public int endIndex;
    float time = 0.5f;
    public void Init(int start, int end)
    {
        startIndex = start;
        endIndex = end;
    }
    public void Enable()
    {
        transform.DOScaleY(2.5f, time).SetEase(Ease.OutBack);
    }
    public void Disable()
    {
        transform.DOScaleY(0f, time).SetEase(Ease.InBack).OnComplete(() => {
            Destroy(gameObject);
        });
    }

}
