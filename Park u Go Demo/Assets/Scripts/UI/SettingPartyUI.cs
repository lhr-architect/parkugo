
using Unity.Netcode;
using UnityEngine;

public class SettingPartyUI : MonoBehaviour
{
    public SettingPartyUIManager manager;
    public void OnEnable()
    {
        if(NetworkManager.Singleton != null && NetworkManager.Singleton.IsConnectedClient)
        {
            manager.refreshUi();
        }
    }
}
