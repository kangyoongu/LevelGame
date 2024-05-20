using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageButton : MonoBehaviour
{
    TextMeshProUGUI childText;
    GameObject image;
    int index;
    void Start()
    {
        StageSetter.setButton += SetImage;
        childText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        image = transform.GetChild(1).gameObject;
        index = transform.GetSiblingIndex() + (StageSetter.pageStageNum * transform.parent.GetSiblingIndex());
        childText.text = (index+1).ToString();
        SetImage();
        GetComponent<Button>().onClick.AddListener(DoStart);
    }
    void DoStart()
    {
        if (index <= GameManager.Instance.StageNum)
            NodeManager.Instance.OnClickStartStage(index);
    }
    void SetImage()
    {
        if (index  <= GameManager.Instance.StageNum)
        {
            image.SetActive(false);
            childText.gameObject.SetActive(true);
        }
    }
}
