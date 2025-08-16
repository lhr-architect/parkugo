using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CameraPage2UIManager : MonoBehaviour
{
    public GameObject panel;
    public GameObject UiManager;
    private TMP_InputField inputField;


    // Start is called before the first frame update
    void Start()
    {
       inputField = panel.GetComponentInChildren<TMP_InputField>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void onClickStart()
    {
        panel.SetActive(false);
        
        //��������û����ǿ�
        if (inputField.text.Length > 0)
        {
            //���û���ȥע��Tcp
            photoClient.Instance?.StartTcp(inputField.text);
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>()?.fillPlayerUserName(inputField.text);

            //Temp Logic
            UiManager.GetComponentInChildren<SettingPartyUIManager>().panel.SetActive(true);
        }
        else
        {
           //GUI ��������û����ǿ�

        }
    }

    public void changePage()
    {
        panel.SetActive(false);

        UiManager.GetComponentInChildren<SettingPartyUIManager>().panel.SetActive(true);
    }

}
