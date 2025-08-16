
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


//Hunter、Sniper、Wizard、scout、priest

public class SelectingHeroUIManager : MonoBehaviour
{
    public GameObject panel;
    public GameObject UiManager;

    public RawImage centerImg;
    public RawImage background;
    public RawImage backBtnImg, nextBtnImg;

    public RawImage[] iconImgs;
    public PlayerController.Hero SelectedType { get; private set; }


    private void Awake()
    {
        SelectedType = PlayerController.Hero.unclear;
    }
    private void Start()
    {
        SelectedType = PlayerController.Hero.unclear;
    }

    public void onClickHunter()
    {
        panel.SetActive(false);
        SelectedType = PlayerController.Hero.hunter;

        UiManager.GetComponentInChildren<IntroHeroUIManager>().panel.SetActive(true);
    }
    public void onClickSniper()
    {
        panel.SetActive(false);
        SelectedType = PlayerController.Hero.sniper;
        UiManager.GetComponentInChildren<IntroHeroUIManager>().panel.SetActive(true);
    }
    public void onClickScout()
    {
        panel.SetActive(false);
        SelectedType = PlayerController.Hero.scout;
        UiManager.GetComponentInChildren<IntroHeroUIManager>().panel.SetActive(true);
    }
    public void onClickPriest()
    {
        panel.SetActive(false);
        SelectedType = PlayerController.Hero.priest;
        UiManager.GetComponentInChildren<IntroHeroUIManager>().panel.SetActive(true);
    }
    public void onClickWizard()

    {
        panel.SetActive(false);
        SelectedType = PlayerController.Hero.wizard;
        UiManager.GetComponentInChildren<IntroHeroUIManager>().panel.SetActive(true);

    }
    public void onClickback()
    {
        panel.SetActive(false);
        SelectedType = PlayerController.Hero.unclear;

        //返回选颜色界面，移除该用户数据
        GameManager.instance.removeUserRpc(
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().playerinfo.userName,
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().playerinfo.userParty
        );

        //StartPage2Auto 出现
        UiManager.GetComponentInChildren<SettingPartyUIManager>().panel.SetActive(true);
    }

    public void onClicknext()
    {
        panel.SetActive(false);
        
        GameManager.instance.settleHeroRpc(
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().playerinfo.userName,
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().playerinfo.userParty,
            SelectedType
        );

        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().playerinfo.heroType = SelectedType;

        //waitingclickgo 出现
        UiManager.GetComponentInChildren<LobbyUIManager>().panel.SetActive(true);
    }

    public void clearSelectedHero()
    {
        SelectedType = PlayerController.Hero.unclear;
    }

    public void SetSelectedType(PlayerController.Hero heroType)
    {
        SelectedType = heroType;
        GameManager.WorldTopic worldTopic = GameManager.instance.getPartyWorldTopic(
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().playerinfo.userParty
        );

        centerImg.texture = GameManager.FindTextureWithTopic(worldTopic, $"icon_{SelectedType}");
    }

    public void refreshUi()
    {

        GameManager.WorldTopic worldTopic = GameManager.instance.getPartyWorldTopic(
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().playerinfo.userParty
        );
        backBtnImg.texture = GameManager.FindTextureWithTopic(worldTopic, "icon_arrowBack");

        nextBtnImg.texture = GameManager.FindTextureWithTopic(worldTopic, "icon_arrowNext");

        background.texture = GameManager.FindTextureWithTopic(worldTopic, "background_3");

        //To do 换五个按钮的素材
        for (int i = 0;i < 5;++i)
        {
            int index = i;
            iconImgs[index].texture = GameManager.FindTextureWithTopic(worldTopic, $"icon_{(PlayerController.Hero)index}");
        }

        if(SelectedType == PlayerController.Hero.unclear)
        {
            centerImg.texture = null;
            centerImg.color = new Color(1f,1f,1f,0f);
        }
        else
        {
            centerImg.texture = GameManager.FindTextureWithTopic(worldTopic, $"icon_{SelectedType}");
            centerImg.color = new Color(1f, 1f, 1f, 1f);
        }
    }
}