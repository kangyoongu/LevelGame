using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class NodeManager : MonoBehaviour
{
    public static NodeManager instance;
    public NodeInfo[] fullNodes;
    public List<NodeInfo> blankNode = new List<NodeInfo>();
    public GameObject visual;

    public Color[] nodeColor;
    public TextMeshProUGUI[] bestText;
    public TextMeshProUGUI[] currentText;
    private int score = 0;
    bool resurvive = false;
    public int Score {
        get { return score; }
        set
        {
            score = value;

            if (score > PlayerPrefs.GetInt("Best"))
            {
                PlayerPrefs.SetInt("Best", score);
            }
            RenewalText();
        }
    }
    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        RenewalText();
    }

    private void RenewalText()
    {
        for(int i = 0; i < bestText.Length; i++)
            bestText[i].text = PlayerPrefs.GetInt("Best").ToString("0");
        for (int i = 0; i < currentText.Length; i++)
            currentText[i].text = Score.ToString("0");
    }

    void MakeVisual(NodeInfo node, int num)
    {
        node.num = num;
        Instantiate(visual, node.transform.position, node.transform.rotation, node.transform).GetComponent<VisualMove>().SetColor();
        blankNode.Remove(node);
    }
    public void MakeNode()
    {
        int index = Random.Range(0, blankNode.Count);
        MakeVisual(blankNode[index], Random.Range(1, 4));
    }

    public void EndCheck()//������ �������� üũ
    {
        ResetBlank();
        int index = 1;
        for (int i = 0; i < blankNode.Count; i++)
        {
            if(blankNode[i].blankIndex == 0)
            {
                PutIndex(index, blankNode[i]);
                index++;
            }
        }
        for (int i = 1; i < index+1; i++)
        {
            List<NodeInfo> node = new List<NodeInfo>();
            for(int j = 0; j < blankNode.Count; j++)
            {
                if(blankNode[j].blankIndex == i)
                {
                    for (int x = 0; x < blankNode[j].neighbor.Count; x++)
                    {
                        if (!node.Contains(blankNode[j].neighbor[x]) && !blankNode.Contains(blankNode[j].neighbor[x]))
                        {
                            node.Add(blankNode[j].neighbor[x]);
                        }
                    }
                }
            }
            for(int j = 0; j < node.Count; j++)
            {
                for(int x = 0; x < node.Count; x++)
                {
                    if(node[j].num == node[x].num && j != x && !node[j].neighbor.Contains(node[x]))
                    {
                        return;
                    }
                }
            }
        }
        GameOver();
    }

    private void PutIndex(int index, NodeInfo nodeInfo)
    {
        nodeInfo.blankIndex = index;
        for(int i = 0; i < nodeInfo.neighbor.Count; i++)
        {
            if(nodeInfo.neighbor[i].blankIndex == 0 && nodeInfo.neighbor[i].num == 0)
            {
                PutIndex(index, nodeInfo.neighbor[i]);
            }
        }
    }

    void ResetBlank()//������ ��ĭ�� ��ȣ�� ����
    {
        for(int i = 0; i < fullNodes.Length; i++)
        {
            fullNodes[i].blankIndex = 0;
        }
    }
    public void GameOver()//���� ����
    {
        if (Score > PlayerPrefs.GetInt("Best") * 0.7f && resurvive == false)
        {
            resurvive = true;
            UIManager.instance.SurvivalUIIn();
        }
        else
        {
            UIManager.instance.GameOverUIIn();
            UIManager.instance.PlayUIOut();
            resurvive = false;
        }
        GameManager.instance.canMove = false;
    }
    public void OnClickStart()//����ȭ�鿡�� ���۹�ư ������
    {
        UIManager.instance.MainUIOut();
        UIManager.instance.PlayUIIn();
        StartCoroutine(StartWork());
    }
    public IEnumerator StartWork()//������ �� �� �ϵ�
    {
        Score = 0;
        GameManager.instance.max = 6;
        List<NodeInfo> list = new List<NodeInfo>(fullNodes);
        blankNode = new List<NodeInfo>(fullNodes);
        for (int i = 0; i < 7; i++)
        {
            int index = Random.Range(0, list.Count);
            MakeVisual(list[index], Random.Range(1, 4));
            list.Remove(list[index]);
        }
        yield return new WaitForEndOfFrame();
        EndCheck();
        yield return new WaitForSeconds(0.6f);
        GameManager.instance.canMove = true;
    }
    public void RemoveVisual()//��ϵ� ���� ��Ʈ���°�
    {
        for(int i = 0; i < fullNodes.Length; i++)
        {
            fullNodes[i].num = 0;
            if (!blankNode.Contains(fullNodes[i]))
            {
                StartCoroutine(fullNodes[i].visualmove.Disapear());
            }
        }
    }
    public IEnumerator GoToMain()//����ȭ������ ���°�
    {
        yield return new WaitForSeconds(2.5f);
        ResetBlank();
        Score = 0;
        UIManager.instance.MainUIIn();
    }
    public void OnClickAd()//������ ������
    {
        UIManager.instance.SurvivalUIOut();
        List<int> nums = new List<int>();
        for (int i = 0; i < fullNodes.Length; i++)
        {
            if (fullNodes[i].num != 0)
            {
                nums.Add(fullNodes[i].num);
                fullNodes[i].num = 0;
            }
            if (!blankNode.Contains(fullNodes[i]))
            {
                StartCoroutine(fullNodes[i].visualmove.Disapear());
            }
        }
        StartCoroutine(ReturnBlock(nums));
    }
    private IEnumerator ReturnBlock(List<int> nums)
    {
        yield return new WaitForSeconds(1.5f);
        for(int i = 0; i < nums.Count; i++)
        {
            float delay = Random.Range(0f, 0.2f);
            yield return new WaitForSeconds(delay);
            NodeInfo node = blankNode[Random.Range(0, blankNode.Count)];
            MakeVisual(node, nums[i]);
            blankNode.Remove(node);
        }
        GameManager.instance.canMove = true;
    }
    public void OnClickDone()//������ ������
    {
        UIManager.instance.GameOverUIIn();
        UIManager.instance.PlayUIOut();
        resurvive = false;
    }
}
