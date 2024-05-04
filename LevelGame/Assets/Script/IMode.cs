using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMode 
{
    public void Init();
    public void StartMove();
    public void EndMove();
    public bool CanDrag(NodeInfo start, NodeInfo end) => true;
    public void ResetGame();
}
