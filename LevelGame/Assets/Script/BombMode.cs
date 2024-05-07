using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombMode : MonoBehaviour, IMode
{
    public void EndMove()
    {
        return;
    }

    public void Init()
    {
        return;
    }

    public void ResetGame()
    {
        return;
    }

    public void StartMove()
    {
        return;
    }
    public void LastSpawn(List<NodeInfo> list, int level)//¸¶Áö¸· ½ºÆù(ÆøÅº ¸¸µé ¶§ ¾¸)
    {
        int index = Random.Range(0, list.Count);
        if (level == 0)
            NodeManager.Instance.MakeBombVisual(list[index], Random.Range(1, 4));
        else
            NodeManager.Instance.MakeBombVisual(list[index], level);
    }
}
