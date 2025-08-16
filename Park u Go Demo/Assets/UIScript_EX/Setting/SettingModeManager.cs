using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingModeManager : MonoBehaviour
{
    public GameObject SettingMode;
    public GameObject UiManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void onClickStart()

    {
        SettingMode.SetActive(false);

        //StartPage2Auto ³öÏÖ
        UiManager.GetComponentInChildren<SettingPartyUIManager>().panel.SetActive(true);
    }
    public void StartInvoke()
    {
        Invoke("GoToNextPage", 1.0f);
    }
}
