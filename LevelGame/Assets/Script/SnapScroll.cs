using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SnapScroll : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public Scrollbar scrollbar;
    [SerializeField] private TextMeshProUGUI pageText;

    [SerializeField] int size = 2;
    float[] pos;
    float distance, curPos, targetPos;
    bool isDrag = false;
    int targetIndex;

    private void Start()
    {
        pos = new float[size];
        distance = 1f / (size - 1);
        for (int i = 0; i < size; i++) pos[i] = distance * i;
        if (!PlayerPrefs.HasKey("Snap"))
        {
            PlayerPrefs.SetInt("Snap", 0);
        }
        targetPos = pos[PlayerPrefs.GetInt("Snap")];
        pageText.text = $"{targetPos + 1} / {size}";
        scrollbar.value = targetPos;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDrag = true;
        curPos = SetPos();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;
        targetPos = SetPos();

        if(curPos == targetPos)
        {
            if (eventData.delta.x > 10f && curPos - distance >= 0)
            {
                --targetIndex;
                targetPos = curPos - distance;
                pageText.text = $"{targetPos+1} / {size}";
                PlayerPrefs.SetInt("Snap", targetIndex);
            }
            else if (eventData.delta.x < -10f && curPos + distance <= 1.01f)
            {
                ++targetIndex;
                targetPos = curPos + distance;
                pageText.text = $"{targetPos+1} / {size}";
                PlayerPrefs.SetInt("Snap", targetIndex);
            }
        }
        else
        {
            pageText.text = $"{targetPos + 1} / {size}";
            PlayerPrefs.SetInt("Snap", targetIndex); 
        }
    }

    private float SetPos()
    {
        for (int i = 0; i < size; i++)
        {
            if (scrollbar.value < pos[i] + distance * 0.5f && scrollbar.value > pos[i] - distance * 0.5f)
            {
                targetIndex = i;
                return pos[i];
            }
        }
        return 0;
    }

    private void Update()
    {
        if(!isDrag) scrollbar.value = Mathf.Lerp(scrollbar.value, targetPos, 8f * Time.deltaTime);
    }
}
