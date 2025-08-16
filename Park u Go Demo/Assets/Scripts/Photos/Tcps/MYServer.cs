using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Diagnostics;
using Unity.VisualScripting.FullSerializer;
using Unity.Netcode;


namespace OriginTcp
{

    public class MYServer 
    {

        private int port = 8888;
        private bool isThreadRunning;

        private List<PackedImg> imageBuffer = new List<PackedImg>();
        public List<PackedImg> IMAGES {  get { return imageBuffer; } }

        private List<MYClient> clientsList = new List<MYClient>();
        private Dictionary<string, MYClient> clientTcpMap = new Dictionary<string, MYClient>();


        //����ʱ����Tcp�˿�
        public MYServer(int _port) { port = _port; }

        //����˳�ʼ��
        public void Init()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            isThreadRunning = true;
            try
            {
                while (isThreadRunning)
                {
                    TcpClient client = listener.AcceptTcpClient();
                   
                    MYClient clientInstance = new MYClient(client, this);
                    
                    clientsList.Add(clientInstance);
                    
                }
            }
            catch (ThreadAbortException e)
            {
                _ = e.Message;
            }

            catch (Exception ex)
            {
                _ = ex.Message;
            }
            finally
            {

            }
        }

        /// <summary>
        /// �㲥�������пͻ��˷�������
        /// </summary>
        /// <param name="data"></param>
        public void Broadcast(byte[] data)
        {
            foreach (var key in clientTcpMap.Keys)
            {
                clientTcpMap[key].Send(data);
            }

        }

        public void sendTo(string userName, byte[] data)
        {
            if (clientTcpMap.ContainsKey(userName))
            {
                clientTcpMap[userName].Send(data);
            }
            else
            {
                
            }
        }

        public void Broadcast(string data)
        {

            Broadcast(Encoding.Default.GetBytes(data));
        }




        /// <summary>
        /// �ڷ��������������һ��PackedImg
        /// </summary>
        /// <param name="img"></param>
        public void AddImg2buffer(PackedImg img)
        {
            imageBuffer.Add(img);

        }

        public bool tryAddNameToClient(string TcpRemoteEndPoiont, string userName)
        {
            foreach (var client in clientsList)
            {
                if(client.tcpClient.Client.LocalEndPoint.ToString().Equals(TcpRemoteEndPoiont))
                {
                    client.userName = userName;
                    clientTcpMap.Add(userName, client);
                    clientsList.Remove(client);
                    return true;
                }

            }
            return false;
        }



        /// <summary>
        /// �Ƴ��ͻ���
        /// </summary>
        /// <param name="client"></param>
        public void Remove(MYClient client)
        {
            if (clientsList.Contains(client))
            {
                clientsList.Remove(client);
            }
        }
    }


}