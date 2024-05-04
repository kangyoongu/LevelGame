using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VisualMoveBase : MonoBehaviour
{
    protected void Start()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(new Vector3(1.2f, 0.8f, -1f), 0.3f).SetEase(Ease.OutBack);
        transform.DOScale(new Vector3(1f, 1f, -1f), 0.7f).SetEase(Ease.OutElastic).SetDelay(0.3f);
    }


    public IEnumerator Remove(float randomRange = 1.5f)
    {
        NodeInfo parent = transform.parent.GetComponent<NodeInfo>();
        NodeManager.Instance.blankNode.Add(parent);
        float delay = Random.Range(0f, randomRange);
        yield return new WaitForSeconds(delay);
        NodeManager.Instance.PopSoundPlay();
        parent.num = 0;
        transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InElastic);
        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }
}