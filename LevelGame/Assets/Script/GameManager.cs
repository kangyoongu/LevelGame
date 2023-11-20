using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Camera mainCamera;

    bool click = false;
    List<NodeInfo> connects = new List<NodeInfo>();
    [HideInInspector] public bool onOtherNode = true;
    [HideInInspector] public bool canMove = false;
    public int max = 5;

    public GameObject[] particles;
    public void Awake()
    {
        PlayerPrefs.DeleteAll();
        if (instance == null) instance = this;
        if (!PlayerPrefs.HasKey("Best"))
        {
            PlayerPrefs.SetInt("Best", 0);
        }
        Application.targetFrameRate = 60;
    }
    void Update()
    {
        if (Input.touchCount == 1 && !IsPointerOverUIObject(Input.GetTouch(0).position))//터치가 1개만 있으면
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
                if (connects[0].num == 10)
                {
                    foreach (NodeInfo node in NodeManager.instance.fullNodes)
                    {
                        if (node.num == 10)
                        {
                            node.visualmove.warning.SetActive(false);
                        }
                    }
                }
                else
                {
                    foreach (NodeInfo node in connects[0].neighbor)
                    {
                        if (node.num == connects[0].num)
                        {
                            node.visualmove.warning.SetActive(false);
                        }
                    }
                }
                click = false;
                if (connects[0].num == connects[connects.Count - 1].num && connects.Count >= 2)
                {
                    connects[0].num = 0;
                    connects[connects.Count - 1].num++;
                    NodeManager.instance.Score += connects[connects.Count - 1].num;
                    if (connects[connects.Count - 1].num > max)//최고기록 갱신하면 하나 더 나옴
                    {
                        max = connects[connects.Count - 1].num;
                        if(max == 10)
                        {
                            particles[0].SetActive(true);
                            particles[1].SetActive(true);
                        }
                        StartCoroutine(connects[0].visualmove.Move(connects[connects.Count - 1].visualmove, 2));
                    }
                    else
                    {
                        StartCoroutine(connects[0].visualmove.Move(connects[connects.Count - 1].visualmove, 1));
                    }
                }
                else
                {
                    onOtherNode = true;
                    connects[0].visualmove.ToReset();
                }
            }
        }
    }

    private void OnMouseClick()
    {
        Touch touch = Input.GetTouch(0); // 첫 번째 터치 입력 사용

        Vector2 touchPosition = mainCamera.ScreenToWorldPoint(touch.position);

        // Raycast를 사용하여 터치 위치에서 스프라이트를 선택
        RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

        if (hit.collider != null)//뭔갈 눌렀으면 
        {
            GameObject selectedSprite = hit.collider.gameObject;//가져오고
            if (selectedSprite.CompareTag("Node")) //태그가 node면
            {
                NodeInfo clicked = selectedSprite.GetComponent<NodeInfo>();//nodeinfo가져옴

                if (touch.phase == TouchPhase.Began)//첫클릭이면
                {
                    if (clicked.num >= 1)//node 숫자가 1이상이면
                    {
                        if (connects.Count > 0 && connects[0].visualmove != null)
                        {
                            connects[0].visualmove.ToReset();
                            foreach (NodeInfo node in connects[0].neighbor)
                            {
                                if (node.num == connects[0].num)
                                {
                                    node.visualmove.warning.SetActive(false);
                                }
                            }
                        }
                        connects = new List<NodeInfo>();
                        connects.Add(clicked);
                        connects[0].visualmove.AddPosition(clicked.transform.position);
                        click = true;
                        if (connects[0].num == 10)
                        {
                            foreach (NodeInfo node in NodeManager.instance.fullNodes)
                            {
                                if (node.num == 10 && node != clicked)
                                {
                                    node.visualmove.warning.SetActive(true);
                                }
                            }
                        }
                        else
                        {
                            if (connects.Count > 0 && connects[0].visualmove != null)
                            {
                                foreach (NodeInfo node in connects[0].neighbor)
                                {
                                    if (node.num == connects[0].num)
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
                        if (connects.Contains(clicked))//이미 지나간길에 가면
                        {
                            if (connects[connects.Count - 1] != clicked)//제자리도 아니면
                            {
                                int index = connects.IndexOf(clicked) + 1;
                                connects[0].visualmove.CutUp(index);
                                int countToRemove = connects.Count - index;
                                connects.RemoveRange(index, countToRemove);
                                onOtherNode = true;
                            }
                        }
                        else if (connects[connects.Count - 1].neighbor.Contains(clicked) && onOtherNode == true)//새로운 길에 가면
                        {
                            if (clicked.num <= 0)
                            {
                                connects.Add(clicked);
                                connects[0].visualmove.AddPosition(clicked.transform.position);
                            }
                            else if (clicked.num == connects[0].num && !connects[0].neighbor.Contains(clicked) && connects[0].num < 10)//이웃은 안합쳐지게
                            {
                                connects.Add(clicked);
                                connects[0].visualmove.AddPosition(clicked.transform.position);
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
}
