using System.Text;
using System.Net.Sockets;
using System.Threading;
using System;


namespace OriginTcp
{
    public class MYClient 
    {
        private MYServer server;
        public string userName;
        public TcpClient tcpClient;
        private NetworkStream stream;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tcpClient"></param>
        /// <param name="server"></param>
        public MYClient(TcpClient tcpClient, MYServer server)
        {
   
            this.server = server;
            this.tcpClient = tcpClient;
            stream = tcpClient.GetStream();
            //启动线程 读取数据
            Thread thread = new Thread(ReadImageThread);
            thread.Start();
        }
        private void ReadImageThread()
        {
            try
            {
                byte[] buffer = new byte[1024 * 1024];
                int bytesRead;
                
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                        server.AddImg2buffer(PackedImg.decode2Class(buffer));
                }
      
            }
            catch (Exception error)
            {
                Console.WriteLine(error.ToString());
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        public void Send(byte[] data)
        {
            //Debug.Log($"Server to boradcast {PackedImg.decode2Class(data).ToString()}");
            stream.Write(data, 0, data.Length);
        }

        public void Send(string str)
        {
            byte[] buffer =  Encoding.Default.GetBytes(str);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}