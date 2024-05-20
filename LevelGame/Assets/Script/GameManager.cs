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
    List<VisualMove> warnings = new List<VisualMove>();
    [HideInInspector] public bool onOtherNode = true;
    [HideInInspector] public bool canMove = false;
    public int max = 5;

    public GameObject[] particles;

    //���� ���, ���������� ���� �ֵ�
    [HideInInspector] public bool stage = false;
    [HideInInspector] public int mode = 0;
    int dragCount;
    List<SpawnNode> spawnNodes;

    public int unlockWall;
    public int unlockMultiSelect;
    public int unlockBoom;
    [SerializeField] GameObject[] locks;

    public int StageNum {
        get
        {
            return PlayerPrefs.GetInt("StageNum");
        }
        set
        {
            PlayerPrefs.SetInt("StageNum", value);

            if (value >= unlockWall)
                locks[2].SetActive(false);
            if (value >= unlockMultiSelect)
                locks[1].SetActive(false);
            if (value >= unlockBoom)
                locks[0].SetActive(false);

            StageSetter.setButton?.Invoke();
        }
    }
    public void Awake()
    {
        Application.targetFrameRate = 90;
        //PlayerPrefs.DeleteAll();
        if (!PlayerPrefs.HasKey("Best"))
        {
            PlayerPrefs.SetInt("Best", 0);
            PlayerPrefs.SetInt("BlockBest", 0);
            PlayerPrefs.SetInt("MultiBest", 0);
            PlayerPrefs.SetInt("BombBest", 0);
            PlayerPrefs.SetInt("StageNum", 0);
            PlayerPrefs.SetInt("Mode", 0);
        }
        StageNum = StageNum;
    }
    private void Start()
    {
        UIManager.Instance.RenewalModeIcon();
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
        RemoveWarning();
        click = false;
        if (connects[0].num == connects[connects.Count - 1].num && connects.Count >= 2)
        {
            Merge();
        }
        else
        {
            connects[0].visualMove.ToReset();
        }
        onOtherNode = true;
    }

    private void Merge()
    {
        canMove = false;
        connects[0].num = 0;
        connects[connects.Count - 1].num++;
        NodeManager.Instance.Score += connects[connects.Count - 1].num;
        int spawnCount = 1;
        if (NodeManager.Instance.currentMode.CanMultiSelect()) spawnCount++;//��Ƽ�������� ���� 2����
        if (stage) dragCount++;
        if (connects[connects.Count - 1].num > max)//�ְ��� �����ϸ� �ϳ� �� ����
        {
            max = connects[connects.Count - 1].num;
            if (max == 10)//��������� 10�̸� ����
            {
                particles[0].SetActive(true);
                particles[1].SetActive(true);
            }
            spawnCount++;
        }
        if (stage && spawnNodes.Count > 0 && dragCount == spawnNodes[0].turnCount)//������������ �̹��Ͽ� ������� �ָ� ����������
        {
            int spawn = 0;
            while (spawnNodes.Count > 0 && spawnNodes[0].turnCount == dragCount)
            {
                if (spawn == 0)
                {
                    if (connects[0].visualMove as BombVisualMove)
                        (connects[0].visualMove as BombVisualMove).Move(connects[connects.Count - 1].visualMove, 1, spawnNodes[0].spawnLevel);
                    else
                        connects[0].visualMove.Move(connects[connects.Count - 1].visualMove, 1, spawnNodes[0].spawnLevel);
                }
                else
                {
                    if (connects[0].visualMove as BombVisualMove)
                        (connects[0].visualMove as BombVisualMove).AddSpawn(1, spawnNodes[0].spawnLevel);
                    else
                        connects[0].visualMove.AddSpawn(1, spawnNodes[0].spawnLevel);
                }
                spawn++;
                spawnNodes.RemoveAt(0);
            }
            if(spawn < spawnCount)
            {

                if (connects[0].visualMove as BombVisualMove)
                    (connects[0].visualMove as BombVisualMove).AddSpawn(spawnCount- spawn, 0);
                else
                    connects[0].visualMove.AddSpawn(spawnCount- spawn, 0);
            }
        }
        else
        {
            if(connects[0].visualMove as BombVisualMove)
                (connects[0].visualMove as BombVisualMove).Move(connects[connects.Count - 1].visualMove, spawnCount, 0);
            else
                connects[0].visualMove.Move(connects[connects.Count - 1].visualMove, spawnCount, 0);
        }
        for (int i = 1; i < connects.Count - 1; i++)
        {
            if (connects[i].num == connects[connects.Count - 1].num - 1)
            {
                connects[i].visualMove.NoAnimRemove(connects[i].visualMove.speed * i);
            }
            else if (connects[i].coinNode)
            {
                connects[i].EatCoin(connects[0].visualMove.speed * i);
            }
        }
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
                    if (clicked.num >= 1 && clicked.visualMove != null)//node ���ڰ� 1�̻��̸�(�� �ڸ��� ���� ������)
                    {
                        if (connects.Count > 0 && connects[0].num > 0 && connects[0].visualMove != null)//���� �� ���װ� ���°� ������ 
                        {
                            connects[0].visualMove.ToReset();
                            /*foreach (NodeInfo node in connects[0].neighbor)
                            {
                                if (node.num == connects[0].num && node.visualmove != null)
                                {
                                    node.visualmove.warning.SetActive(false);
                                }
                            }*/
                        }
                        connects = new List<NodeInfo>();
                        connects.Add(clicked);
                        connects[0].visualMove.AddPosition(clicked.transform.position + new Vector3(0, 0.06f, 0));
                        click = true;
                        ApplyWarning();
                    }
                }
                else
                {
                    if (!click) return;

                    if (connects.Contains(clicked))//�̹� �������濡 ����
                    {
                        if (connects[connects.Count - 1] != clicked)//���ڸ��� �ƴϸ�(������ �� ������ �� �� �����)
                        {
                            int index = connects.IndexOf(clicked) + 1;
                            connects[0].visualMove.CutUp(index);
                            int countToRemove = connects.Count - index;
                            connects.RemoveRange(index, countToRemove);
                            onOtherNode = true;
                            ApplyWarning();
                        }
                    }
                    else if (connects[connects.Count - 1].neighbor.Contains(clicked) && onOtherNode == true)//���ο� �濡 ����
                    {
                        if (NodeManager.Instance.currentMode.CanDrag(connects[connects.Count - 1], clicked))
                        {
                            if (clicked.num <= 0)//��ĭ�̸�
                            {
                                connects.Add(clicked);
                                connects[0].visualMove.AddPosition(clicked.transform.position + new Vector3(0, 0.06f, 0));
                            }//���ۼ��ڶ� �����ָ�
                            else if (clicked.num == connects[0].num && connects[0].num < 10 && clicked.visualMove != null && clicked.visualMove.warning.activeSelf == false)//�̿��� ����������
                            {
                                connects.Add(clicked);
                                connects[0].visualMove.AddPosition(clicked.transform.position + new Vector3(0, 0.06f, 0));
                                onOtherNode = NodeManager.Instance.currentMode.CanMultiSelect();
                                if (NodeManager.Instance.currentMode.CanMultiSelect())
                                {
                                    ApplyWarning();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    public void ApplyWarning()
    {
        if (click)
        {
            RemoveWarning();
            for (int i = 0; i < connects.Count; i++)
            {
                for (int j = 0; j < connects[i].neighbor.Count; j++)
                {
                    if (connects[i].num == connects[i].neighbor[j].num && connects[i].neighbor[j].visualMove != null && connects[i].neighbor[j].visualMove.warning.activeSelf == false)
                    {
                        warnings.Add(connects[i].neighbor[j].visualMove);
                        connects[i].neighbor[j].visualMove.warning.SetActive(true);
                    }
                }
            }
            if (connects.Count > 0 && connects[0].num == 10)
            {
                foreach (NodeInfo node in NodeManager.Instance.fullNodes)
                {
                    if (node.num == 10 && connects[0] != node)
                    {
                        warnings.Add(node.visualMove);
                        node.visualMove.warning.SetActive(true);
                    }
                }
            }
        }
    }
    private void RemoveWarning()
    {
        for(int i = 0; i < warnings.Count; i++)
        {
            if(warnings[i] != null)
                warnings[i].warning.SetActive(false);
        }
        warnings = new List<VisualMove>();
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
    public void StartGame(bool stage, int mode, StageSettingSO so)
    {
        this.stage = stage;
        this.mode = mode;
        dragCount = 0;
        connects = new List<NodeInfo>();
        RemoveWarning();
        if (stage)
        {
            spawnNodes = new List<SpawnNode>(so.spawnNode);
        }
    }
}
