using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AlbumUIManager : MonoBehaviour
{

    public enum AlbumState
    {
        NULL,
        DEFAULT,
        SELECTED,
        LOCKED
    }
    public struct Album
    {
        public RawImage rawImage;
        public AlbumState state;

        // ���캯��
        public Album(RawImage rawImage, AlbumState initialState)
        {
            this.rawImage = rawImage;
            state = initialState;
        }

        // ����RawImage״̬�ķ���
        public void UpdateState(AlbumState newState)
        {
            state = newState;
            switch (state)
            {
                case AlbumState.NULL:
                    rawImage.color = Color.black; // ����������ʾNULL״̬����ɫ
                    break;
                case AlbumState.DEFAULT:
                    rawImage.color = Color.white; // Ĭ����ɫ
                    break;
                case AlbumState.SELECTED:
                    rawImage.color = Color.green; // ѡ����ɫ
                    break;
                case AlbumState.LOCKED:
                    rawImage.color = Color.gray; // ������ɫ
                    break;
            }
        }
    }

    public GameObject UIManagers;
    public GameObject panel;
    public Button BackBtn;
    public RawImage[] rawImages;
    public Button[] deleteButtons;

    public Album[] albums; // ʹ��Album�ṹ�����滻RawImage����
    public PackedImg[] imgsbuffer;// client������Imgs

    private void Awake()
    {
        imgsbuffer = new PackedImg[rawImages.Length];
        albums = new Album[rawImages.Length];
    }

    void Start()
    {
        Init();
    }
    //Add Listener
    private void Init()
    {
        BackBtn.onClick.AddListener(OnClickBack);

        for (int i = 0; i < albums.Length; i++)
        {
            int index = i;
            albums[index] = new Album(rawImages[index], AlbumState.NULL);
            albums[index].UpdateState(AlbumState.NULL);

            deleteButtons[index].onClick.AddListener(() => OnDeleteButtonClicked(index));
        }

        foreach (var album in albums)
        {
            EventTrigger trigger = album.rawImage.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => OnAlbumImgClicked(album.rawImage));
            trigger.triggers.Add(entry);
        }

    }


    public void lockImgAt(int index)
    {
        if (index >= 0 && index < albums.Length)
        {
            albums[index].UpdateState(AlbumState.LOCKED);  
        }
        else
        {
            Debug.LogWarning("index out of albumLength");
        }
    }

    public void unlockImgAt(int index)
    {
        if (index >= 0 && index < albums.Length)
        {
            if(albums[index].state == AlbumState.LOCKED)
            {
                albums[index].UpdateState(AlbumState.DEFAULT);
            }
        }
        else
        {
            Debug.LogWarning("index out of albumLength");
        }
    }

    //�����п�����
    public void unlockImg(string userName, int pid)
    {
        for (int i = 0; i < albums.Length; i++)
        {
            if (albums[i].state == AlbumState.LOCKED && imgsbuffer[i].Equals(userName,pid))
            {
                unlockImgAt(i);
                break;
            }
        }
    }

    public void BurnPossibleImg(string userName, int pid)
    {
        for (int i = 0; i < albums.Length; i++)
        {
            if (albums[i].state == AlbumState.NULL) continue;
            if (imgsbuffer[i].Equals(userName, pid))
            {
                BurnPossibleImgAt(i);
            }
        }
    }

    public void BurnPossibleImgAt(int index)
    {
        if(index >= 0 && index < albums.Length) {
                RemoveImageAt(index);
        }
    }

    public void clearSelected()
    {
        if (albums == null) return;
        for (int i = 0; i < albums.Length; i++)
        {
            if (albums[i].state == AlbumState.SELECTED)
            {
                albums[i].UpdateState(AlbumState.DEFAULT);
            }
        }
    }


    // �ҵ�һ����λ����ͼƬ����
    public bool tryPutPhoto(PackedImg u)
    {
        for (int i = 0; i < albums.Length; i++)
        {
            if (albums[i].state == AlbumState.NULL)
            {
                imgsbuffer[i] = u;                                      
                Texture2D tex = new Texture2D(u.width, u.height);
                tex.LoadImage(u.ImageData);

                albums[i].rawImage.texture = tex;
                albums[i].UpdateState(AlbumState.DEFAULT);
                return true;
            }
        }
        return false;
    }

    public void OnDeleteButtonClicked(int index)
    {
        if (index >= 0 && index < albums.Length)
        {
            if (albums[index].state == AlbumState.NULL)  return; 
 
            if (albums[index].state == AlbumState.LOCKED)
            {
                photoClient.Instance.RemovePhotoAnywayRpc(
                    imgsbuffer[index].userName,
                    imgsbuffer[index].pid,
                    NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().playerinfo.userParty
                );
            }

            //ɾ��������е�ѡ��
            RemoveImageAt(index);
            clearSelected();
        }
    }

    public void RemoveImageAt(int index)
    {
        if(index >= 0 && index < albums.Length)
        {
            albums[index].rawImage.texture = null;
            imgsbuffer[index] = null;

            albums[index].UpdateState(AlbumState.NULL);
        }
    }

    public void OnClickBack()
    {
        panel.SetActive(false);
  
        UIManagers.GetComponentInChildren<BigmapUIManagers>().BigmapPanel.SetActive(true);
    }

    private void OnAlbumImgClicked(RawImage clickedRawImage)
    {
        // ���ұ������RawImage�������е��±�

        for (int i = 0; i < albums.Length; i++)
        {
            if (albums[i].rawImage == clickedRawImage)
            {
                //����������RawImage
                if (albums[i].state == AlbumState.DEFAULT)
                {
                    clearSelected();
                    albums[i].UpdateState(AlbumState.SELECTED);
                    break;
                }
            }
        }
    }

}