using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StartPage1ClickManager : MonoBehaviour
{
    public GameObject StartPage1Click;
    public GameObject UiManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void onClickStart()
    {
        StartPage1Click.SetActive(false);

        //StartPage2Auto 出现
        UiManager.GetComponentInChildren<StartPage2AutoManager>().StartPage2Auto.SetActive(true);
        UiManager.GetComponentInChildren<StartPage2AutoManager>().StartInvoke();
    }
    public void onClickHelp()
    {
        StartPage1Click.SetActive(false);

        //HelpPage 出现
        UiManager.GetComponentInChildren<HelpPageManager>().HelpPage.SetActive(true);
        
    }
}
