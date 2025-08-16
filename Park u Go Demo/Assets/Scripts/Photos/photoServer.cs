using HelloWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class photoServer : NetworkBehaviour
{

    static public photoServer Instance;

    public OriginTcp.MYServer server;
    Thread serverThread;
    public int readThreadCnt = 0;

    private Dictionary<string, Landmark> landmarks;


    // 给指定的Landmark添加PackedImg
    public void AddImageToLandmark(string landmarkName, PackedImg packedImg, int UIIndex, PlayerController.Party party)
    {
        if (landmarks.TryGetValue(landmarkName, out Landmark landmark))
        {
            landmark.tryAddImageAt(packedImg, UIIndex, party);
            return;
        }
        else
        {
            return;
        }
    }

    public bool tryAddImageToLandmark(string landmarkName, PackedImg packedImg, int UIIndex, PlayerController.Party party)
    {
        if (landmarks.TryGetValue(landmarkName, out Landmark landmark))
        {
            landmark.tryAddImageAt(packedImg, UIIndex, party);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RemoveImageInLandmark(string landmarkName, int UIIndex, PlayerController.Party party)
    {
        if (landmarks.TryGetValue(landmarkName, out Landmark landmark))
        {
            landmark.RemoveImageAt(UIIndex, party);
        }
        else
        {
            Debug.LogWarning($"Landmark {landmarkName} not found.");
        }
    }

    //遍历一遍，把服务器中的照片删了0
    public void RemovePhotoAnyWay(string userName, int pid, PlayerController.Party party)
    {
        foreach (var value in landmarks.Values)
        {
            if(value.tryRemoveImage(userName, pid, party)) return;
        }
    }

    public void RemoveImageInLandmark(string landmarkName, string userName, int pid, PlayerController.Party party)
    {
        if (landmarks.TryGetValue(landmarkName, out Landmark landmark))
        {
            landmark.tryRemoveImage(userName, pid, party);
        }
        else
        {
            Debug.LogWarning($"Landmark {landmarkName} not found.");
        }
    }


    // Start is called before the first frame update
    photoServer()
    {
        Instance = this;
    }


    [Rpc(SendTo.ClientsAndHost)] 
    public void burnImgRpc(string cmdStr)
    {
        if (IsClient)
        {
            Command cmd = Command.FromJson(cmdStr);
            for (int i = 0; i < cmd.count; i++)
            {
                Debug.LogWarning($"on Client to burn img: {cmd.userNames[i]} {cmd.pids[i]}");
                photoClient.Instance.UIManagers.GetComponentInChildren<AlbumUIManager>().BurnPossibleImg(cmd.userNames[i], cmd.pids[i]);
            }
        }
    }


    void Start()
    {
        //起一个Server
        server = new OriginTcp.MYServer(NetworkManager.gameObject.GetComponent<ModeManager>().TcpPort);

        serverThread = new Thread(server.Init);
        serverThread.Start();

        landmarks = new Dictionary<string, Landmark>();
        RegisterLandmarksInScene();

    }

    private void RegisterLandmarksInScene()
    {
        Landmark[] allLandmarks = FindObjectsOfType<Landmark>();
        foreach (Landmark LmInScene in allLandmarks)
        {
            if (!landmarks.ContainsKey(LmInScene.LandmarkName))
            {
/*                Debug.Log($"Find  Landmark {LmInScene.Name} In Scene ");*/
                landmarks.Add(LmInScene.LandmarkName, LmInScene.GetComponent<Landmark>());
            }
            else
            {
                Debug.Log($"Landmark {LmInScene.LandmarkName} already exists in the dictionary.");
            }
        }
    }

    public Landmark GetLandmark(string landmarkName)
    {
        if (landmarks.ContainsKey(landmarkName))
        {
            return landmarks[landmarkName];
        }
        else
        {
            Debug.Log($"Not found {landmarkName} in Dic");
            return null;
        }
    }

    public void BurnImgIn(Landmark landmark, PlayerController.Party winParty)
    {
        Command cmd = new Command();
        string cmdStr = cmd.ToJsonBoth(landmark);
        burnImgRpc(cmdStr);
    }

    public void GetAllPhoto(string landmarkName,string userName, PlayerController.Party party)
    {

        if(landmarks.ContainsKey(landmarkName))
        {
            if(party == PlayerController.Party.RED)
            {
                foreach (var img in landmarks[landmarkName].red)
                {
                    if (img != null)
                    {
                        //Debug.Log($"server get request from red {userName}");
                        server.sendTo(userName, img.encode2Bytes());
      
                    }
                }
            }
            else
            {
                foreach (var img in landmarks[landmarkName].blue)
                {
                    if (img != null)
                    {
                        //Debug.Log($"server get request from blue {userName}");
                        //server.Broadcast(img.encode2Bytes());
                        server.sendTo(userName, img.encode2Bytes());
                    }
                }
            }

        }
        else
        {
            Debug.LogWarning($"In Func SendPhoto Server not contains {landmarkName}");
        }
    }

    public override void OnDestroy()
    {
        serverThread?.Abort();
        base.OnDestroy();
    }
}
