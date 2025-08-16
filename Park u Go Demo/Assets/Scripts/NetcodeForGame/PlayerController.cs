using System;
using System.Collections;
using System.Net.NetworkInformation;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public NetworkVariable<int> health = new NetworkVariable<int>(2, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<Hero> hero =  new NetworkVariable<Hero>(Hero.unclear, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<Party> party = new NetworkVariable<Party>(Party.RED, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    //Local Data
    public PlayerInfo playerinfo;
    public float speed;

    public float uploadCD = 30f;
    public float lastUploadTime;

    public enum Party
    {
        RED,
        BLUE
    }
    public enum Hero
    {
        hunter,
        sniper,
        wizard,
        scout,
        priest,
        unclear
    }

    public struct PlayerInfo : INetworkSerializable
    {
        //�ͻ���ui�������
        public string userName;
        public ulong clientId;
        public Party userParty;
        public Hero heroType;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref userName);
            serializer.SerializeValue(ref clientId);
            serializer.SerializeValue(ref userParty);
            serializer.SerializeValue(ref heroType);
        }
    }

    public void fillPlayerUserName(string userName)
    {
        playerinfo.userName = userName;
    }
    public void fillPlayerParty(Party uiParty)
    {
        playerinfo.userParty = uiParty;
    }

    public void MovePlayer(Vector3 direction)
    {
        if (!IsOwner || !Application.isFocused) { return; }

        transform.Translate(direction * speed);
    }

    override public void OnNetworkSpawn()
    {
  

        transform.position = new Vector3(Camera.main.transform.position.x,Camera.main.transform.position.y, 0f);
        playerinfo = new PlayerInfo();

        hero.OnValueChanged += OnHeroChanged;
        health.OnValueChanged += OnHealthChanged;
        party.OnValueChanged += OnPartyChanged;


        StartCoroutine(DelayedAction());
        lastUploadTime = Time.time - uploadCD;
    }



    private void OnHealthChanged(int old, int now)
    {
        if (now <= 0) {
            //����ͨ�� ����һ����Ϣ�أ���ұ��
        }

        //����owner����
        if (!IsOwner) return;

        photoClient.Instance.UIManagers.GetComponentInChildren<BigmapUIManagers>().setHealth(now);


    }

    private void OnHeroChanged(Hero old, Hero now)
    {
        changePlayerIcon(now);
    }

    private void OnPartyChanged(Party old, Party now)
    {
        changePlayerParty(now);
    }


    IEnumerator DelayedAction()
    {
        yield return new WaitUntil(() => (NetworkManager.Singleton != null && NetworkManager.Singleton.IsConnectedClient));

        changePlayerIcon(hero.Value);
        changePlayerParty(party.Value);

    }


    public void changePlayerIcon(Hero h)
    {
        Texture2D tex = GameManager.instance.FindHeroIcon_Map(h, party.Value);
        if (tex != null)
        {
            Sprite newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 10f);
            gameObject.GetComponent<SpriteRenderer>().sprite = newSprite;
            gameObject.GetComponent<SpriteRenderer>().size = new Vector2(80f, 80f);
        }
    }

    public void changePlayerParty(Party p)
    {
        Texture2D tex = GameManager.instance.FindHeroIcon_Map(hero.Value, p);
        if (tex != null)
        {
            Sprite newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 10f);
            gameObject.GetComponent<SpriteRenderer>().sprite = newSprite;

            gameObject.GetComponent<SpriteRenderer>().size = new Vector2(80f, 80f);
        }
    }


    public Vector2 GetPosition()
    {
        //��ʱ�߼����ֻ�����ʹ��
        return new Vector2(transform.position.x, transform.position.y);
    }


    // Update is called once per frame
    void Update()
    {

        if (!IsOwner || !Application.isFocused) { return; }

        //Windows Editor�¿����߼�
        if (Application.platform == RuntimePlatform.WindowsEditor) 
        { 
            float moveUD = 0f;
            float moveLR = 0f;

            if (Input.GetKey(KeyCode.W)) moveUD = 1f;
            else if (Input.GetKey(KeyCode.S)) moveUD = -1f;
            if (Input.GetKey(KeyCode.D)) moveLR = 1f;
            else if (Input.GetKey(KeyCode.A)) moveLR = -1f;

            // �����������λ��
            Vector3 movement = new Vector3(moveLR, moveUD, 0.0f);
            transform.Translate(movement * speed * Time.deltaTime);
        }

    }


    public string GetLocalIPv4()
    {
        string LocalAddress = ".";
        try
        {
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                NetworkInterfaceType _type1 = NetworkInterfaceType.Wireless80211;
                NetworkInterfaceType _type2 = NetworkInterfaceType.Ethernet;
                if ((item.NetworkInterfaceType == _type1 || item.NetworkInterfaceType == _type2) && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            LocalAddress = ip.Address.ToString();
                            break;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
        return LocalAddress;
    }

}
