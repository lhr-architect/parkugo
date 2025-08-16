using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPage5Manager : MonoBehaviour
{
    public GameObject AnimationPage5;
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
        AnimationPage5.SetActive(false);

        if (UiManager != null)
        {
            // ��� UiManager ��ר�ŵ���ת�߼�����������������䷽��
            UiManager.GetComponentInChildren<AnimationPage6Manager>().AnimationPage6.SetActive(true);
            UiManager.GetComponentInChildren<AnimationPage6Manager>().StartInvoke();
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
