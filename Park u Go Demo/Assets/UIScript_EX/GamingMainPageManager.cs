using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamingMainPageManager : MonoBehaviour
{
    public GameObject GamingMainPage;
    public GameObject UiManager;
    public RectTransform SlidingImage; // 滑动图片的 RectTransform
    public float SlidingSpeed = 500f; // 滑动速度

    private Vector2 hiddenPosition; // 图片隐藏位置
    private Vector2 visiblePosition; // 图片显示位置
    private bool isImageVisible = false; // 当前图片是否可见
    private Vector2 touchStart; // 记录滑动开始的位置

    // Start is called before the first frame update
    void Start()
    {
        // 初始化隐藏和显示位置
        hiddenPosition = new Vector2(-264, -38); // 隐藏到屏幕外
        visiblePosition = new Vector2(-128, -38); // 完全显示位置
        SlidingImage.anchoredPosition = hiddenPosition; // 设置初始状态为隐藏
    }

    public void onClickCamera()
    {
        GamingMainPage.SetActive(false);
        UiManager.GetComponentInChildren<GamingCameraUIManager>().panel.SetActive(true);
    }

    public void onClickAlbum()
    {
        GamingMainPage.SetActive(false);
        UiManager.GetComponentInChildren<GamingAlbumManager>().GamingAlbum.SetActive(true);
    }

    public void onClickAttack()
    {
        GamingMainPage.SetActive(false);
        UiManager.GetComponentInChildren<GamingAttackUIManager>().panel.SetActive(true);
    }

    public void onClickSkill()
    {
        GamingMainPage.SetActive(false);
        UiManager.GetComponentInChildren<GamingSkillManager>().GamingSkill.SetActive(true);
    }

    public void onClickBoard()
    {
        GamingMainPage.SetActive(false);
        UiManager.GetComponentInChildren<GamingBoardManager>().GamingBoard.SetActive(true);
    }

    // 检测滑动手势
    void Update()
    {
        DetectSwipe();
    }

    private void DetectSwipe()
    {
        if (Input.GetMouseButtonDown(0)) // 记录触摸或鼠标按下的起始位置
        {
            touchStart = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0)) // 检测触摸或鼠标释放时的位置
        {
            Vector2 touchEnd = Input.mousePosition;
            float deltaX = touchEnd.x - touchStart.x; // 计算滑动的水平距离

            if (Mathf.Abs(deltaX) > 50) // 如果滑动距离大于 50 像素
            {
                if (deltaX > 0 && !isImageVisible) // 右滑显示图片
                {
                    ToggleSlidingImage(true);
                }
                else if (deltaX < 0 && isImageVisible) // 左滑隐藏图片
                {
                    ToggleSlidingImage(false);
                }
            }
        }
    }

    // 控制滑动图片的显示和隐藏
    private void ToggleSlidingImage(bool show)
    {
        StopAllCoroutines(); // 停止当前所有协程，防止冲突
        if (show)
        {
            StartCoroutine(SlideTo(visiblePosition)); // 滑动到显示位置
        }
        else
        {
            StartCoroutine(SlideTo(hiddenPosition)); // 滑动到隐藏位置
        }
        isImageVisible = show; // 更新图片状态
    }

    // 协程实现平滑滑动
    private IEnumerator SlideTo(Vector2 targetPosition)
    {
        while (Vector2.Distance(SlidingImage.anchoredPosition, targetPosition) > 1f)
        {
            SlidingImage.anchoredPosition = Vector2.Lerp(
                SlidingImage.anchoredPosition,
                targetPosition,
                SlidingSpeed * Time.deltaTime
            );
            yield return null; // 等待下一帧
        }
        SlidingImage.anchoredPosition = targetPosition; // 确保精确停在目标位置
    }
}
