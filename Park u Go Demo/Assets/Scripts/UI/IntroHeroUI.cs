
using UnityEngine;

public class IntroHeroUI : MonoBehaviour
{
    public IntroHeroUIManager manager;
    private void OnEnable()
    {
        manager.refreshUi();
    }
}
