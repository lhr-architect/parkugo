
using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    public LobbyUIManager manager;
    // Start is called before the first frame update
    private void OnEnable()
    {
        manager.refreshUi();
    }
}
