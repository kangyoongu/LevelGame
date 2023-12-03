using GooglePlayGames;
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
    public Sprite[] inside;
    public TextMeshProUGUI[] bestText;
    public TextMeshProUGUI[] currentText;
    private int score = 0;
    bool resurvive = false;

    public RandomPitchPlay popSound;
    public RandomPitchPlay makeSound;

    [HideInInspector] public Color warningColor;
    public int Score {
        get { return score; }
        set
        {
            score = value;

            if (score > PlayerPrefs.GetInt("Best"))
            {
                PlayerPrefs.SetInt("Best", score);
                if (Social.localUser.authenticated)
                {
                   GPGSBinder.Inst.ReportLeaderboard(GPGIds.leaderboard_rank, score);
                }
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
        makeSound.JustPlay();
    }
    public void MakeNode()
    {
        int index = Random.Range(0, blankNode.Count);
        MakeVisual(blankNode[index], Random.Range(1, 4));
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

    void ResetBlank()//정해진 빈칸의 번호들 리셋
    {
        for(int i = 0; i < fullNodes.Length; i++)
        {
            fullNodes[i].blankIndex = 0;
        }
    }
    public IEnumerator GameOver()//게임 끝남
    {
        yield return new WaitForSeconds(3);
        if (((Score >= 300 && Random.value > 0.5f) || Score >= 550) && resurvive == false)
        {
            resurvive = true;
            UIManager.instance.SurvivalUIIn();
        }
        else
        {
            OverSet();
        }
        GameManager.instance.canMove = false;
    }
    public void OnClickStart()//메인화면에서 시작버튼 누르면
    {
        UIManager.instance.MainUIOut();
        UIManager.instance.PlayUIIn();
        StartCoroutine(StartWork());
    }
    public IEnumerator StartWork()//시작할 때 할 일들
    {
        Score = 0;
        GameManager.instance.max = 5;
        resurvive = false;
        List<NodeInfo> list = new List<NodeInfo>(fullNodes);
        blankNode = new List<NodeInfo>(fullNodes);
        for (int i = 0; i < 8; i++)
        {
            int index = Random.Range(0, list.Count);
            MakeVisual(list[index], Random.Range(1, 4));
            list.Remove(list[index]);
        }
        yield return new WaitForEndOfFrame();
        EndCheck(false);
        yield return new WaitForSeconds(0.6f);
        GameManager.instance.canMove = true;
        UIManager.instance.block[1].SetActive(false);
    }
    public void PopSoundPlay()
    {
        popSound.Play();
    }
    public void RemoveVisual()//블록들 터트리는거
    {
        UIManager.instance.block[1].SetActive(true);
        for (int i = 0; i < fullNodes.Length; i++)
        {
            fullNodes[i].num = 0;
            if (!blankNode.Contains(fullNodes[i]))
            {
                StartCoroutine(fullNodes[i].visualmove.Disapear());
            }
        }
    }
    public IEnumerator GoToMain()//메인화면으로 가는거
    {
        yield return new WaitForSeconds(2.2f);
        UIManager.instance.block[1].SetActive(false);
        ResetBlank();
        Score = 0;
        UIManager.instance.MainUIIn();
        UIManager.instance.PlayUIOut();
    }
    public void OnClickAd()//광고보기 누르면
    {
        if (PlayerPrefs.GetInt("Ad") == 1)
        {
            Debug.Log("리워드 광고");
            //광고 띄움
        }
        else
        {
            Reservive();
        }
    }

    private void Reservive()
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
                    fullNodes[i].num = 0;
                }
                if (!blankNode.Contains(fullNodes[i]))
                {
                    StartCoroutine(fullNodes[i].visualmove.Disapear());
                }
            }
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
        GameManager.instance.canMove = true;
    }
    public void OnClickDone()//끝내기 누르면
    {
        AdBackground.minus = false;
        UIManager.instance.SurvivalUIOut();
        OverSet();

    }

    private void OverSet()
    {
        if(Random.value < 0.334f)
        {
            Debug.Log("광고나옴");
            //광고재생
        }
        UIManager.instance.GameOverUIIn();
        UIManager.instance.PlayUIOut();
        resurvive = false;
    }
}
