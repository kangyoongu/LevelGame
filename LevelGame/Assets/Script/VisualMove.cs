using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VisualMove : MonoBehaviour
{
    public List<Vector3> positions = new List<Vector3>();
    public float speed = 3;

    private MeshRenderer meshRenderer;
    private LineRenderer lineRenderer;

    [HideInInspector] public GameObject warning;
    public AudioClip clips;
    private ParticleSystem particle;
    [HideInInspector] public ParticleSystem bombParticle;
    int value;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        lineRenderer = GetComponent<LineRenderer>();
        warning = transform.GetChild(0).gameObject;
        particle = transform.GetChild(2).GetComponent<ParticleSystem>();
        bombParticle = transform.GetChild(3).GetComponent<ParticleSystem>();
    }
    private void Start()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(new Vector3(1.2f, 0.8f, -1f), 0.3f).SetEase(Ease.OutBack);
        transform.DOScale(new Vector3(1f, 1f, -1f), 0.7f).SetEase(Ease.OutElastic).SetDelay(0.3f);
        lineRenderer.positionCount = 0; // 포인트 개수 설정 (시작점과 끝점)
    }
    public void SetColor()//자신 칸의 값에 따라 색 바꿈
    {
        NodeInfo parentNodeInfo = transform.parent.GetComponent<NodeInfo>();
        value = parentNodeInfo.num;
        if (value == 0) return;
        if (meshRenderer.material != NodeManager.Instance.currentMat)
        {
            meshRenderer.material = NodeManager.Instance.currentMat;
        }
        //transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = NodeManager.instance.inside[value - 1];
        Color color = NodeManager.Instance.nodeColor[value-1];
        meshRenderer.material.color = color;
        warning.GetComponent<SpriteRenderer>().color = NodeManager.Instance.warningColor;
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[1];
        colorKeys[0].color = color;
        colorKeys[0].time = gradient.colorKeys[0].time;
        // 그라디언트를 새로운 색상 키 배열로 설정
        gradient.SetKeys(colorKeys, gradient.alphaKeys);

        // 변경된 그라디언트를 재질에 적용
        lineRenderer.colorGradient = gradient;
    }


    public void Move(VisualMove target, int makeNum, int makeLevel)
    {
        StartCoroutine(MoveCoroutine(target, makeNum, makeLevel));
    }
    IEnumerator MoveCoroutine(VisualMove target, int makeNum, int makeLevel)//합치면 그 위치로 움직임
    {
        RandomPitchPlay rand = GetComponent<RandomPitchPlay>();
        rand.Play(clips);
        particle.startColor = NodeManager.Instance.nodeColor[value-1];
        particle.Play();
        for(int i = 0; i < positions.Count-1; i++)
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

        #region 쫄깃한 애니메이션
        if (target == null)
        {
            Destroy(gameObject);
            yield break;
        }
        Vector3 dir = transform.position - positions[positions.Count - 1];
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        if (angle < 0)
        {
            angle += 360f;
        }
        angle /= 60;
        angle = Mathf.Round(angle);
        transform.localRotation = Quaternion.Euler(0, 0, angle * -60);
        target.transform.localRotation = Quaternion.Euler(0, 0, angle * -60);
        transform.DOMove(positions[positions.Count - 1] + transform.right * 0.25f, 0.6f).SetEase(Ease.OutQuad);
        transform.DOScale(new Vector3(0.7f, 1.1f, -1f), 0.6f).SetEase(Ease.OutQuad);
        Vector3 targetPos = target.transform.position;
        target.transform.DOMove(target.transform.position + target.transform.right * -0.3f, 0.6f).SetEase(Ease.OutQuad);
        target.transform.DOScale(new Vector3(0.7f, 1.1f, -1f), 0.6f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(0.6f * 0.27f);
        lineRenderer.positionCount = 0;
        yield return new WaitForSeconds(0.6f * 0.73f);

        if (target == null)
        {
            Destroy(gameObject);
            yield break;
        }//이거 되는 중간에 다시하기 하면 생기는 버그 방지
        target.transform.position = targetPos;
        target.transform.localScale = new Vector3(1.2f, 1f, -1f);
        target.transform.DOScale(new Vector3(1.5f, 1.1f, -1f), 0.1f).SetEase(Ease.OutCubic);
        target.transform.DOScale(new Vector3(0.8f, 1.3f, -1f), 0.1f).SetEase(Ease.OutCubic).SetDelay(0.1f);
        target.transform.DOScale(new Vector3(1f, 1f, -1f), 0.6f).SetEase(Ease.OutElastic).SetDelay(0.2f);
        particle.transform.parent = null;
        particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        ToReset();
        #endregion

        target.SetColor();
        target.bombParticle.startColor = NodeManager.Instance.nodeColor[value];
        target.bombParticle.Play();
        if (GameManager.Instance.stage)
        {
             NodeManager.Instance.CheckClear();
        }

        NodeInfo parent = GetComponentInParent<NodeInfo>();
        NodeManager.Instance.blankNode.Add(parent);
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        PublicAudio.Instance.merge.Play();
        float spawnDelay = 0.5f;
        for (int i = 0; i < makeNum; i++)
        {
            NodeManager.Instance.MakeNode(spawnDelay, makeLevel);
        }
        yield return new WaitForEndOfFrame();
        if(GameManager.Instance.canMove)
            NodeManager.Instance.EndCheck(false);

        Destroy(gameObject);
    }
    public void ToReset()
    {
        positions = new List<Vector3>();
        lineRenderer.positionCount = 0;
        GameManager.Instance.onOtherNode = true;
    }
    public void AddPosition(Vector3 pos)//linerenderer 값 추가
    {
        positions.Add(pos);
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
    }
    public void CutUp(int index)//들어온거 이후로 값 다 자름
    {
        lineRenderer.positionCount = index;
        Vector3[] x = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(x);
        positions = new List<Vector3>(x);
    }
    public IEnumerator Disapear()
    {
        NodeInfo parent = transform.parent.GetComponent<NodeInfo>();
        NodeManager.Instance.blankNode.Add(parent);
        float delay = Random.Range(0f, 1.5f);
        yield return new WaitForSeconds(delay);
        NodeManager.Instance.PopSoundPlay();
        parent.num = 0;
        transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InElastic);
        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }
}