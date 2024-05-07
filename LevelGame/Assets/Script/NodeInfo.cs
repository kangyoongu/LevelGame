using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeInfo : MonoBehaviour
{
    public int index;
    public int num = 0;
    public List<NodeInfo> neighbor;
    [HideInInspector]public VisualMove visualMove;
    public int blankIndex = 0;

    private void Update()
    {
        if(num >= 1 && transform.childCount > 0)
        {
            if (visualMove == null)
            {
                visualMove = transform.GetChild(0).GetComponent<VisualMove>();
                GameManager.Instance.ApplyWarning();
            }
        }
        else
        {
            visualMove = null;
        }
    }
}
