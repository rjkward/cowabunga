using System.Collections;
using UnityEngine;

public class SkyboxCapture : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        StartCoroutine(Cap());
    }
    
    IEnumerator Cap()
    {
        yield return null;
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        Capture("forward");
        yield return null;
        transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        Capture("right");
        yield return null;
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        Capture("back");
        yield return null;
        transform.rotation = Quaternion.Euler(0f, 270f, 0f);
        Capture("left");
        yield return null;
        transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        Capture("up");
        yield return null;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        Capture("down");
    }

    private void Capture(string screenName)
    {
        Debug.Log(screenName);
        ScreenCapture.CaptureScreenshot("/Users/rob/weird_shit/screens/" +
                                        screenName + ".png");
    }
}
