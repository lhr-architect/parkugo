using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class CameraControllerFake : MonoBehaviour
{
    string CameraName;
    [SerializeField] RawImage webCamImage;
    WebCamTexture pics;


    private void Start()
    {


        //开启安卓权限
        if (Application.platform == RuntimePlatform.Android)
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Permission.RequestUserPermission(Permission.Camera);
            }
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageRead);
            }
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            }
        }

        StartCoroutine(OpenCamera());
    }

    private IEnumerator CheckCameraDevices()
    {
        yield return new WaitForEndOfFrame();
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.Log("No camera devices found.");
        }
        
    }

    IEnumerator OpenCamera()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        yield return CheckCameraDevices();

        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {

            WebCamDevice[] devices = WebCamTexture.devices;
            CameraName = devices[0].name;

            pics = new WebCamTexture(CameraName, 0, 0);
            webCamImage.texture = pics;
            pics.Play();
        }

    }




   

    /// <summary>

    /// </summary>
    /// <param name="path"></param>
    private void OnSaveImagesPlartform(string filePath)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            string[] paths = new string[1];
            paths[0] = filePath; 
            using (AndroidJavaClass PlayerActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject playerActivity = PlayerActivity.GetStatic<AndroidJavaObject>("currentActivity");
                using (AndroidJavaObject Conn = new AndroidJavaObject("android.media.MediaScannerConnection", playerActivity, null))
                {
                    Conn.CallStatic("scanFile", playerActivity, paths, null, null);
                }
            }
#endif
    }


}