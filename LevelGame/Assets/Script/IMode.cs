using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public interface IMode 
{
    public void Init();//���� ������ �� �ѹ�
    public void StartMove();//������ �� �����Ҷ�
    public void EndMove();//������ �� ���� ��
    public bool CanDrag(NodeInfo start, NodeInfo end) => true;//���� �� ���� �巡�� �� �� �ִ���?
    public void ResetGame();//������ ��
    public bool CanMultiSelect() => false;
    public void LastSpawn(List<NodeInfo> list, int level)//������ ����(��ź ���� �� ��)
    {
        int index = Random.Range(0, list.Count);
        if(level == 0)
            NodeManager.Instance.MakeVisual(list[index], Random.Range(1, 4));
        else
            NodeManager.Instance.MakeVisual(list[index], level);
    }
}
