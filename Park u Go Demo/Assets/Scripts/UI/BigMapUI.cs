using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BigMapUI : MonoBehaviour
{
    public BigmapUIManagers myManager;
    // Start is called before the first frame update

    private void OnEnable()
    {

        if(NetworkManager.Singleton != null && NetworkManager.Singleton.IsConnectedClient)
        {
            myManager.setHealth(NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().health.Value);
            myManager.refreshUi();
        }

    }
}
