using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMode : MonoBehaviour, IMode
{
    public NodeInfo[] centerNodes;
    public GameObject block;
    List<Block> blocks;
    public void Init()
    {
        blocks = new List<Block>();
        EndMove();
    }
    public void StartMove()
    {
        return;
    }
    public void EndMove()
    {
        for(int i = 0; i < blocks.Count; i++)
        {
            blocks[i].Disable();
        }
        blocks = new List<Block>();

        for (int i = 0; i < centerNodes.Length; i++)
        {
            for(int j = 0; j < centerNodes[i].neighbor.Count; j++)
            {
                if(Random.Range(0, 10) < 3)
                {
                    float angle;
                    switch (j)
                    {
                        case 0:
                        case 5:
                            angle = 0f;
                            break;
                        case 1:
                        case 4:
                            angle = 60f;
                            break;
                        default:
                            angle = 120f;
                            break;
                    }
                    Vector3 pos = (centerNodes[i].transform.position + centerNodes[i].neighbor[j].transform.position) * 0.5f;
                    pos.y -= 0.25f;
                    Block b = Instantiate(block, pos, Quaternion.Euler(0f, angle, 0f)).GetComponent<Block>();
                    b.Enable();
                    b.Init(centerNodes[i].index, centerNodes[i].neighbor[j].index);
                    blocks.Add(b);
                }
            }
        }
    }
    public bool CanDrag(NodeInfo start, NodeInfo end)
    {
        bool value = true;
        for (int i = 0; i < blocks.Count; i++)
        {
            if ((blocks[i].startIndex == start.index && blocks[i].endIndex == end.index) || (blocks[i].startIndex == end.index && blocks[i].endIndex == start.index))
            {
                value = false;
                break;
            }
        }
        return value;
    }

    public void ResetGame()
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            blocks[i].Disable();
        }
        blocks = new List<Block>();
    }
}
