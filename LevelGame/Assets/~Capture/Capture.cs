using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Capture : MonoBehaviour
{
    public RenderTexture DrawTexture;   //PNG저장할 타겟 렌더 텍스쳐

    [ContextMenu("shot")]
    void RenderTextureSave()
    {
        RenderTexture.active = DrawTexture;
        var texture2D = new Texture2D(DrawTexture.width, DrawTexture.height);
        texture2D.ReadPixels(new Rect(0, 0, DrawTexture.width, DrawTexture.height), 0, 0);
        texture2D.Apply();
        var data = texture2D.EncodeToPNG();
        File.WriteAllBytes("C:/Image/Capture.png", data);
    }
}