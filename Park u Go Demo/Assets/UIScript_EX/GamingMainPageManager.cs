using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamingMainPageManager : MonoBehaviour
{
    public GameObject GamingMainPage;
    public GameObject UiManager;
    public RectTransform SlidingImage; // ����ͼƬ�� RectTransform
    public float SlidingSpeed = 500f; // �����ٶ�

    private Vector2 hiddenPosition; // ͼƬ����λ��
    private Vector2 visiblePosition; // ͼƬ��ʾλ��
    private bool isImageVisible = false; // ��ǰͼƬ�Ƿ�ɼ�
    private Vector2 touchStart; // ��¼������ʼ��λ��

    // Start is called before the first frame update
    void Start()
    {
        // ��ʼ�����غ���ʾλ��
        hiddenPosition = new Vector2(-264, -38); // ���ص���Ļ��
        visiblePosition = new Vector2(-128, -38); // ��ȫ��ʾλ��
        SlidingImage.anchoredPosition = hiddenPosition; // ���ó�ʼ״̬Ϊ����
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

    // ��⻬������
    void Update()
    {
        DetectSwipe();
    }

    private void DetectSwipe()
    {
        if (Input.GetMouseButtonDown(0)) // ��¼��������갴�µ���ʼλ��
        {
            touchStart = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0)) // ��ⴥ��������ͷ�ʱ��λ��
        {
            Vector2 touchEnd = Input.mousePosition;
            float deltaX = touchEnd.x - touchStart.x; // ���㻬����ˮƽ����

            if (Mathf.Abs(deltaX) > 50) // �������������� 50 ����
            {
                if (deltaX > 0 && !isImageVisible) // �һ���ʾͼƬ
                {
                    ToggleSlidingImage(true);
                }
                else if (deltaX < 0 && isImageVisible) // ������ͼƬ
                {
                    ToggleSlidingImage(false);
                }
            }
        }
    }

    // ���ƻ���ͼƬ����ʾ������
    private void ToggleSlidingImage(bool show)
    {
        StopAllCoroutines(); // ֹͣ��ǰ����Э�̣���ֹ��ͻ
        if (show)
        {
            StartCoroutine(SlideTo(visiblePosition)); // ��������ʾλ��
        }
        else
        {
            StartCoroutine(SlideTo(hiddenPosition)); // ����������λ��
        }
        isImageVisible = show; // ����ͼƬ״̬
    }

    // Э��ʵ��ƽ������
    private IEnumerator SlideTo(Vector2 targetPosition)
    {
        while (Vector2.Distance(SlidingImage.anchoredPosition, targetPosition) > 1f)
        {
            SlidingImage.anchoredPosition = Vector2.Lerp(
                SlidingImage.anchoredPosition,
                targetPosition,
                SlidingSpeed * Time.deltaTime
            );
            yield return null; // �ȴ���һ֡
        }
        SlidingImage.anchoredPosition = targetPosition; // ȷ����ȷͣ��Ŀ��λ��
    }
}
