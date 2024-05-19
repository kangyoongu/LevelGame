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
    public static int pageStageNum = 20;
    private void Start()
    {
        stageNum = NodeManager.Instance.stageSO.Length;
        int chapterNum = stageNum / pageStageNum;
        scroll.size = chapterNum;

        for (int i = 0; i < chapterNum; i++)
        {
            Transform parent = Instantiate(chapter, chapterParent).transform;
            for(int j = 0; j < pageStageNum; j++)
            {
                Instantiate(stageButton, parent);
            }
        }
        if (stageNum % pageStageNum > 0)
        {
            scroll.size++;
            Transform parent = Instantiate(chapter, chapterParent).transform;
            for (int i = 0; i < stageNum % pageStageNum; i++)
            {
                Instantiate(stageButton, parent);
            }
        }
        scroll.Init();
    }
}
