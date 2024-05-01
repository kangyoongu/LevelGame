using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : SingleTon<GameManager>
{
    public Camera mainCamera;

    bool click = false;
    List<NodeInfo> connects = new List<NodeInfo>();//�巡���ϸ鼭 ������ ��� �ֵ�
    [HideInInspector] public bool onOtherNode = true;
    [HideInInspector] public bool canMove = false;
    public int max = 5;

    public GameObject[] particles;

    //���� ���, ���������� ���� �ֵ�
    [HideInInspector] public bool stage = false;
    [HideInInspector] public bool mode = false;
    int dragCount;
    List<SpawnNode> spawnNodes;

    public void Awake()
    {
        if (!PlayerPrefs.HasKey("Best"))
        {
            PlayerPrefs.SetInt("Best", 0);
        }
        Application.targetFrameRate = 90;
    }
    void Update()
    {
        if (Input.touchCount == 1 && !IsPointerOverUIObject(Input.GetTouch(0).position))//��ġ�� 1���� ������
        {
            if (canMove == true)
            {
                OnMouseClick();
            }
        }
        else if (Input.touchCount == 0)
        {
            if (click == true)
            {
                EndDrag();
            }
        }
    }

    private void EndDrag()
    {
        if (connects[0].num == 10)
        {
            foreach (NodeInfo node in NodeManager.Instance.fullNodes)//10�̶� ���� warning����
            {
                if (node.num == 10)
                {
                    node.visualmove.warning.SetActive(false);
                }
            }
        }
        else
        {
            foreach (NodeInfo node in connects[0].neighbor)//ó��Ŭ���� ��� �ֺ� warning����
            {
                if (node.num == connects[0].num && node.visualmove != null)
                {
                    node.visualmove.warning.SetActive(false);
                }
            }
        }
        click = false;
        if (connects[0].num == connects[connects.Count - 1].num && connects.Count >= 2)
        {
            Merge();
        }
        else
        {
            onOtherNode = true;
            connects[0].visualmove.ToReset();
        }
    }

    private void Merge()
    {
        connects[0].num = 0;
        connects[connects.Count - 1].num++;
        NodeManager.Instance.Score += connects[connects.Count - 1].num;
        int spawnCount = 1;
        if (stage) dragCount++;
        if (connects[connects.Count - 1].num > max)//�ְ��� �����ϸ� �ϳ� �� ����
        {
            max = connects[connects.Count - 1].num;
            if (max == 10)
            {
                particles[0].SetActive(true);
                particles[1].SetActive(true);
            }
            spawnCount++;
        }
        if (stage && spawnNodes.Count > 0 && dragCount == spawnNodes[0].turnCount)
        {
            connects[0].visualmove.Move(connects[connects.Count - 1].visualmove, 1, spawnNodes[0].spawnLevel);
            spawnNodes.RemoveAt(0);
        }
        else
            connects[0].visualmove.Move(connects[connects.Count - 1].visualmove, spawnCount, 0);
    }

    private void OnMouseClick()
    {
        Touch touch = Input.GetTouch(0); // ù ��° ��ġ �Է� ���

        Ray touchRay = mainCamera.ScreenPointToRay(touch.position);

        // Raycast�� ����Ͽ� ��ġ ��ġ���� ��������Ʈ�� ����
        RaycastHit hit;
        if (Physics.Raycast(touchRay, out hit))//���� �������� 
        {
            GameObject selectedSprite = hit.collider.gameObject;//��������
            if (selectedSprite.CompareTag("Node")) //�±װ� node��
            {
                NodeInfo clicked = selectedSprite.GetComponent<NodeInfo>();//nodeinfo������

                if (touch.phase == TouchPhase.Began)//ùŬ���̸�
                {
                    if (clicked.num >= 1 && clicked.visualmove != null)//node ���ڰ� 1�̻��̸�(�� �ڸ��� ���� ������)
                    {
                        if (connects.Count > 0 && connects[0].num > 0 && connects[0].visualmove != null)//���� �� ���װ� ���°� ������ 
                        {
                            connects[0].visualmove.ToReset();
                            foreach (NodeInfo node in connects[0].neighbor)
                            {
                                if (node.num == connects[0].num && node.visualmove != null)
                                {
                                    node.visualmove.warning.SetActive(false);
                                }
                            }
                        }
                        connects = new List<NodeInfo>();
                        connects.Add(clicked);
                        connects[0].visualmove.AddPosition(clicked.transform.position + new Vector3(0, 0.06f, 0));
                        click = true;
                        if (connects[0].num == 10)
                        {
                            foreach (NodeInfo node in NodeManager.Instance.fullNodes)
                            {
                                if (node.num == 10 && node != clicked)
                                {
                                    node.visualmove.warning.SetActive(true);
                                }
                            }
                        }
                        else
                        {
                            if (connects.Count > 0 && connects[0].num > 0)
                            {
                                foreach (NodeInfo node in connects[0].neighbor)
                                {
                                    if (node.num == connects[0].num && node.visualmove != null)
                                    {
                                        node.visualmove.warning.SetActive(true);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (click)
                    {
                        if (connects.Contains(clicked))//�̹� �������濡 ����
                        {
                            if (connects[connects.Count - 1] != clicked)//���ڸ��� �ƴϸ�(������ �� ������ �� �� �����)
                            {
                                int index = connects.IndexOf(clicked) + 1;
                                connects[0].visualmove.CutUp(index);
                                int countToRemove = connects.Count - index;
                                connects.RemoveRange(index, countToRemove);
                                onOtherNode = true;
                            }
                        }
                        else if (connects[connects.Count - 1].neighbor.Contains(clicked) && onOtherNode == true)//���ο� �濡 ����
                        {
                            if (clicked.num <= 0)
                            {
                                connects.Add(clicked);
                                connects[0].visualmove.AddPosition(clicked.transform.position + new Vector3(0, 0.06f, 0));
                            }
                            else if (clicked.num == connects[0].num && !connects[0].neighbor.Contains(clicked) && connects[0].num < 10 && clicked.visualmove != null)//�̿��� ����������
                            {
                                connects.Add(clicked);
                                connects[0].visualmove.AddPosition(clicked.transform.position + new Vector3(0, 0.06f, 0));
                                onOtherNode = false;
                            }
                        }
                    }
                }
            }
        }
    }
    public bool IsPointerOverUIObject(Vector2 touchPos)
    {
        PointerEventData eventDataCurrentPosition
            = new PointerEventData(EventSystem.current);

        eventDataCurrentPosition.position = touchPos;

        List<RaycastResult> results = new List<RaycastResult>();


        EventSystem.current
        .RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }
    public void StartGame(bool stage, bool mode, StageSettingSO so)
    {
        this.stage = stage;
        this.mode = mode;
        dragCount = 0;
        if (stage)
        {
            spawnNodes = new List<SpawnNode>(so.spawnNode);
        }
    }
}
