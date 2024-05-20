using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSetter : MonoBehaviour
{
    public GameObject chapter;
    public RectTransform viewport;
    public GameObject stageButton;
    public Transform chapterParent;
    public SnapScroll scroll;
    int stageNum;
    public static Action setButton;
    public static int pageStageNum = 20;
    private void Start()
    {
        stageNum = NodeManager.Instance.stageSO.Length;
        RectTransform button = stageButton.GetComponent<RectTransform>();

        /*int row = (int)((viewport.rect.height- 104) / (button.rect.height+33));
        int colum = (int)((viewport.rect.width- 104) / (button.rect.width+33));
        pageStageNum = row * colum;*/
        //print($"{viewport.rect.height} {button.rect.height}");
        pageStageNum = 32;
        int chapterNum = stageNum / pageStageNum;
        //print($"{pageStageNum}");

        for (int i = 0; i < chapterNum; i++)
        {
            Transform parent = Instantiate(chapter, chapterParent).transform;
            parent.GetComponent<RectTransform>().sizeDelta = new Vector2(viewport.rect.width, viewport.rect.height);
            for(int j = 0; j < pageStageNum; j++)
            {
                Instantiate(stageButton, parent);
            }
        }
        if (stageNum % pageStageNum > 0)
        {
            scroll.size++;
            Transform parent = Instantiate(chapter, chapterParent).transform;
            parent.GetComponent<RectTransform>().sizeDelta = new Vector2(viewport.rect.width, viewport.rect.height);
            for (int i = 0; i < stageNum % pageStageNum; i++)
            {
                Instantiate(stageButton, parent);
            }
            chapterNum++;
            scroll.size = chapterNum;
        }
        scroll.Init();
    }
}
