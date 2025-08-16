using LitJson;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static AlbumUIManager;

public class LmSelectUIManager : MonoBehaviour, IManager
{
    public GameObject panel;
    public GameObject UiManager;

    public RawImage centerImg;
    public Button backBtn;
    public TMP_InputField input;

    //cards
    public GameObject card;
    public RawImage[] cardImgs;
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
        // 返回主页面
        UiManager.GetComponentInChildren<LandmarkUIManager>().panel.SetActive(true);
    }

    public void onClickStealFromAlbum()
    {
        if (AlbumIndex >= 0 && AlbumIndex < 5 )
        {
            //有照片选中了
            if (cardImgs[AlbumIndex].texture != null)
            {
                centerImg.texture = cardImgs[AlbumIndex].texture;
            }
        }
        card.SetActive(false);
    }

    public void GetEffectedFromAlbum()
    {
        for (int i = 0; i < 5; i++)
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
                Debug.Log($"Click{i}");
                AlbumIndex = i;
                break;
            }
        }
    }

    public void onClickCenterImg()
    {
        //重新拉取AlbumManager
        AlbumIndex = -1;
        card.SetActive(true);
    }


    public void onClickSend()
    {
        if (AlbumIndex >= 0 && AlbumIndex < cardImgs.Length)
        {
            if (UiManager.GetComponentInChildren<AlbumUIManager>().albums[AlbumIndex].state == AlbumState.NULL) return;

            UiManager.GetComponentInChildren<AlbumUIManager>().lockImgAt(AlbumIndex);
            centerImg.texture = null;

            byte[] oldPhotoData = UiManager.GetComponentInChildren<AlbumUIManager>().imgsbuffer[AlbumIndex].encode2Bytes();
            PackedImg u = PackedImg.decode2Class(oldPhotoData);

            photoClient.Instance.SendImg2Server(u);

            //删旧，传新的
            photoClient.Instance.AddImgRpc(
                u.userName, u.pid,
                photoClient.Instance.LastHitLandmarkName,
                UiManager.GetComponentInChildren<LandmarkUIManager>().PhotoIndex,
                NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().playerinfo.userParty
            );
        }

    }


}
