using LitJson;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static AlbumUIManager;

public class GamingAttackUIManager : MonoBehaviour, IManager
{
    public GameObject panel;
    public GameObject card;

    public GameObject UiManager;

    public PlayerController.PlayerInfo[] attackInfos = new PlayerController.PlayerInfo[5];

    public RawImage[] cardImgs;
    public LoopList loopList;
    public RawImage centerImg;
   

    public int  AttackIndex;
    public int AlbumIndex {  get; set; }

    private void Start()
    {
        
        foreach (var img in cardImgs)
        {
            EventTrigger trigger = img.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => OnAlbumImgClicked(img));
            trigger.triggers.Add(entry);
        }
        card.GetComponent<CardUI>().manager = this;

    }

    public void onClickback()
    {
        panel.SetActive(false);
        UiManager.GetComponentInChildren<BigmapUIManagers>().BigmapPanel.SetActive(true);
    }

    public void onClickSteal()
    {
        if(AlbumIndex >= 0 && AlbumIndex < 5)
        {
            //����Ƭѡ����
            if (cardImgs[AlbumIndex].texture != null)
            {
                centerImg.texture = cardImgs[AlbumIndex].texture;
            }
        }
        card.SetActive(false);
    }

    public void onClickKill()
    {
        //To do �ҵ� uiIndex == 0�� DataIndex, ������Ѫ
        int dataIndex = loopList.findCenterHeroDataIndex();
        if (dataIndex == -1) { return; }

        if (loopList.data[dataIndex].name != "")
        {
            string deadName = loopList.data[dataIndex].name;
            PlayerController.Party deadParty = loopList.data[dataIndex].party;
            //ѡ���˻���
            //Debug.Log($"try to kill{loopList.data[dataIndex].name}");

            //To do �ع�����ʶ���߼�
            System.Random random = new System.Random();
            int randomNumber = random.Next(1, 10);

            if (centerImg.texture != null)
            {
                //ʹ����������Ƭ
                UiManager.GetComponentInChildren<AlbumUIManager>().BurnPossibleImgAt(AlbumIndex);
                centerImg.texture = null;
                NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().lastUploadTime = Time.time;

                if(randomNumber < 7)
                {
                    //��ɱ�ɹ� To do ͨ������Ѫ
                    //GameManager.
                    GameManager.instance.AddHurtEventRpc(
                        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().playerinfo.userParty,
                        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().playerinfo.userName,                     
                        deadParty, 
                        deadName
                    );

                }
            }
        }


    }

    public void fillAttackTarget(string targetStr)
    {
        Dictionary<string, PlayerController.PlayerInfo> target =
            JsonMapper.ToObject< Dictionary<string, PlayerController.PlayerInfo> >(targetStr);

        int index = 0;
        foreach(var item in target.Values)
        {
            loopList.data[index] = new Data
            {
                ArrayIndex = index,
                name = item.userName,
                hero = item.heroType,
                party = item.userParty
            };
            index++;
        }
        while(index < 5)
        {
            loopList.data[index] = new Data
            {
                ArrayIndex = index,
                name = "",
                hero = PlayerController.Hero.unclear,
                party = PlayerController.Party.RED
            };
            index++;
        }

        loopList.refreshLoopList();
    }

    public void GetEffectedFromAlbum()
    {
        for(int i = 0;i < 5;i++)
        {
            if (UiManager.GetComponentInChildren<AlbumUIManager>().albums[i].state == AlbumState.DEFAULT)
            {
                Texture2D tex = new Texture2D(
                    UiManager.GetComponentInChildren<AlbumUIManager>().imgsbuffer[i].width,
                    UiManager.GetComponentInChildren<AlbumUIManager>().imgsbuffer[i].height
                );
                tex.LoadImage(UiManager.GetComponentInChildren<AlbumUIManager>().imgsbuffer[i].ImageData);
                
                cardImgs[i].texture = tex;
            }
            else
            {
                cardImgs[i].texture = null;
            }
        }
    }

    private void OnAlbumImgClicked(RawImage clickedRawImage)
    {
        
        for (int i = 0; i < cardImgs.Length; i++)
        {
            if (cardImgs[i] == clickedRawImage)
            {
                AlbumIndex = i;
                break;
            }
        }
        Debug.Log($"click{AlbumIndex}");
    }

    public void onClickImgSlot()
    {
        card.SetActive(true);
    }


}
