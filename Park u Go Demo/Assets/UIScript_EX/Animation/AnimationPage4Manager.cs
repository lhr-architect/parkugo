using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPage4Manager : MonoBehaviour
{
    public GameObject AnimationPage4;
    public GameObject UiManager;
    // Start is called before the first frame update
    void Start()
    {
        // �ӳ�1.5����÷�����ת����һҳ��
       // Invoke("GoToNextPage", 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void GoToNextPage()
    {
        // ����ҳ���л��߼��Ǽ���/����GameObject
        AnimationPage4.SetActive(false);

        if (UiManager != null)
        {
            // ��� UiManager ��ר�ŵ���ת�߼�����������������䷽��
            UiManager.GetComponentInChildren<AnimationPage5Manager>().AnimationPage5.SetActive(true);
            UiManager.GetComponentInChildren<AnimationPage5Manager>().StartInvoke();
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
