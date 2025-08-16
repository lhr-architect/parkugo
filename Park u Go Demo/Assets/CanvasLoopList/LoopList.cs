
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class LoopList : MonoBehaviour {

    public Data[] data;
    public float offset;
    public float duration;
    public float HitTime;

    public static float[] posX = new float[5] { -600f, -300f, 0f, 300f, 600f };
    public static float[] size = new float[5] { 180f, 210f, 250f, 210f, 180f };
    public static float[] Alpha = new float[5] { 0.5f, 0.9f, 1f, 0.9f, 0.5f };

    public Data GetData(int index) {
        return data[(index % data.Length)];
    }

    public LoopListItem[] children;
    public LinkedList<LoopListItem> childrenDeque = new LinkedList<LoopListItem>();

    private void Start() {

        foreach (LoopListItem item in children) {
            item.GetComponent<Button>().onClick.AddListener(() => Shift(item));
        }

        refreshLoopList();
    }

    public void refreshLoopList()
    {
        //还原本身的位置
        for (int i = 0; i < children.Length; i++)
        {
            children[i].uiIndex = children[i].dataIndex;
        }

        childrenDeque.Clear();

        foreach (LoopListItem item in children)
        {
            childrenDeque.AddLast(item);
        }
        fillUiWithData();
    }

    public int findCenterHeroDataIndex()
    {
        foreach (LoopListItem item in childrenDeque)
        {
            if(item.uiIndex == 0)
            {
                return item.dataIndex + 2;
            }
        }
        return -1;
    }

    public void fillUiWithData()
    {
        foreach (LoopListItem item in childrenDeque)
        {
            item.SetData(GetData(item.uiIndex + 2));

            item.gameObject.transform.localPosition = new(posX[(2 + item.uiIndex)], 0, 0);
            item.GetComponent<RectTransform>().sizeDelta = new Vector2(size[(2 + item.uiIndex)], size[(2 + item.uiIndex)]);
            item.GetComponent<RawImage>().color = new Vector4(1f, 1f, 1f, Alpha[(2 + item.uiIndex)]);
        }
    }

    private void Shift(LoopListItem item) {
        switch (item.uiIndex) {
            case < 0:
                ShiftRight();
                break;
            case > 0:
                ShiftLeft();
                break;
        }
    }

    private void ShiftRight() {

        float nowTime = Time.time; 
        if (nowTime - HitTime > duration)
        {
            HitTime = nowTime;
            ReplaceRightOverflow();
            foreach (LoopListItem item in children)
            {
                item.ShiftRight(offset, duration);
            }
        }
    }

    private void ReplaceRightOverflow() {
        LoopListItem last = childrenDeque.Last.Value;
        childrenDeque.RemoveLast();

        last.uiIndex = -last.uiIndex - 1;

        childrenDeque.AddFirst(last);
    }

    private void ShiftLeft() {
        float nowTime = Time.time;
        if (nowTime - HitTime > duration)
        {
            HitTime = nowTime;
            ReplaceLeftOverflow();

            foreach (LoopListItem item in children)
            {
                item.ShiftLeft(offset, duration);
            }
        }
    }


    private void ReplaceLeftOverflow() {
        LoopListItem first = childrenDeque.First.Value;
        childrenDeque.RemoveFirst();

        first.uiIndex = Mathf.Abs(first.uiIndex) + 1;

        childrenDeque.AddLast(first);
    }

}

