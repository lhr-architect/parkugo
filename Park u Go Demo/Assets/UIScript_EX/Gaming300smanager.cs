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
        // ����ҳ���л��߼��Ǽ���/����GameObject
        Gaming300s.SetActive(false);

        if (UiManager != null)
        {
            // ��� UiManager ��ר�ŵ���ת�߼�����������������䷽��
            UiManager.GetComponentInChildren<GamingMainPageManager>().GamingMainPage.SetActive(true);
           
            // ���� UiManager �� ShowNextPage ����
        }
        else
        {
            Debug.LogWarning("UiManager δ���ã��޷��л�ҳ��");
        }
    }
    public void StartInvoke()
    {
        Invoke("GoToNextPage", 1.0f);
    }
}
