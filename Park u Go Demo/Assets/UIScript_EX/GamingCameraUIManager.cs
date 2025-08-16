using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamingCameraUIManager : MonoBehaviour
{
    public GameObject panel;
    public GameObject UiManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void onClickback()

    {
        panel.SetActive(false);

        //waitingclickgo ³öÏÖ
        UiManager.GetComponentInChildren<BigmapUIManagers>().BigmapPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
