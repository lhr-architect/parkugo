
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;


namespace HelloWorld
{
    public class ModeManager: MonoBehaviour
    {
        private NetworkManager m_NetworkManager;
        public GameObject c_camManager,s_camManager;
        public GameObject UIManagers;
        
        public string serverIP;
        public int TcpPort;

        void Awake()
        {
            m_NetworkManager = GetComponent<NetworkManager>();
        }

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 500));
            if (!m_NetworkManager.IsClient && !m_NetworkManager.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();
            }
            GUILayout.EndArea();
        }

        void StartButtons()
        {

            GUIStyle customButtonStyle = new GUIStyle(GUI.skin.button);
            customButtonStyle.fontSize = 36; // …Ë÷√Œ™24∫≈◊÷ÃÂ

            if (GUILayout.Button("Host", customButtonStyle, GUILayout.Width(200), GUILayout.Height(100)))
            {
                var utp = (UnityTransport)m_NetworkManager.NetworkConfig.NetworkTransport;
                utp.SetConnectionData(serverIP, 7777, "0.0.0.0");

                m_NetworkManager.StartHost();
                HostStart();
            }
            if (GUILayout.Button("Client", customButtonStyle, GUILayout.Width(200), GUILayout.Height(100)))
            {

                m_NetworkManager.StartClient();

                ClientStart();
            }
            if (GUILayout.Button("Server", customButtonStyle, GUILayout.Width(200), GUILayout.Height(100)))
            {
                var utp = (UnityTransport)m_NetworkManager.NetworkConfig.NetworkTransport;
                utp.SetConnectionData(serverIP, 7777, "0.0.0.0");

                m_NetworkManager.StartServer();
                ServerStart();
            }
        }
        
        void ServerStart()
        {
            s_camManager.GetComponent<photoServer>().enabled = true;
            
        }

        void ClientStart()
        {
            //c_camManager.GetComponent<CameraControllerFake>().enabled = true;
            c_camManager.GetComponent<photoClient>().enabled = true;

            UIManagers.GetComponentInChildren<CameraPage2UIManager>().panel.SetActive(true);

        }

        void HostStart()
        {   
            ServerStart();
            ClientStart();
        }
        
        void StatusLabels()
        {
            var mode = m_NetworkManager.IsHost ?
                "Host" : m_NetworkManager.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " +
                m_NetworkManager.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);
        }

    }
}