using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GoUI : MonoBehaviour
{
    public GoUIManager manager;
    // Start is called before the first frame update
    private void OnEnable()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsConnectedClient)
        {
            manager.refreshUi();
        }
    }
}
