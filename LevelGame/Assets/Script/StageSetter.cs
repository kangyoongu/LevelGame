using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSetter : MonoBehaviour
{
    public GameObject chapter;
    public GameObject stageButton;
    public Transform chapterParent;
    public SnapScroll scroll;
    int stageNum;
    public static Action setButton;
    private void Start()
    {
        stageNum = NodeManager.Instance.stageSO.Length;
        int chapterNum = stageNum / 32;
        scroll.size = chapterNum;
        for (int i = 0; i < chapterNum; i++)
        {
            Transform parent = Instantiate(chapter, chapterParent).transform;
            for(int j = 0; j < 32; j++)
            {
                Instantiate(stageButton, parent);
            }
        }
        if (stageNum % 32 > 0)
        {
            scroll.size++;
            Transform parent = Instantiate(chapter, chapterParent).transform;
            for (int i = 0; i < stageNum % 32; i++)
            {
                Instantiate(stageButton, parent);
            }
        }
        scroll.Init();
    }
}
