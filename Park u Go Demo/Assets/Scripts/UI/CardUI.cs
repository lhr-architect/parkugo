using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    // Start is called before the first frame update
    public IManager manager;

    private void OnEnable()

    {   if(manager != null)
        {
            manager.GetEffectedFromAlbum();
            manager.AlbumIndex = -1;
        }

    }
}
