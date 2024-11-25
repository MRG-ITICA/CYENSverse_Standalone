using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

public class VideoTest : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer videoPlayer;

    // Start is called before the first frame update
    void Start()
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                // Get the root of the external storage
                using (AndroidJavaObject externalStorageDir = currentActivity.Call<AndroidJavaObject>("getExternalFilesDir", (string)null))
                {
                    if (externalStorageDir != null)
                    {
                        string rootPath = externalStorageDir.Call<string>("getAbsolutePath");
                        rootPath = rootPath.Substring(0, rootPath.IndexOf("/Android"));
                        videoPlayer.url = Path.Combine(rootPath, "Movies/papakaliatis.mp4");
                        Debug.Log("playing video: " + videoPlayer.url);

                        videoPlayer.Play();

                    }
                    else
                    {
                        Debug.LogError("Failed to get the external storage directory.");
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
