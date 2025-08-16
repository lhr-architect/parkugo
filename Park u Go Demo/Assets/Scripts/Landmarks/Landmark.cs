using JetBrains.Annotations;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine; // ����LitJson��

public class Landmark : MonoBehaviour
{
    public string LandmarkName;
    public string description;
    public int IndexMax;
    
    private float radius;


    LandMarkState state;

    public enum LandMarkState { 
        DEFAULT,
        RED,
        BLUE
    }

   
    public PackedImg[] red {  get; private set; }
    public PackedImg[] blue { get; private set; }


    private int redCnt = 0; // ����������¼red�����зǿ�PackedImg������
    private int blueCnt = 0; // ����������¼blue�����зǿ�PackedImg������
    private bool CheckIfFull(PlayerController.Party party)
    {
        if(party == PlayerController.Party.RED)
        {
            return redCnt == IndexMax;
        }
        else
        {
            return blueCnt == IndexMax;
        }
    } 
    private void Start()
    {
        red = new PackedImg[IndexMax];
        blue = new PackedImg[IndexMax];
        state = LandMarkState.DEFAULT;
        radius = GetComponent<CircleCollider2D>().radius;
    }


    // ��ͼƬ�б���ʱ�����ķ���
    public void OnImgFull()
    {
        // ���������ӵ�ͼƬ�б���ʱ��Ҫִ�еĴ���
        Debug.Log($"The image list for {LandmarkName} is full.");
    }

    // �������ͼƬ�ķ���
    public void ClearAllImgs()
    {
        for (int i = 0; i < IndexMax; i++)
        {
            red[i] = null;
            blue[i] = null;
        }
    }

    public void ClearImg(PlayerController.Party party)
    {
        if(party == PlayerController.Party.RED)
        {
            for (int i = 0; i < IndexMax; i++)
            {
                red[i] = null;
            }
        }
        else
        {
            for (int i = 0; i < IndexMax; i++)
            {
                blue[i] = null;
            }
        }
    }

    public void ClearBothImg(int groupId)
    {
        if (red[groupId] != null) { red[groupId] = null; redCnt--; }
        if (red[groupId + 1] != null) { red[groupId + 1] = null; redCnt--; }
        if (red[groupId + 2] != null) { red[groupId + 2] = null; redCnt--; }

        if (blue[groupId] != null) { blue[groupId] = null; blueCnt--; }
        if (blue[groupId + 1] != null) { blue[groupId + 1] = null; blueCnt--; }
        if (blue[groupId + 2] != null) { blue[groupId + 2] = null; blueCnt--; }
    }

    public int checkNearBy(PlayerController.Party party)
    {
        int result = 0;
        if(party == PlayerController.Party.RED)
        {
            for(int i = 0;i < IndexMax; i++)
            {
                if (red[i] == null) continue;
                if (Vector2.Distance(new Vector2(red[i].x, red[i].y), new Vector2(transform.position.x, transform.position.y)) <= radius)
                {
                    result++;
                }
            }
        }
        else
        {
            for (int i = 0; i < IndexMax; i++)
            {
                if (blue[i] == null) continue;
                if (Vector3.Distance(new Vector3(blue[i].x, blue[i].y, 0), transform.position) <= radius)
                {
                    result++;
                }
            }
        }
        return result;
    }

    private void copyImgTo(PlayerController.Party party)
    {
        //blue to red
        if (party == PlayerController.Party.RED)
        {
            for (int i = 0; i < IndexMax; i++)
            {
                red[i] = blue[i];
            }
        }
        else
        {
            for (int i = 0; i < IndexMax; i++)
            {
                blue[i] = red[i];
            }
        }
    }

 

    public bool tryAddImageAt(PackedImg img, int positon, PlayerController.Party party)
    {
        bool flag = false;
        // find img.userName belongs to 
        if (positon >= 0 && positon < IndexMax)
        {
            if(party == PlayerController.Party.RED)
            {
                //���ԭλ�ǿգ���һ����
                if(red[positon] == null) { redCnt++; flag = true; }
                red[positon] = img;

                //������Ƭcheckһ���Ƿ�����
                if(CheckIfFull(PlayerController.Party.RED) && checkNearBy(PlayerController.Party.RED) == IndexMax)
                {
                    //To do ��ȫ��뾶
                    photoServer.Instance.BurnImgIn(this, PlayerController.Party.RED);
                    state = LandMarkState.RED; 

                    copyImgTo(PlayerController.Party.BLUE);
                }
            }
            else
            {
                if (blue[positon] == null) { blueCnt++; flag = true; }
                blue[positon] = img;

                //������Ƭcheckһ���Ƿ�����
                if (CheckIfFull(PlayerController.Party.BLUE) && checkNearBy(PlayerController.Party.BLUE) == IndexMax)
                {
                    photoServer.Instance.BurnImgIn(this, PlayerController.Party.BLUE);
                    state = LandMarkState.BLUE;

                    copyImgTo(PlayerController.Party.RED);
                }
            }
            return flag;
        }
        else
        {
            return false;
        }
    }

    public bool RemoveImageAt(int positon, PlayerController.Party party)
    {
        if (positon >= 0 && positon < IndexMax)
        {
            if (party == PlayerController.Party.RED)
            {
                //ȷ��Ҫ�Ƴ�һ��
                if (red[positon] != null) {

                    if(state != LandMarkState.DEFAULT)
                    {
                        int Group = positon / 3;
                        ClearBothImg(Group);

                        //����ˣ���ع�Ĭ��״̬
                        if(checkNearBy(party) == 0)
                        {
                            state = LandMarkState.DEFAULT;
                        }
                    }
                    else
                    {
                        red[positon] = null;
                        redCnt--;
                    }
       
                }
            }
            else
            {
                if (blue[positon] != null) {

                    if (state != LandMarkState.DEFAULT)
                    {
                        int Group = positon / 3;
                        ClearBothImg(Group); //�����˼��һ��

                        //����ˣ��ع�Ĭ��״̬
                        if (checkNearBy(party) == 0)
                        {
                            state = LandMarkState.DEFAULT;
                        }
                    }
                    else 
                    {
                        blue[positon] = null;
                        blueCnt--;
                    }

                }

            }
            return true;
        }
        else
        {
            Debug.LogWarning($"Landmark {LandmarkName} try to remove at {positon} ");
            return false;
        }
    }

    public bool tryRemoveImage(string userName, int pid, PlayerController.Party party)
    {
        if (party == PlayerController.Party.RED)
        {
            for (int i = 0; i < IndexMax; i++)
            {
                if (red[i] != null && red[i].Equals(userName, pid))
                {
                    RemoveImageAt(i, party);
                    return true;
                }
            }
            return false;
        }
        else
        {
            for (int i = 0; i < IndexMax; i++)
            {

                if (blue[i] != null && blue[i].Equals(userName, pid))
                {

                    RemoveImageAt(i, party);
                    return true;
                }
            }
            return false;
        }
        
    }

}