using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public interface IMode 
{
    public void Init();//게임 시작할 때 한번
    public void StartMove();//움직임 딱 시작할때
    public void EndMove();//움직임 딱 끝날 때
    public bool CanDrag(NodeInfo start, NodeInfo end) => true;//지금 이 사이 드래그 할 수 있느냐?
    public void ResetGame();//끝났을 때
    public bool CanMultiSelect() => false;
    public void LastSpawn(List<NodeInfo> list, int level)//마지막 스폰(폭탄 만들 때 씀)
    {
        int index = Random.Range(0, list.Count);
        if(level == 0)
            NodeManager.Instance.MakeVisual(list[index], Random.Range(1, 4));
        else
            NodeManager.Instance.MakeVisual(list[index], level);
    }
}
