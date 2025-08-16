
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SettingPartyUIManager : MonoBehaviour
{
    public GameObject panel;
    public GameObject UiManager;

    public TextMeshProUGUI redText, blueText;
    public RawImage redBackground,blueBackground;

    // Start is called before the first frame update
    void Start()
    {

    }
    private void Update()
    {
        if (panel.activeSelf && GameManager.instance != null)
        {
            redText.text = $"{GameManager.instance.redPartyCnt.Value}/5";
            blueText.text = $"{GameManager.instance.bluePartyCnt.Value}/5";

            refreshUi();
        }

    }



    // Update is called once per frame
    public void onClickRed()
    {
        if(GameManager.instance.redPartyCnt.Value >= 5) { return; }

        panel.SetActive(false);
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().fillPlayerParty(PlayerController.Party.RED);

        GameManager.instance.addUserRpc(
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().playerinfo.userName,
            NetworkManager.Singleton.LocalClientId,
            PlayerController.Party.RED
        );

        UiManager.GetComponentInChildren<GoUIManager>().panel.SetActive(true);


    }


    public void onClickBlue()
    {
        if (GameManager.instance.bluePartyCnt.Value >= 5) { return; }

        panel.SetActive(false);
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().fillPlayerParty(PlayerController.Party.BLUE);

        GameManager.instance.addUserRpc(
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().playerinfo.userName,
            NetworkManager.Singleton.LocalClientId,
            PlayerController.Party.BLUE
        );

        UiManager.GetComponentInChildren<GoUIManager>().panel.SetActive(true);
    }


    public void onClickRollRed()
    {
        if(GameManager.instance.redSureCnt.Value > 0) { return; }
        GameManager.instance.randWorldTopicRpc(PlayerController.Party.RED);
    }

    public void onClickRollBlue()
    {
        if(GameManager.instance.blueSureCnt.Value > 0) { return; }
        GameManager.instance.randWorldTopicRpc(PlayerController.Party.BLUE);
    }

    public void refreshUi()
    {
        redBackground.texture = GameManager.FindTextureWithTopic(GameManager.instance.redTopic.Value,"background_1");
        blueBackground.texture = GameManager.FindTextureWithTopic(GameManager.instance.blueTopic.Value, "background_1");
    }

    public void onClickBack()

    {
        panel.SetActive(false);
        //StartPage2Auto ³öÏÖ
        UiManager.GetComponentInChildren<CameraPage2UIManager>().panel.SetActive(true);
    }
}
