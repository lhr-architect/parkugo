using System;
using UnityEngine;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Collections;
using Unity.Netcode;
using Newtonsoft.Json.Linq;
using OriginTcp;
using Unity.VisualScripting;
using HelloWorld;


public class photoClient : NetworkBehaviour{
    
    static public photoClient Instance { get; set; }
    public string LastHitLandmarkName { get; set; }
    public Landmark LastHitLandmark { get; set; }

    public string UsernameGlobal { get; private set; }

    public GameObject UIManagers;

    private string ipAddress;
    private int port;

    private Thread readDataThread;
    private TcpClient tcpClient;
    private NetworkStream stream;
    public Command cmd { get; private set; }

    //将Server给的数据存队列 依次取出
    private Queue<PackedImg> ImgBuffer = new Queue<PackedImg>();

    private bool isConnected;

    public void clearQueue()
    {
        cmd = null;
        ImgBuffer.Clear();
    }

    public void Start()
    {
        Instance = this;
        //UIManagers.GetComponentInChildren<BigmapUIManager>().BigmapPanel.SetActive(true);
    }

    public override void OnNetworkSpawn()
    {
        
        base.OnNetworkSpawn();

        ipAddress = NetworkManager.Singleton.gameObject.GetComponent<ModeManager>().serverIP;
        port = NetworkManager.Singleton.gameObject.GetComponent<ModeManager>().TcpPort;

    }

    public void StartTcp(string userName)
    {
        if (IsClient)
        {
            UsernameGlobal = userName;
            StartCoroutine(ConnectServer());
        }
    }

    private void Update()
    {
        print($"Client connection state: {isConnected}");
        if (ImgBuffer.Count > 0)
        {
            //Debug.LogWarning(UsernameGlobal + "receivephoto");
            if (cmd != null)
            {
                PackedImg u = ImgBuffer.Dequeue();
                UIManagers.GetComponentInChildren<LandmarkUIManager>().SaveImg(u, cmd);
            }
        }
    }

    [Rpc(SendTo.Server)]
    public void GetPhotoRpc(string LmName, string RequestUserName, PlayerController.Party party)
    {
        if (IsServer)
        {   
            //Debug.Log(UsernameGlobal);
            // 调用MyServer类的发送照片方法
            photoServer.Instance.GetAllPhoto(LmName, RequestUserName, party);
        }
    }

    [Rpc(SendTo.Server)]
    public void AddImgRpc(string userName, int pid, string LmName, int UIPosition, PlayerController.Party party)
    {
        if (IsServer)
        {

            StartCoroutine(AddImageCoroutine(userName, pid, LmName, UIPosition, party));
        }
    }

    private IEnumerator AddImageCoroutine(string userName, int pid, string LmName, int UIPosition, PlayerController.Party party)
    {
        yield return new WaitUntil(() => (photoServer.Instance.readThreadCnt <= 1));
        bool notFound = true;
        int cnt = 0;
       // yield return new WatchedList

        while (notFound && cnt < 20)
        {
            // 循环找当前的img
            foreach (var img in photoServer.Instance.server.IMAGES)
            {
                if(img == null) continue;
                if (img.isImg(userName, pid))
                {
                    photoServer.Instance.AddImageToLandmark(LmName, img, UIPosition, party);
                    photoServer.Instance.server.IMAGES.Remove(img);

                    notFound = false;
                    break;
                }
                
            }
            cnt++;
            if (!notFound)
            {
                break;
            }
            // Wait for the next frame before checking again
        }

        photoServer.Instance.readThreadCnt--;
        if (notFound)
        {
            Debug.LogWarning("not found");
        }
    }

    [Rpc(SendTo.Server)]
    public void RemoveLandmarkPhotoRpc(string LmName, int UIposition, PlayerController.Party party)
    {
        if (IsServer)
        {
            photoServer.Instance.RemoveImageInLandmark(LmName, UIposition, party);
        }
    }

    [Rpc(SendTo.Server)]
    public void RemoveLandmarkPhotoRpc(string LmName, string userName, int pid, PlayerController.Party party)
    {
        if (IsServer)
        {
            photoServer.Instance.RemoveImageInLandmark(LmName, userName, pid, party);
        }
    }

    [Rpc(SendTo.Server)]
    public void RemovePhotoAnywayRpc(string userName, int pid, PlayerController.Party party)
    {
        if (IsServer)
        {
            photoServer.Instance.RemovePhotoAnyWay(userName, pid, party);
        }
    }

    //在函数中传参，Server修改，Client使用
    [Rpc(SendTo.Server)]
    public void GetImgUIPositionRpc(string userName, string cmdStr, string LmName, PlayerController.Party party)
    {
        if (IsServer)
        {
            Command cmd = new Command();
            cmdStr  = cmd.ToJsonSelective(photoServer.Instance.GetLandmark(LmName), party);
      
            //Debug.Log(cmdStr);

            GetCommandClientRpc(userName, cmdStr);

            //Debug.Log($"Server to Find Landmark {LmName}  " + cmdStr);
        }
        // 这里调用客户端的方法
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void GetCommandClientRpc(string userName, string cmdString)
    {
        if (IsClient)
        {
           
            if (userName != UsernameGlobal) return;

            cmd = Command.FromJson(cmdString);
            // 客户端接收到服务器返回的字符串
            Debug.Log($"Client received modified string: {cmdString}");
        }
    }


    [Rpc(SendTo.Server)]
    public void tryAddNameToClientRpc(string TcpRemoteEndPoint, string userName)
    {
        if(IsServer)
        {
           bool result = photoServer.Instance.server.tryAddNameToClient(TcpRemoteEndPoint, userName);
           //Debug.Log($"Add user {userName} {result}");
           AddTcpNameCallbackClientRpc(result, userName);
        }

    }

    [Rpc(SendTo.ClientsAndHost)]
    private void AddTcpNameCallbackClientRpc(bool IsAddSucceeded, string userName)
    {
        if(IsClient)
        {
            if (userName != UsernameGlobal) return;

            if (IsAddSucceeded)
            {
                UIManagers.GetComponentInChildren<CameraPage2UIManager>().changePage();
                Debug.Log($"成功连入了 {UsernameGlobal} sb");
            }
            //failure 重新取名
            else
            {
                Debug.Log("滚回去重新取名 sb");
            }
        }
    }


    //连接线程
    IEnumerator ConnectServer()
    {
        tcpClient = new TcpClient();

        tcpClient.BeginConnect(ipAddress, port, ConnectThreadCallBack, tcpClient);

        yield return new WaitUntil(() =>  isConnected);
        
        //等待客户端连接后，去找TcpClient绑定userName
        tryAddNameToClientRpc(tcpClient.Client.RemoteEndPoint.ToString(), UsernameGlobal);

    }


    private void ConnectThreadCallBack(IAsyncResult result)
    {
        tcpClient = result.AsyncState as TcpClient;
        if (tcpClient.Connected)
        {
            tcpClient.EndConnect(result);
            stream = tcpClient.GetStream();

            isConnected = true;

            //将userName和Tc关联

            //确认连接到server后，起一个线程处理读入
            readDataThread = new Thread(ReadDataThread);
            readDataThread.Start();
        }
    }

    //读取数据线程
    private void ReadDataThread()
    {
        try
        {
            byte[] buffer = new byte[1024 * 1024];
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                ImgBuffer.Enqueue(PackedImg.decode2Class(buffer));

            }
        }
        catch (Exception e)
        {
            print(e.Message);
        }
    }
    
    public void SendImg2Server(PackedImg u)
    {
        byte[] buffer = u.encode2Bytes();
        //Debug.Log("c;oe")
        stream?.Write(buffer, 0, buffer.Length);
    }

    public override void OnDestroy()
    {
        
        stream?.Close();
        tcpClient?.Close();
        readDataThread?.Abort();

        base.OnDestroy();
    }
}
