
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GoUIManager : MonoBehaviour
{
    public GameObject panel;
    public GameObject UiManager;
    public RawImage background;
    public void onClickGo()
    {
        panel.SetActive(false);
        UiManager.GetComponentInChildren<SelectingHeroUIManager>().panel.SetActive(true);
    }

    public void refreshUi()
    {
        GameManager.WorldTopic worldTopic = GameManager.instance.getPartyWorldTopic(
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().playerinfo.userParty
        );

        background.texture = GameManager.FindTextureWithTopic(worldTopic, "background_2");
    }
}
