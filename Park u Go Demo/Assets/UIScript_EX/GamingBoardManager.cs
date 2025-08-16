using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamingBoardManager : MonoBehaviour
{
    public GameObject GamingBoard;
    public GameObject UiManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void onClickback()

    {
        GamingBoard.SetActive(false);

        //waitingclickgo ³öÏÖ
        UiManager.GetComponentInChildren<GamingMainPageManager>().GamingMainPage.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
