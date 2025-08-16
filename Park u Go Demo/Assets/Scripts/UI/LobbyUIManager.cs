using LitJson;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class LobbyUIManager : MonoBehaviour
{
    public GameObject panel;
    public GameObject UiManagers;

    public TextMeshProUGUI red, blue;
    public RawImage[] redSlots, blueSlots;
    public RawImage backBtnImg, background;

    // Update is called once per frame
    void Update()
    {
        if (panel.activeSelf && GameManager.instance != null)
        {
            red.text = $"{GameManager.instance.redSureCnt.Value}/{GameManager.instance.redPartyCnt.Value}";
            blue.text = $"{GameManager.instance.blueSureCnt.Value}/{GameManager.instance.bluePartyCnt.Value}";

            GameManager.instance.requestLobbyDataRpc();
        }        
    }

    public void onClickGo()
    {
        
        panel.SetActive(false);

        UiManagers.GetComponentInChildren<BigmapUIManagers>().BigmapPanel.SetActive(true);
    }

    public void onClickBack()
    {
        panel.SetActive(false);
        GameManager.instance.unSettleHeroRpc(
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().playerinfo.userName,
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().playerinfo.userParty
        );
        GameManager.instance.removeUserRpc(
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().playerinfo.userName,
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().playerinfo.userParty
        );
        UiManagers.GetComponentInChildren<SettingPartyUIManager>().panel.SetActive(true);
    }

    public void freshLobby(string redstr, string bluestr)
    {
        if (!panel.activeSelf) return;

        Dictionary<string, PlayerController.PlayerInfo> redmap =
            JsonMapper.ToObject < Dictionary<string, PlayerController.PlayerInfo>>(redstr);

        Dictionary<string, PlayerController.PlayerInfo> bluemap =
            JsonMapper.ToObject<Dictionary<string, PlayerController.PlayerInfo>>(bluestr);

        fillSlots(redSlots, redmap);
        fillSlots(blueSlots, bluemap);

    }

    private void fillSlots(RawImage[] imgSlots, Dictionary<string, PlayerController.PlayerInfo> map)
    {
        int index = 0;
        foreach(var info in map)
        {
            imgSlots[index].texture = GameManager.instance.FindHeroIcon_Ui(
                info.Value.heroType,
                info.Value.userParty
            );
            imgSlots[index].color = new Color(1, 1, 1, 1);
            index++;
        }
        while(index < 5)
        {
            imgSlots[index].texture = null;
            imgSlots[index].color = new Color(1, 1, 1, 0);
            index++;
        }
    }

    public void refreshUi()
    {
        GameManager.WorldTopic worldTopic = GameManager.instance.getPartyWorldTopic(
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().playerinfo.userParty
        );

        backBtnImg.texture = GameManager.FindTextureWithTopic(worldTopic, "icon_arrowBack");
    }

}
