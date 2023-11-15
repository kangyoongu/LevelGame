using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VisualMove : MonoBehaviour
{
    public List<Vector3> positions = new List<Vector3>();
    public float speed = 3;

    private SpriteRenderer spriteRenderer;
    private LineRenderer lineRenderer;

    [HideInInspector] public GameObject warning;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lineRenderer = GetComponent<LineRenderer>();
        warning = transform.GetChild(0).gameObject;
    }
    private void Start()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutElastic);
        lineRenderer.positionCount = 0; // ����Ʈ ���� ���� (�������� ����)
    }
    public void SetColor()//�ڽ� ĭ�� ���� ���� �� �ٲ�
    {
        int value = transform.parent.GetComponent<NodeInfo>().num;
        Color color = NodeManager.instance.nodeColor[value];
        spriteRenderer.color = color;

        Gradient gradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[1];
        colorKeys[0].color = color;
        colorKeys[0].time = gradient.colorKeys[0].time;
        // �׶���Ʈ�� ���ο� ���� Ű �迭�� ����
        gradient.SetKeys(colorKeys, gradient.alphaKeys);

        // ����� �׶���Ʈ�� ������ ����
        lineRenderer.colorGradient = gradient;
    }
    public IEnumerator Move(VisualMove target, int makeNum)//Ÿ�� ��ġ�� ������
    {
        for(int i = 0; i < positions.Count; i++)
        {
            transform.DOMove(positions[i], speed).SetEase(Ease.Linear);

            if (i > 0)
            {
                yield return new WaitForSeconds(speed * 0.35f);
                for (int j = 0; j < lineRenderer.positionCount - 1; j++)
                {
                    lineRenderer.SetPosition(j, lineRenderer.GetPosition(j + 1));
                }
                yield return new WaitForSeconds(speed * 0.65f);
            }
        }
        for (int i = 0; i < makeNum; i++)
        {
            NodeManager.instance.MakeNode();
        }
        ToReset();
        target.SetColor();
        NodeManager.instance.blankNode.Add(GetComponentInParent<NodeInfo>());
        yield return new WaitForEndOfFrame();
        NodeManager.instance.EndCheck();
        Destroy(gameObject);
    }
    public void ToReset()
    {
        positions = new List<Vector3>();
        lineRenderer.positionCount = 0;
        GameManager.instance.onOtherNode = true;
    }
    public void AddPosition(Vector3 pos)//linerenderer �� �߰�
    {
        positions.Add(pos);
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
    }
    public void CutUp(int index)//���°� ���ķ� �� �� �ڸ�
    {
        lineRenderer.positionCount = index;
        Vector3[] x = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(x);
        positions = new List<Vector3>(x);
    }
    public IEnumerator Disapear()
    {
        NodeManager.instance.blankNode.Add(transform.parent.GetComponent<NodeInfo>());
        float delay = Random.Range(0f, 1.5f);
        yield return new WaitForSeconds(delay);
        transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InElastic).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}
