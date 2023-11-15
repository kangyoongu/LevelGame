using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeInfo : MonoBehaviour
{
    public int index;
    public int num = 0;
    public List<NodeInfo> neighbor;
    [HideInInspector]public VisualMove visualmove;
    public int blankIndex = 0;

    private void Update()
    {
        if(num >= 1)
        {
            if(visualmove == null)visualmove = transform.GetChild(0).GetComponent<VisualMove>();
        }
        else
        {
            visualmove = null;
        }
    }
}
