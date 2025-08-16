using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingClickgoManager : MonoBehaviour
{
    public GameObject WaitingClickgo;
    public GameObject UiManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void onClicknext()

    {
         WaitingClickgo.SetActive(false);

        //waitingclickgo ³öÏÖ
        UiManager.GetComponentInChildren<Gaming300sManager>().Gaming300s.SetActive(true);
        UiManager.GetComponentInChildren<Gaming300sManager>().StartInvoke();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
