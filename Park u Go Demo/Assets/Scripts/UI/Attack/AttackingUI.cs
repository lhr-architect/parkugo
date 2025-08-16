using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AttackingUI : MonoBehaviour
{
    public GamingAttackUIManager AtkManager;
    // Start is called before the first frame update
    public void OnEnable()
    {
        GameManager.instance?.requestAttackTargetsRpc(
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().playerinfo.userName,
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().playerinfo.userParty
        );

        AtkManager.loopList.refreshLoopList();
        AtkManager.centerImg.texture =  null;
    }
}
