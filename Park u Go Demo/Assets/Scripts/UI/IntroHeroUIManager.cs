
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class IntroHeroUIManager : MonoBehaviour
{
    public GameObject panel;
    public GameObject UiManager;
    public RawImage background,backBtn;
    // Start is called before the first frame update

    public void refreshUi()
    {
        GameManager.WorldTopic worldTopic = GameManager.instance.getPartyWorldTopic(
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().playerinfo.userParty
        );

        background.texture = GameManager.FindTextureWithTopic(
            worldTopic, 
            $"background_{photoClient.Instance.UIManagers.GetComponentInChildren<SelectingHeroUIManager>().SelectedType}"
        );

        backBtn.texture = GameManager.FindTextureWithTopic(worldTopic,"icon_arrowBack");

    }

    public void onClickBack()
    {
        panel.SetActive(false);
        UiManager.GetComponentInChildren<SelectingHeroUIManager>().clearSelectedHero();
        UiManager.GetComponentInChildren<SelectingHeroUIManager>().panel.SetActive(true);
    }


    public void onClickBackGround()
    {
        panel.SetActive(false);
        UiManager.GetComponentInChildren<SelectingHeroUIManager>().panel.SetActive(true);
    }
}
