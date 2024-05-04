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
    public TextMeshProUGUI[] bestText;
    public TextMeshProUGUI[] currentText;
    public TextMeshProUGUI targetText;
    int[] clearTargets;
    bool resurvive = false;

    public RandomPitchPlay popSound;
    public RandomPitchPlay makeSound;

    [HideInInspector] public Material currentMat;
    [HideInInspector] public Color warningColor;
    [HideInInspector] public int stageIndex;

    public StageSettingSO[] stageSO;
    public GameObject[] modeObject;
    IMode[] modes;
    public IMode currentMode;
    public Action OnStartMode;
    public Action OnEndMove;

    Coroutine delayMakeVisual;
    private int score = 0;
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
        modes = new IMode[modeObject.Length];
        for(int i = 0; i < modeObject.Length; i++)
        {
            modes[i] = modeObject[i].GetComponent<IMode>();
        }
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

        if (stageIndex >= GameManager.Instance.StageNum)
        {
            GameManager.Instance.StageNum++;
        }
        StartCoroutine(ResetNodes(() =>
        {
            UIManager.Instance.StageClearUIIn();
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

    public bool EndCheck(bool chance)//게임이 끝났는지 체크
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
                        if (!node.Contains(blankNode[j].neighbor[x]) && !blankNode.Contains(blankNode[j].neighbor[x]) && currentMode.CanDrag(blankNode[j], blankNode[j].neighbor[x]))
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
            if(nodeInfo.neighbor[i].blankIndex == 0 && nodeInfo.neighbor[i].num == 0 && currentMode.CanDrag(nodeInfo, nodeInfo.neighbor[i]))
            {
                PutIndex(index, nodeInfo.neighbor[i]);
            }
        }
    }

    void ResetBlank()//나뉘지 않은 빈공간끼리 묶어서 빈공간마다 번호를 매기는데 매겨전 번호 초기화하는것
    {
        for(int i = 0; i < fullNodes.Length; i++)
        {
            fullNodes[i].blankIndex = 0;
        }
    }
    public IEnumerator GameOver()//게임 끝남
    {
        yield return new WaitForSeconds(3);
        if (GameManager.Instance.stage)
        {
            UIManager.Instance.StageFailUIIn();
        }
        else
        {
            if (((Score >= 250 && Random.value > 0.5f) || Score >= 450) && resurvive == false)
            {
                resurvive = true;
                UIManager.Instance.SurvivalUIIn();
            }
            else
            {
                OverSet();
            }
        }
        GameManager.Instance.canMove = false;
    }
    public void OnClickStartStage(int index)//메인화면에서 시작버튼 누르면
    {
        UIManager.Instance.MainUIOut();
        UIManager.Instance.StageUIOut();
        UIManager.Instance.StagePlayUIIn();
        stageIndex = index;
        StartCoroutine(StartWork(true));
    }

    public void OnClickStart(int gameMode)//메인화면에서 시작버튼 누르면
    {
        UIManager.Instance.MainUIOut();
        UIManager.Instance.SelectModeUIOut();
        UIManager.Instance.PlayUIIn();
        StartCoroutine(StartWork(false, gameMode));
    }
    public IEnumerator StartWork(bool stage = true, int mode = 0)//시작할 때 할 일들
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
            int ind = 0;
            while (ind < list.Count)
            {
                if (list[ind] == null) list.RemoveAt(ind);
                else ind++;
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
        OnEndMove = null;
        OnEndMove += modes[mode].EndMove;
        OnStartMode = null;
        OnStartMode += modes[mode].StartMove;
        modes[mode].Init();
        currentMode = modes[mode];
        yield return new WaitForEndOfFrame();
        EndCheck(false);
        yield return new WaitForSeconds(0.6f);
        GameManager.Instance.canMove = true;
        UIManager.Instance.block[1].SetActive(false);
    }
    public void PopSoundPlay()
    {
        popSound.Play();
    }
    public void RemoveVisual()//블록들 터트리는거
    {
        if (delayMakeVisual != null)
        {
            StopCoroutine(delayMakeVisual);
        }
        for (int i = 0; i < fullNodes.Length; i++)
        {
            if (fullNodes[i].visualmove != null) 
            {
                StartCoroutine(fullNodes[i].visualmove.Remove());
            }
        }
    }
    public IEnumerator ResetNodes(Action action, float startDelay = 0f)
    {
        GameManager.Instance.canMove = false;
        yield return new WaitForSeconds(startDelay);
        RemoveVisual();
        currentMode.ResetGame();
        yield return new WaitForSeconds(2f);
        ResetBlank();
        action?.Invoke();
    }
    public void OnClickAd()//광고보기 누르면
    {
        /*if (PlayerPrefs.GetInt("Ad") == 1)
        {
            Debug.Log("리워드 광고");
            AdmobAdsScript.instance.ShowRewardedAd(Reservive);
        }
        else
        {
            Reservive();
        }*/
        Reservive();
    }

    private void Reservive()//살리기
    {
        UIManager.Instance.block[2].SetActive(true);
        UIManager.Instance.SurvivalUIOut();
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
                    StartCoroutine(fullNodes[i].visualmove.Remove());
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
        UIManager.Instance.block[2].SetActive(false);
        GameManager.Instance.canMove = true;
    }

    private void SetAllNumToZero()
    {
        for(int i = 0; i <fullNodes.Length; i++)
        {
            if (fullNodes[i].num != 0) fullNodes[i].num = 0;
        }
    }

    public void OnClickDone()//끝내기 누르면
    {
        AdBackground.minus = false;
        UIManager.Instance.SurvivalUIOut();
        OverSet();

    }
    private void OverSet()
    {
        /*if(Random.value < 0.33333f && PlayerPrefs.GetInt("Ad") == 1)
        {
            Debug.Log("광고나옴");
            AdmobAdsScript.instance.ShowInterAd();
        }*/
        UIManager.Instance.GameOverUIIn();
        UIManager.Instance.PlayUIOut();
        resurvive = false;
    }
}
