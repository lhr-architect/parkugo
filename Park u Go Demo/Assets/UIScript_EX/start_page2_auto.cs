using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPage2AutoManager : MonoBehaviour
{
    public GameObject StartPage2Auto;
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
        StartPage2Auto.SetActive(false);

        if (UiManager != null)
        {
            // ��� UiManager ��ר�ŵ���ת�߼�����������������䷽��
            UiManager.GetComponentInChildren<CameraPage2ClickManager>().CameraPage2Click.SetActive(true); // ���� UiManager �� ShowNextPage ����
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

