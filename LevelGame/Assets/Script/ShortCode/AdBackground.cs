using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdBackground : MonoBehaviour
{
    public Image bar;

    public static bool minus = false;
    private void OnEnable()
    {
        bar.fillAmount = 1;
        minus = true;
    }
    void Update()
    {
        if (minus)
        {
            bar.fillAmount -= Time.deltaTime * 0.13f;
            if (bar.fillAmount == 0)
            {
                NodeManager.instance.OnClickDone();
                minus = false;
            }
        }
    }
}
