using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpPageManager : MonoBehaviour
{
    public GameObject HelpPage;
    public GameObject UiManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void onClickBack()
    {
        HelpPage.SetActive(false);

        //HelpPage ³öÏÖ
        UiManager.GetComponentInChildren<StartPage1ClickManager>().StartPage1Click.SetActive(true);
    }
}
