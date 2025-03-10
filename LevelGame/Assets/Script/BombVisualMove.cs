using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BombVisualMove : VisualMove
{
    public void BombSetColor()
    {
        if (meshRenderer.material != NodeManager.Instance.currentMat)
        {
            meshRenderer.material = NodeManager.Instance.currentMat;
        }
        Color color = Color.black;
        meshRenderer.material.color = color;
        NodeInfo parentNodeInfo = transform.parent.GetComponent<NodeInfo>();
        if(parentNodeInfo.num != 0)
            value = parentNodeInfo.num;
        icon.sprite = ThemeManager.Instance.CurrentTheme.sprites[value - 1];
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
    public new void Move(VisualMove target, int makeNum, int makeLevel)
    {
        StartCoroutine(BombMoveCoroutine(target, makeNum, makeLevel));
    }
    IEnumerator BombMoveCoroutine(VisualMove target, int makeNum, int makeLevel)//합치면 그 위치로 움직임
    {
        RandomPitchPlay rand = GetComponent<RandomPitchPlay>();
        rand.Play(clips);
        NodeManager.Instance.OnStartMode?.Invoke();
        particle.Play();
        for (int i = 0; i < positions.Count - 1; i++)
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

        NodeManager.Instance.RemoveCoin();

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
        transform.localRotation = Quaternion.Euler(0, 0, angle * 60f);
        meshRenderer.transform.localRotation = Quaternion.Euler(0, 0, -angle * 60f);
        target.transform.localRotation = Quaternion.Euler(0, 0, angle * 60f);
        target.meshRenderer.transform.localRotation = Quaternion.Euler(0, 0, -angle * 60f);
        transform.DOMove(positions[positions.Count - 1] + transform.right * 0.25f, 0.6f).SetEase(Ease.OutQuad);
        transform.DOScale(new Vector3(0.7f, 1.1f, 1f), 0.6f).SetEase(Ease.OutQuad);
        Vector3 targetPos = target.transform.position;
        target.transform.DOMove(target.transform.position + target.transform.right * -0.3f, 0.6f).SetEase(Ease.OutQuad);
        target.transform.DOScale(new Vector3(0.7f, 1.1f, 1f), 0.6f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(0.6f * 0.27f);
        lineRenderer.positionCount = 0;
        yield return new WaitForSeconds(0.6f * 0.73f);

        if (target == null)
        {
            Destroy(gameObject);
            yield break;
        }//이거 되는 중간에 다시하기 하면 생기는 버그 방지
        transform.position = targetPos;
        transform.localScale = new Vector3(1.2f, 1f, 1f);
        transform.DOScale(new Vector3(1.5f, 1.1f, 1f), 0.1f).SetEase(Ease.OutCubic);
        transform.DOScale(new Vector3(0.8f, 1.3f, 1f), 0.1f).SetEase(Ease.OutCubic).SetDelay(0.1f);
        transform.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutElastic).SetDelay(0.2f);
        particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        ToReset();
        #endregion

        NodeInfo parent = GetComponentInParent<NodeInfo>();
        value = target.gameObject.GetComponentInParent<NodeInfo>().num;
        bombParticle.Play();

        SetColor();
        NodeManager.Instance.blankNode.Add(parent);
        target.meshRenderer.enabled = false;
        PublicAudio.Instance.merge.Play();
        int destroyCount = Mathf.Max(0, KillNeighbor(target) + makeNum - 1);
        if (GameManager.Instance.stage && destroyCount <= 0)
        {
            NodeManager.Instance.CheckClear();
        }

        for (int i = 0; i < destroyCount; i++)
        {
            spawnList.Enqueue(makeLevel);
        }
        Spawn();
        NodeManager.Instance.OnEndMove?.Invoke();
        NodeManager.Instance.SpawnCoin();
        yield return new WaitForEndOfFrame();
        /*if (GameManager.Instance.canMove)
            NodeManager.Instance.EndCheck(false);
        else GameManager.Instance.canMove = true;*/
        GameManager.Instance.canMove = true;
        if (GameManager.Instance.stage)
            NodeManager.Instance.CheckClear();
        else
            NodeManager.Instance.EndCheck(false);
        transform.parent = target.transform.parent;
        Destroy(target.gameObject);
    }

    public int KillNeighbor(VisualMove target)
    {
        int popCount = 0;
        NodeInfo targetParent = target.GetComponentInParent<NodeInfo>();
        foreach (NodeInfo node in targetParent.neighbor)
        {
            if (node.num > 0)
            {
                node.visualMove.Remove(0f);
                popCount++;
            }
        }
        return popCount;
    }
}