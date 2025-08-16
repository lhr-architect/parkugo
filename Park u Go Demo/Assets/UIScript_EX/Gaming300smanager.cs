using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gaming300sManager : MonoBehaviour
{
    public GameObject Gaming300s;
    public GameObject UiManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void GoToNextPage()
    {
        // 假设页面切换逻辑是激活/禁用GameObject
        Gaming300s.SetActive(false);

        if (UiManager != null)
        {
            // 如果 UiManager 有专门的跳转逻辑，可以在这里调用其方法
            UiManager.GetComponentInChildren<GamingMainPageManager>().GamingMainPage.SetActive(true);
           
            // 假设 UiManager 有 ShowNextPage 方法
        }
        else
        {
            Debug.LogWarning("UiManager 未设置，无法切换页面");
        }
    }
    public void StartInvoke()
    {
        Invoke("GoToNextPage", 1.0f);
    }
}
