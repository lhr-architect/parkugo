using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPage2ClickManager : MonoBehaviour
{
    public GameObject CameraPage2Click;
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
        CameraPage2Click.SetActive(false);

        //StartPage2Auto ³öÏÖ
        UiManager.GetComponentInChildren<AnimationPage1Manager>().AnimationPage1.SetActive(true);
        UiManager.GetComponentInChildren<AnimationPage1Manager>().StartInvoke();
    }
}
