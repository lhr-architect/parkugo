using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LandMarkUI : MonoBehaviour
{
    // Start is called before the first frame update

    public LandmarkUIManager manager;
    
    //在每次Landmark Panel Active的时候去请求服务器数据
    private void OnEnable()
    {
        if (photoClient.Instance != null)
        {
            //Debug.LogWarning(photoClient.Instance.LastHitLandmarkName);

            manager.clearOldData();
            photoClient.Instance.clearQueue();

            photoClient.Instance.GetImgUIPositionRpc(
                photoClient.Instance.UsernameGlobal, 
                "", 
                photoClient.Instance.LastHitLandmarkName,
                NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().playerinfo.userParty
            );

            photoClient.Instance.GetPhotoRpc(
                photoClient.Instance.LastHitLandmarkName,
                photoClient.Instance.UsernameGlobal,
                NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().playerinfo.userParty
            );

        }
    }


    private void OnDisable()
    {
        manager.GetComponent<LandmarkUIManager>().clearOldData();
    }




}
