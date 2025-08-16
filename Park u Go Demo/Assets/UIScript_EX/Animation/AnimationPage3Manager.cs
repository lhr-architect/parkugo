using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPage3Manager : MonoBehaviour
{
    public GameObject AnimationPage3;
    public GameObject UiManager;
    // Start is called before the first frame update
    void Start()
    {
        // 延迟1.5秒调用方法跳转到下一页面
      //  Invoke("GoToNextPage", 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void GoToNextPage()
    {
        // 假设页面切换逻辑是激活/禁用GameObject
        AnimationPage3.SetActive(false);

        if (UiManager != null)
        {
            // 如果 UiManager 有专门的跳转逻辑，可以在这里调用其方法
            UiManager.GetComponentInChildren<AnimationPage4Manager>().AnimationPage4.SetActive(true);
            UiManager.GetComponentInChildren<AnimationPage4Manager>().StartInvoke();
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
