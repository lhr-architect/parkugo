using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamingSkillManager : MonoBehaviour
{
    public GameObject GamingSkill;
    public GameObject UiManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void onClickback()

    {
        GamingSkill.SetActive(false);

        //waitingclickgo ³öÏÖ
        UiManager.GetComponentInChildren<GamingMainPageManager>().GamingMainPage.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
