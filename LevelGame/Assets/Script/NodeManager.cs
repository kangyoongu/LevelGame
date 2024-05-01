using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class NodeManager : SingleTon<NodeManager>
{
    public NodeInfo[] fullNodes;
    public List<NodeInfo> blankNode = new List<NodeInfo>();
    public GameObject visual;

    public Color[] nodeColor;
    [HideInInspector]public Material currentMat;
    public TextMeshProUGUI[] bestText;
    public TextMeshProUGUI[] currentText;
    private int score = 0;
    bool resurvive = false;

    public RandomPitchPlay popSound;
    public RandomPitchPlay makeSound;

    [HideInInspector] public Color warningColor;

    public StageSettingSO[] stageSO;
    [HideInInspector]public int stageIndex;
    int[] clearTargets;
    public TextMeshProUGUI targetText;
    Coroutine delayMakeVisual;
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
        Instantiate(visual, node.transform.position + new Vector3(0f, 0.1144f, 0f), node.transform.rotation, node.transform).GetComponent<VisualMove>().SetColor();
        blankNode.Remove(node);
        makeSound.JustPlay();
    }
    IEnumerator MakeVisual(NodeInfo node, int num, float delay)
    {
        blankNode.Remove(node);
        node.num = num;
        yield return new WaitForSeconds(delay);
        if (!GameManager.Instance.canMove) yield break;
        Vector3 pos = node.transform.position + new Vector3(0f, 0.1144f, 0f);
        VisualMove newNode = Instantiate(visual, pos, node.transform.rotation, node.transform).GetComponent<VisualMove>();
        newNode.SetColor();
        makeSound.JustPlay();
        
        if (GameManager.Instance.stage)
        {
            CheckClear();
        }
    }

    public void CheckClear()
    {
        clearTargets = new int[stageSO[stageIndex].clearTarget.Count];
        for (int i = 0; i < fullNodes.Length; i++)
        {
            if(fullNodes[i].num != 0)
            {
                for(int j = 0; j < clearTargets.Length; j++)
                {
                    if(fullNodes[i].num == stageSO[stageIndex].clearTarget[j].nodeLevel)
                    {
                        clearTargets[j]++;
                        break;
                    }
                }
            }
        }
        for (int i = 0; i < clearTargets.Length; i++)
        {
            if (clearTargets[i] < stageSO[stageIndex].clearTarget[i].count)
            {
                return;
            }
        }
        StartCoroutine(ResetNodes(() =>
        {
            UIManager.instance.StageClearUIIn();
        }, 1.5f));
    }

    public void MakeNode(float visualDelay = 0, int level = 0)
    {
        int index = Random.Range(0, blankNode.Count);
        if (visualDelay == 0f)
            MakeVisual(blankNode[index], level == 0 ? Random.Range(1, 4) : level);
        else
        {
            delayMakeVisual = StartCoroutine(MakeVisual(blankNode[index], level == 0 ? Random.Range(1, 4) : level, visualDelay));
        }
    }

    public bool EndCheck(bool chance)//������ �������� üũ
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
                    if(node[j].num == node[x].num && j != x && !node[j].neighbor.Contains(node[x]) && node[j].num < 10 && node[x].num < 10)
                    {
                        return false;
                    }
                }
            }
        }
        if (chance == false)
        {
            StartCoroutine(GameOver());
            return false;
        }
        else
        {
            return true;
        }
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

    void ResetBlank()//������ ���� ��������� ��� ��������� ��ȣ�� �ű�µ� �Ű��� ��ȣ �ʱ�ȭ�ϴ°�
    {
        for(int i = 0; i < fullNodes.Length; i++)
        {
            fullNodes[i].blankIndex = 0;
        }
    }
    public IEnumerator GameOver()//���� ����
    {
        yield return new WaitForSeconds(3);
        if (GameManager.Instance.stage)
        {
            UIManager.instance.StageFailUIIn();
        }
        else
        {
            if (((Score >= 250 && Random.value > 0.5f) || Score >= 450) && resurvive == false)
            {
                resurvive = true;
                UIManager.instance.SurvivalUIIn();
            }
            else
            {
                OverSet();
            }
        }
        GameManager.Instance.canMove = false;
    }
    public void OnClickStartStage(int index)//����ȭ�鿡�� ���۹�ư ������
    {
        UIManager.instance.MainUIOut();
        UIManager.instance.StageUIOut();
        UIManager.instance.StagePlayUIIn();
        stageIndex = index;
        StartCoroutine(StartWork(true, false));
    }

    public void OnClickStart(int gameMode)//����ȭ�鿡�� ���۹�ư ������
    {
        UIManager.instance.MainUIOut();
        UIManager.instance.SelectModeUIOut();
        UIManager.instance.PlayUIIn();
        StartCoroutine(StartWork(false, true));
    }
    public IEnumerator StartWork(bool stage = true, bool mode = false)//������ �� �� �ϵ�
    {
        SetAllNumToZero();
        GameManager.Instance.StartGame(stage, mode, stageSO[stageIndex]);
        Score = 0;
        GameManager.Instance.max = 5;
        resurvive = false;
        List<NodeInfo> list = new List<NodeInfo>(fullNodes);
        blankNode = new List<NodeInfo>(fullNodes);
        if (stage)
        {
            for (int i = 0; i < stageSO[stageIndex].startFormat.Count; i++)
            {
                int index = stageSO[stageIndex].startFormat[i].blockNum;
                MakeVisual(list[index], stageSO[stageIndex].startFormat[i].spawnLevel);
                list[index] = null;
            }
            for(int i = 0; i < list.Count; i++)
            {
                if (list[i] == null) list.RemoveAt(i);
            }
            int leftSpawn = stageSO[stageIndex].startNodeCount - stageSO[stageIndex].startFormat.Count;
            for(int i = 0; i < leftSpawn; i++)
            {
                int index = Random.Range(0, list.Count);
                MakeVisual(list[index], Random.Range(1, 4));
                list.RemoveAt(index);
            }
            targetText.text = "";
            for(int i = 0; i < stageSO[stageIndex].clearTarget.Count; i++)
            {
                targetText.text += $"level {stageSO[stageIndex].clearTarget[i].nodeLevel} no.{stageSO[stageIndex].clearTarget[i].count}\n";
            }
        }
        else
        {
            for (int i = 0; i < 8; i++)
            {
                int index = Random.Range(0, list.Count);
                MakeVisual(list[index], Random.Range(1, 4));
                list.RemoveAt(index);
            }
        }
        yield return new WaitForEndOfFrame();
        EndCheck(false);
        yield return new WaitForSeconds(0.6f);
        GameManager.Instance.canMove = true;
        UIManager.instance.block[1].SetActive(false);
    }
    public void PopSoundPlay()
    {
        popSound.Play();
    }
    public void RemoveVisual()//��ϵ� ��Ʈ���°�
    {
        if (delayMakeVisual != null)
        {
            StopCoroutine(delayMakeVisual);
        }
        for (int i = 0; i < fullNodes.Length; i++)
        {
            if (fullNodes[i].visualmove != null) 
            {
                StartCoroutine(fullNodes[i].visualmove.Disapear());
            }
        }
    }
    public IEnumerator ResetNodes(Action action, float startDelay = 0f)
    {
        GameManager.Instance.canMove = false;
        yield return new WaitForSeconds(startDelay);
        RemoveVisual();
        yield return new WaitForSeconds(2f);
        ResetBlank();
        action?.Invoke();
    }
    public void OnClickAd()//������ ������
    {
        /*if (PlayerPrefs.GetInt("Ad") == 1)
        {
            Debug.Log("������ ����");
            AdmobAdsScript.instance.ShowRewardedAd(Reservive);
        }
        else
        {
            Reservive();
        }*/
        Reservive();
    }

    private void Reservive()//�츮��
    {
        UIManager.instance.block[2].SetActive(true);
        UIManager.instance.SurvivalUIOut();
        AdBackground.minus = false;
        StartCoroutine(ReturnBlock());
    }

    private IEnumerator ReturnBlock()
    {
        while (EndCheck(true))
        {
            List<int> nums = new List<int>();
            for (int i = 0; i < fullNodes.Length; i++)
            {
                if (fullNodes[i].num != 0)
                {
                    nums.Add(fullNodes[i].num);
                }
                if (!blankNode.Contains(fullNodes[i]))
                {
                    StartCoroutine(fullNodes[i].visualmove.Disapear());
                }
            }
            SetAllNumToZero();
            yield return new WaitForSeconds(1.5f);
            for (int i = 0; i < nums.Count; i++)
            {
                float delay = Random.Range(0f, 0.2f);
                yield return new WaitForSeconds(delay);
                NodeInfo node = blankNode[Random.Range(0, blankNode.Count)];
                MakeVisual(node, nums[i]);
                blankNode.Remove(node);
            }
            yield return new WaitForSeconds(0.5f);
        }
        UIManager.instance.block[2].SetActive(false);
        GameManager.Instance.canMove = true;
    }

    private void SetAllNumToZero()
    {
        for(int i = 0; i <fullNodes.Length; i++)
        {
            if (fullNodes[i].num != 0) fullNodes[i].num = 0;
        }
    }

    public void OnClickDone()//������ ������
    {
        AdBackground.minus = false;
        UIManager.instance.SurvivalUIOut();
        OverSet();

    }
    private void OverSet()
    {
        /*if(Random.value < 0.33333f && PlayerPrefs.GetInt("Ad") == 1)
        {
            Debug.Log("������");
            AdmobAdsScript.instance.ShowInterAd();
        }*/
        UIManager.instance.GameOverUIIn();
        UIManager.instance.PlayUIOut();
        resurvive = false;
    }
}
