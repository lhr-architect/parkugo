using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    string CameraName;

    public Button btnShot;
    public Button btnSave;
    //必须使用左下角锚点模式
    public AlbumUIManager albumUiManager;

    [SerializeField] RawImage webCamImage;
    WebCamTexture pics;
    static int Clientpid = 0;

    private PackedImg innerImgBuffer;
    public PackedImg ImgBuffer {
        get
        {
            return innerImgBuffer;
        }
    }

    private void Start()
    {
        btnShot.onClick.AddListener(TakePic);
        btnSave.onClick.AddListener(delegate {
            if (ImgBuffer != null)
            { 
                StartCoroutine(SaveImages(ImgBuffer)); 
            }
        });

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
        else
        {

        }
    }

    public void TakePic()
    {
        StartCoroutine(TakePic0());
    }

    IEnumerator TakePic0()
    {

        yield return new WaitForEndOfFrame();

        //To do 截到屏幕指定的位置
        Rect rect = new Rect(100, 1400, 800, 600);

        Texture2D tex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);

        tex.ReadPixels(rect, 0, 0);
        tex.Apply();


        //拿到本地玩家的位置
        Vector2 playerPosition = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().GetPosition();

        // 拍照后存入 ImgBuffer
        byte [] data = tex.EncodeToPNG();
        innerImgBuffer = new PackedImg(photoClient.Instance.UsernameGlobal, data, (int)rect.width, (int)rect.height, ++Clientpid, playerPosition.x, playerPosition.y);
        
        
    }
   
    IEnumerator SaveImages(PackedImg img)
    {
        yield return new WaitForEndOfFrame();
        byte[] data = img.ImageData;

        Texture2D tex = new Texture2D(img.width, img.height);
        tex.LoadImage(data);

        
        // album ui API
        if(!albumUiManager.tryPutPhoto(img))
        {
            //to do 弹窗显示
            Debug.LogWarning("Your Photo is Full bro");
        }

        pics.Play();

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