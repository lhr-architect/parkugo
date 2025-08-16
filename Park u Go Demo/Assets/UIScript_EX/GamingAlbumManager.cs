using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamingAlbumManager : MonoBehaviour
{
    public GameObject GamingAlbum;
    public GameObject UiManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void onClickback()

    {
        GamingAlbum.SetActive(false);

        //waitingclickgo ³öÏÖ
        UiManager.GetComponentInChildren<GamingMainPageManager>().GamingMainPage.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
