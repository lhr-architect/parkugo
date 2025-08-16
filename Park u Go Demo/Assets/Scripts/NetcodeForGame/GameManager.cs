using Mono.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Netcode;

using UnityEngine;
using static PlayerController;

public class GameManager : NetworkBehaviour
{


    public enum WorldTopic
    {
        digital,
        fantasy,
        wuxia,
        pixel,
        modern
    }

    // run on everywhere
    public static GameManager instance;
    public NetworkVariable<int> redPartyCnt = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> bluePartyCnt = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<int> redSureCnt = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> blueSureCnt = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<int> redSummary = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> blueSummary = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<WorldTopic> redTopic = new NetworkVariable<WorldTopic>(WorldTopic.wuxia, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<WorldTopic> blueTopic = new NetworkVariable<WorldTopic>(WorldTopic.modern, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    //only on server
    public Dictionary<string, PlayerInfo> redPlayers;
    public Dictionary<string, PlayerInfo> bluePlayers;


    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            redPlayers = new Dictionary<string, PlayerInfo>();
            bluePlayers = new Dictionary<string, PlayerInfo>();

            //redTopic.Value = WorldTopic.wuxia;
            //blueTopic.Value = WorldTopic.modern;

            redTopic.OnValueChanged += onTopicChanged;
            blueTopic.OnValueChanged += onTopicChanged;
        }

       
        instance = this;

    }

    public WorldTopic getPartyWorldTopic(Party p)
    {
        if(p == Party.RED) {  return redTopic.Value; }
        else {  return blueTopic.Value; }
    }

    private void onTopicChanged(WorldTopic old, WorldTopic now)
    {
        randWorldTopicCallBackRpc();
    }

    [Rpc(SendTo.Server)]
    public void randWorldTopicRpc(Party p)
    {
        if (IsServer)
        {
            if (p == Party.RED)
            {
                redTopic.Value = GetRandomTopicExcluding(blueTopic.Value, redTopic.Value);
            }
            else
            {
                blueTopic.Value = GetRandomTopicExcluding(redTopic.Value, blueTopic.Value);
            }
        }

        randWorldTopicCallBackRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void randWorldTopicCallBackRpc()
    {
        if(photoClient.Instance.UIManagers.GetComponentInChildren<SettingPartyUIManager>().panel.activeSelf)
        {
            //photoClient.Instance.UIManagers.GetComponentInChildren<SettingPartyUIManager>().refreshUi();
        }
    }


    private WorldTopic GetRandomTopicExcluding(WorldTopic excludeTopic1, WorldTopic excludeTopic2)
    {
        WorldTopic[] topics = (WorldTopic[])Enum.GetValues(typeof(WorldTopic));
        WorldTopic randomTopic;
        do
        {
          randomTopic = topics[UnityEngine.Random.Range(0, topics.Length)];
        } while (randomTopic == excludeTopic1 || randomTopic == excludeTopic1);
        return randomTopic;
    }


    [Rpc(SendTo.Server)]
    public void addSummaryRpc(PlayerController.Party party)
    {
        if(IsServer)
        {
            if (party == PlayerController.Party.RED)
            {
                redSummary.Value += 2;
            }
            else
            {
                blueSummary.Value += 2;
            }
        }
     
    }

    [Rpc(SendTo.Server)]
    public void settleHeroRpc(string userName, PlayerController.Party party, PlayerController.Hero hero)
    {
        //这里进入游戏了
        if (!IsServer) return;
        if (party == Party.RED)
        {
            if (redPlayers.ContainsKey(userName))
            {

                PlayerController.PlayerInfo u = redPlayers[userName];
                redPlayers[userName] = new PlayerController.PlayerInfo {
                    userName = u.userName,
                    clientId = u.clientId,
                    userParty = party,
                    heroType = hero
                };

                redSureCnt.Value++;

                setPlayerIcon(redPlayers[userName].clientId, party, hero);
            }
            else
            {
                Debug.LogWarning("no user bro");
            }
        }
        else
        {
            if (bluePlayers.ContainsKey(userName))
            {
                PlayerController.PlayerInfo u = bluePlayers[userName];
                bluePlayers[userName] = new PlayerController.PlayerInfo
                {
                    userName = u.userName,
                    clientId = u.clientId,
                    userParty = party,
                    heroType = hero
                };
                blueSureCnt.Value++;

                setPlayerIcon(bluePlayers[userName].clientId, party, hero);
            }
            else
            {
                Debug.LogWarning("no user bro");
            }
        }
    }


    public void setPlayerIcon(ulong clientId, Party p, Hero hero)
    {
        //run on server
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
        {
            client.PlayerObject.GetComponent<PlayerController>().party.Value = p;

            client.PlayerObject.GetComponent<PlayerController>().hero.Value = hero;
            //on Value changed

            //Debug.Log($"Value Changed{p} {hero}");
        }
    }

    [Rpc(SendTo.Server)]
    public void unSettleHeroRpc(string userName, Party party)
    {
        if (!IsServer) return;
        if (party == Party.RED)
        {
            if (redPlayers.ContainsKey(userName))
            {
                PlayerInfo u = redPlayers[userName];
                redPlayers[userName] = new PlayerInfo
                {
                    userName = u.userName,
                    clientId = u.clientId,
                    userParty = party,
                    heroType = Hero.unclear
                };
                redSureCnt.Value--;
            }
            else
            {
                Debug.LogWarning("no user bro");
            }
        }
        else
        {
            if (bluePlayers.ContainsKey(userName))
            {
                PlayerInfo u = bluePlayers[userName];
                bluePlayers[userName] = new PlayerInfo
                {
                    userName = u.userName,
                    clientId = u.clientId,
                    userParty = party,
                    heroType = Hero.unclear
                };
                blueSureCnt.Value--;
            }
            else
            {
                Debug.LogWarning("no user bro");
            }
        }

    }

    [Rpc(SendTo.Server)]
    public void addUserRpc(string userName, ulong uid, Party party)
    {
        if (!IsServer) return;

        if (party == Party.RED)
        {
            if (!redPlayers.ContainsKey(userName))
            {
                PlayerInfo info = new PlayerInfo
                {
                    userName = userName,
                    clientId = uid,
                    userParty = party,
                    heroType = Hero.unclear
                };
                

                redPartyCnt.Value++;
                redPlayers.Add(userName, info);

            }
            else
            {
                Debug.LogWarning("Already added user");
            }
        }
        else
        {
            if (!bluePlayers.ContainsKey(userName))
            {
                PlayerInfo info = new PlayerInfo
                {
                    userName = userName,
                    clientId = uid,
                    userParty = party,
                    heroType = Hero.unclear
                };
                bluePartyCnt.Value++;
                bluePlayers.Add(userName, info);
            }
            else
            {
                Debug.LogWarning("Already added user");
            }
        }

    }

    [Rpc(SendTo.Server)]
    public void removeUserRpc(string userName, Party party)
    {
        if (!IsServer) return;

        if (party == Party.RED)
        {
            if (redPlayers.ContainsKey(userName))
            {
                redPlayers.Remove(userName);
                redPartyCnt.Value--;
            }

        }
        else
        {
            if (bluePlayers.ContainsKey(userName))
            {
                bluePlayers.Remove(userName);
                bluePartyCnt.Value--;
            }

        }
    }

    [Rpc(SendTo.Server)]
    public void requestAttackTargetsRpc(string requestUser, Party party)
    {
        if(!IsServer) return;
        if (party == Party.RED)
        {
            string str = LitJson.JsonMapper.ToJson(bluePlayers);
            requestAttackTargetsCallBackRpc(requestUser, str);
            //拿所有蓝的
        }
        else
        {
            string str = LitJson.JsonMapper.ToJson(redPlayers);
            requestAttackTargetsCallBackRpc(requestUser, str);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void requestAttackTargetsCallBackRpc(string requestUser, string mapstr)
    {
        if(!IsClient) return;
        //非请别理
        if (NetworkManager.LocalClient.PlayerObject.GetComponent<PlayerController>().playerinfo.userName != requestUser) return;

        //Debug.LogWarning(mapstr);
        photoClient.Instance?.UIManagers.GetComponentInChildren<GamingAttackUIManager>().fillAttackTarget(mapstr);

    }


    [Rpc(SendTo.Server)]
    public void requestLobbyDataRpc()
    {
        if(IsServer)
        {
            string redstr = LitJson.JsonMapper.ToJson(redPlayers);
            string bluestr = LitJson.JsonMapper.ToJson(bluePlayers);

            sendLobbyDataRpc(redstr, bluestr);
        }
    }

    //广播所有人刷新Lobby
    [Rpc(SendTo.ClientsAndHost)]
    public void sendLobbyDataRpc(string redstr, string bluestr)
    {
        if(IsClient)
        {
            photoClient.Instance.UIManagers.GetComponentInChildren<LobbyUIManager>().freshLobby(redstr, bluestr);
        }
    }

    [Rpc(SendTo.Server)]
    public void requesSidebarDataRpc()
    {
        if (IsServer)
        {
            string redstr = LitJson.JsonMapper.ToJson(redPlayers);
            string bluestr = LitJson.JsonMapper.ToJson(bluePlayers);

            sendLobbyDataRpc(redstr, bluestr);
        }
    }

    //广播所有人刷新Lobby
    [Rpc(SendTo.ClientsAndHost)]
    public void sendSidebarDataRpc(string redstr, string bluestr)
    {
        if (IsClient)
        {
            photoClient.Instance.UIManagers.GetComponentInChildren<LobbyUIManager>().freshLobby(redstr, bluestr);
        }
    }



    private List<string> eventList = new List<string>();

    [Rpc(SendTo.Server)]
    public void AddHurtEventRpc(Party killerParty, string killerName, Party deadParty, string deadName)
    {
        string evtStr = $"{killerParty} {killerName} 击中 {deadParty} {deadName}";
        eventList.Add(evtStr);

        //To do 扣血Rpc
        ulong cid;
        if (deadParty == Party.RED)
        {
            cid = redPlayers[deadName].clientId;
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(cid, out var client))
            {
                //打到活人身上了
                if (client.PlayerObject.GetComponent<PlayerController>().health.Value > 0)
                {
                    blueSummary.Value += 1;
                    client.PlayerObject.GetComponent<PlayerController>().health.Value--;
                }

            }
        }
        else
        {
            cid = bluePlayers[deadName].clientId;
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(cid, out var client))
            {
                if (client.PlayerObject.GetComponent<PlayerController>().health.Value > 0)
                {
                    redSummary.Value += 1;
                    client.PlayerObject.GetComponent<PlayerController>().health.Value--;
                }

            }
        }
        //To do Evt显示回调
        EventShowCallBackRpc(evtStr, DateTime.Now.ToString());
    }

    [Rpc(SendTo.Server)]
    public void AddDeathEventRpc(Party deadParty, string deadName)
    {
        string evtStr = $"{deadParty} 阵亡 {deadName}";
        eventList.Add(evtStr);

        if (deadParty == Party.RED)
        {
            blueSummary.Value += 2;
        }
        else
        {
            redSummary.Value += 2;
        }

        //To do Evt显示回调
        EventShowCallBackRpc(evtStr, DateTime.Now.ToString());
    }


    [Rpc(SendTo.ClientsAndHost)]
    public void EventShowCallBackRpc(string EvtStr,string time)
    {
        if(!IsClient) { return; }
        photoClient.Instance.UIManagers.GetComponentInChildren<BigmapUIManagers>().addBoardcastMsg(EvtStr, time);
    }


    static public Party InverseParty(Party p)
    {
        if (p == Party.RED) return Party.BLUE;
        else return Party.RED;
    }

     public Texture2D FindHeroIcon_Ui(Hero hero, Party party)
    {
        WorldTopic worldTopic = getPartyWorldTopic(party);
        string type = $"icon_{hero}";

        return FindTextureWithTopic(worldTopic, type);
    }

    public Texture2D FindHeroIcon_Map(Hero hero, Party party)
    {
        WorldTopic worldTopic = getPartyWorldTopic(party);
        string type = $"icon_{hero}";

        return FindTextureWithTopic(worldTopic, type);
    }


    static public Texture2D FindTextureWithTopic(WorldTopic topic, string type) 
    {
        string url = $"{topic}/{topic}_{type}";
        return Resources.Load<Texture2D>(url);
    }

}
