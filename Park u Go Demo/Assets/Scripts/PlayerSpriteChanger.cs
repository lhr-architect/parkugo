using Unity.Netcode;
using UnityEngine;

public class PlayerSpriteChanger : NetworkBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    [Rpc(SendTo.Server)]
    public void requestChangePlayerIconRpc(string userName, PlayerController.Party p, PlayerController.Hero hero)
    {
        if(!IsServer) return;



    }



}