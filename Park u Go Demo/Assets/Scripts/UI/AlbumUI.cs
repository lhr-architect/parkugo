using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlbumUI : MonoBehaviour
{
    public GameObject Manager;
    private void OnEnable()
    {
        Manager.GetComponent<AlbumUIManager>().clearSelected();
    }
    private void OnDisable()
    {
        Manager.GetComponent<AlbumUIManager>().clearSelected();
    }
}
