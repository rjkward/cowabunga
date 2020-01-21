using UnityEngine;

public class QuitGame : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        bool quit = Input.GetKey(KeyCode.Escape);
        if (quit)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
