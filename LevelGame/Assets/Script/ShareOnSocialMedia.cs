using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ShareOnSocialMedia : MonoBehaviour
{
    public static ShareOnSocialMedia instance;
    public GameObject offUIs;
    private void Awake()
    {
        if (instance == null) instance = this;
    }
    public void OnClickShare()
    {
        offUIs.SetActive(false);
        StartCoroutine(TakeScreenShotAndShare());
    }
    IEnumerator TakeScreenShotAndShare()
    {
        yield return new WaitForEndOfFrame();
        Texture2D tx = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        tx.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        tx.Apply();

        string path = Path.Combine(Application.temporaryCachePath, "ScoreImage.png");
        File.WriteAllBytes(path, tx.EncodeToPNG());

        Destroy(tx);

        new NativeShare()
            .AddFile(path)
            .SetSubject("This is my score.")
            .SetText("How much can you do?")
            .Share();


        offUIs.SetActive(true);
    }
}
