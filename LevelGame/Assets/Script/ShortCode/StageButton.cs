using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageButton : MonoBehaviour
{
    TextMeshProUGUI childText;
    int index;
    void Start()
    {
        index = transform.GetSiblingIndex();
        childText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        childText.text = (index+1).ToString();
        GetComponent<Button>().onClick.AddListener(DoStart);
    }
    void DoStart()
    {
        NodeManager.Instance.OnClickStartStage(index);
    }
}
