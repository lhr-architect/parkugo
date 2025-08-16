using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingRedUIManager : MonoBehaviour
{
    public GameObject panel;
    public GameObject UiManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClickGo()

    {
        panel.SetActive(false);

        //StartPage2Auto ³öÏÖ
        UiManager.GetComponentInChildren<SelectingHeroUIManager>().panel.SetActive(true);
    }
}
