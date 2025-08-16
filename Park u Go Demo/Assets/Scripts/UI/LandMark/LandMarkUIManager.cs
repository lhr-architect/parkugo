using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Mathematics;
using System.Reflection;
using Unity.VisualScripting;
using static AlbumUIManager;



public class LandmarkUIManager : MonoBehaviour
{
    public GameObject UiManager;
    public GameObject panel;
    public Button BackBtn;

    private GameObject[] picBuffers;
    public GameObject PicPrefab;

    public Material LineMaterial;


    public RawImage[] rawImages; 
    public Button[] deleteBtns;

    private PackedImg[] LocalImgBuffer;

    private System.Random catRand;
    private List<GameObject> LinerenderPrefabs;

    //��ͼƬ��д��ʱ�򣬲�������UI��ת�¼�
    private int savingImagesCount = 0;

    public int PhotoIndex {  get; private set; }



    void Start()
    {
        InitLineRenderer();
        picBuffers = new GameObject[rawImages.Length];
        LocalImgBuffer = new PackedImg[rawImages.Length];

        BackBtn.onClick.AddListener(OnClickBack);

        //���ɾ��
        for (int i = 0; i < rawImages.Length; i++)
        {
            int index = i;
            deleteBtns[index].onClick.AddListener(() => OnDeleteButtonClicked(index));
        }

        //���ͼƬ
        foreach (var rawImage in rawImages)
        {
            EventTrigger trigger = rawImage.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => OnRawImageClicked(rawImage));
            trigger.triggers.Add(entry);
        }

        catRand = new System.Random();
    }

    private void InitLineRenderer()
    {
        LinerenderPrefabs = new List<GameObject>();
    }

    public void OnClickBack() {

        panel.SetActive(false);
        UiManager.GetComponentInChildren<BigmapUIManagers>().BigmapPanel.SetActive(true);       
    
    }

    public void OnDeleteButtonClicked(int index)
    {
        if (savingImagesCount > 0) return;
        
        //To do ����ݵ��״̬����Default return
        

        if (index >= 0 && index < rawImages.Length && LocalImgBuffer[index] != null)
        {
            //�����Լ��ĵ� ����ɾ��
            if (LocalImgBuffer[index].userName != NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().playerinfo.userName) { return; }

            //To do ����LastHitLandmark
            photoClient.Instance.RemoveLandmarkPhotoRpc(
                photoClient.Instance.LastHitLandmarkName, index,
                NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().playerinfo.userParty
            );
            
            RemoveRawImageAt(index);

            Line();

            //�ҵ��Լ��������Ƭ�������
            var info = photoClient.Instance.cmd.GetImgInfoAt(index);
            UiManager.GetComponentInChildren<AlbumUIManager>().unlockImg(info.Item1, info.Item2);
        }
          
    }

    private void OnRawImageClicked(RawImage clickedRawImage)
    {
        //�ȴ�����������Ƭ
        if (savingImagesCount > 0) return;
      
        int index = Array.IndexOf(rawImages, clickedRawImage);
        if (index != -1)
        {
            //����Ƭ���õ��
            if (LocalImgBuffer[index] != null) return;

            PhotoIndex = index;

            //To do �л�
            panel.SetActive(false);
            UiManager.GetComponentInChildren<LmSelectUIManager>().panel.SetActive(true);   
        }
        else
        {
            Debug.LogWarning($"RawImage index out {PhotoIndex}");
        }
    }

    // ���UIʱ���ã�����ɻ���
    public void clearOldData()
    {
        for (int i = 0; i < rawImages.Length; i++)
        {
            rawImages[i].texture = Resources.Load<Texture2D>($"BadCats/batcat_{catRand.Next(1,10)}");
            LocalImgBuffer[i] = null;

            if (picBuffers[i] != null)
            {
                DestroyImmediate(picBuffers[i]);
                picBuffers[i] = null;
            }
        }
        ClearAllLine();
    }

    public void SaveImg(PackedImg u, Command cmd)
    {
        savingImagesCount++;
        StartCoroutine(SaveImages2UI(u, cmd));
    }

    IEnumerator SaveImages2UI(PackedImg u, Command cmd)
    {
        yield return null;

        int Index = cmd.GetIndex(u.userName, u.pid);
        AddRawImageAt(Index, u);

        savingImagesCount--;
        if (savingImagesCount == 0) { 
            Line(); 
        }
    }


    private void AddRawImageAt(int index, PackedImg u)
    {
        //��PackedImg �浽��Ӧ��RawImage��  
        if (index >= 0 && index < rawImages.Length)
        {
            LocalImgBuffer[index] = u;

            byte[] data = u.ImageData;
            Texture2D tex = new Texture2D(u.width, u.height);
            tex.LoadImage(data);
            rawImages[index].texture = tex;

            GameObject instance = Instantiate(PicPrefab, u.POSITION, Quaternion.identity);
            picBuffers[index] = instance;
        }
    }

    private void RemoveRawImageAt(int index)
    {
        LocalImgBuffer[index] = null;

        rawImages[index].texture = Resources.Load<Texture2D>($"BadCats/batcat_{catRand.Next(1, 10)}");

        Destroy(picBuffers[index]);
    }


    //������ʹ��
    private GameObject CreateNewLine(Vector3 Start, Vector3 End)
    {
        GameObject obj = new GameObject();
        LineRenderer lr = obj.AddComponent<LineRenderer>();

        lr.startWidth = 10f; lr.startColor = Color.yellow;
        lr.endWidth = 3f; lr.endColor = Color.white;
        lr.material = LineMaterial;

        lr.sortingLayerID = SortingLayer.NameToID("Line");


        lr.positionCount = 2;
        lr.SetPosition(0, Start);
        lr.SetPosition(1, End);

        return obj;

    }

    private void Line()
    {
        // ��ʼ��һ���б����洢��Ч������λ��

        ClearAllLine();

        for (int i = 0; i < rawImages.Length; i += 3)
        {
            // ����������
            if (LocalImgBuffer[i] != null && LocalImgBuffer[i + 1] != null)
            {
                LinerenderPrefabs.Add(
                    CreateNewLine(LocalImgBuffer[i].POSITION, LocalImgBuffer[i + 1].POSITION)
                );
            }
            if (LocalImgBuffer[i + 1] != null && LocalImgBuffer[i + 2] != null)
            {
                LinerenderPrefabs.Add(
                    CreateNewLine(LocalImgBuffer[i + 1].POSITION, LocalImgBuffer[i + 2].POSITION)
                );
            }
            if (LocalImgBuffer[i + 2] != null && LocalImgBuffer[i] != null)
            {
                LinerenderPrefabs.Add(
                    CreateNewLine(LocalImgBuffer[i + 2].POSITION, LocalImgBuffer[i].POSITION)
                );
            }
        }

    }

    public void ClearAllLine()
    {
        foreach (var lr in LinerenderPrefabs)
        {
            if(lr != null)
            {
                lr.gameObject.GetComponent<LineRenderer>().positionCount = 0;
                DestroyImmediate(lr);
            }
        }
        LinerenderPrefabs.Clear();
    }
}