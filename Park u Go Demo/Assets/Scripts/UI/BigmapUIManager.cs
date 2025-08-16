
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class BigmapUIManagers : MonoBehaviour
{

    public GameObject UIManagers;
    public GameObject BigmapPanel;
    public GameObject MapObj;

    public Button upBtn;
    public Button downBtn;
    public Button leftBtn;
    public Button rightBtn;

    public RawImage[] redheroIcons, blueheroIcons;
    public RawImage[] redhealthIcons, bluehealthIcons;

    public RawImage camBtnImg;
    public SpriteRenderer bigmapRender;


    public TextMeshProUGUI cdTextMeshPro;

    private bool Added = false;
    private int cd;

    //Life �ؼ�
    public GameObject fullHeart, halfHerat, deadHeart;

    public RectTransform SlidingImage; // ����ͼƬ�� RectTransform
    public float SlidingSpeed = 500f; // �����ٶ�

    public Vector2 hiddenPosition;        // ͼƬ����λ��
    public Vector2 visiblePosition;       // ͼƬ��ʾλ��
    private bool isImageVisible = false;  // ��ǰͼƬ�Ƿ�ɼ�
    private Vector2 touchStart;           // ��¼������ʼ��λ��


    public TextMeshProUGUI redSun, blueSum,TimeBoard, MsgBorad;
    public bool Alive;

    private void Start()
    {
        // ��ʼ�����غ���ʾλ��
        hiddenPosition = new Vector2(-263, 3); // ���ص���Ļ��
        visiblePosition = new Vector2(-135, 3); // ��ȫ��ʾλ��
        SlidingImage.anchoredPosition = hiddenPosition; // ���ó�ʼ״̬Ϊ����
        Alive = true;

        requestBindMoveBtn();
    }


    void Update()
    {
     
        // ������������
        if (BigmapPanel.activeSelf)
        {

            if(NetworkManager.Singleton != null && NetworkManager.Singleton.IsConnectedClient)
            {
                redSun.text = $"{GameManager.instance.redSummary.Value}";
                blueSum.text = $"{GameManager.instance.blueSummary.Value}";

                cd = (int)(NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().uploadCD - Time.time + NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().lastUploadTime);
                cd = cd < 0 ? 0 : cd; 
                cdTextMeshPro.text = cd.ToString();
            }

            if (Input.GetMouseButtonDown(0)) {

                touchStart = Input.mousePosition;
                // �������λ��ת��Ϊ����
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
                if (hit.collider != null)
                {
                    if (hit.collider.tag == "LandMark")
                    {
                        OnHitLandmark(hit.collider.gameObject);
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0)) // ��ⴥ��������ͷ�ʱ��λ��
            {
                Vector2 touchEnd = Input.mousePosition;
                float deltaX = touchEnd.x - touchStart.x; // ���㻬����ˮƽ����

                if (Mathf.Abs(deltaX) > 200) // �������������� 50 ����
                {
                    if (deltaX > 0 && !isImageVisible) // �һ���ʾͼƬ
                    {
                        GetSideBarInfo();
                        ToggleSlidingImage(true);
                    }
                    else if (deltaX < 0 && isImageVisible) // ������ͼƬ
                    {
                        ToggleSlidingImage(false);
                    }
                }
            }
        }

    }

     public Texture2D FindTexofHealth(int health)
    {
        if(health > 2) { return  null; }
        else if(health == 2)
        {
            return Resources.Load<Texture2D>("Life/full");
        }
        else if(health == 1)
        {
            return Resources.Load<Texture2D>("Life/half");
        }
        else
        {
            return Resources.Load<Texture2D>("Life/dead");
        }
    }

    public void GetSideBarInfo()
    {
        PlayerController[] playerList = FindObjectsOfType<PlayerController>();
        int redIndex = 0, blueIndex = 0;
        foreach (PlayerController player in playerList)
        {
            if(player.party.Value  == PlayerController.Party.RED)
            {
                redheroIcons[redIndex].texture = GameManager.instance.FindHeroIcon_Ui(player.playerinfo.heroType, PlayerController.Party.RED);
                redhealthIcons[redIndex].texture = FindTexofHealth(player.health.Value); 
                redIndex++;
            }
            else
            {
                blueheroIcons[blueIndex].texture = GameManager.instance.FindHeroIcon_Ui(player.playerinfo.heroType, PlayerController.Party.BLUE);
                bluehealthIcons[redIndex].texture = FindTexofHealth(player.health.Value);
                blueIndex++;
            }
        }

        while(redIndex < 5)
        {
            redheroIcons[redIndex].texture = null;
            redhealthIcons[redIndex].texture = null;
            redIndex++;
        }

        while (blueIndex < 5)
        {
            blueheroIcons[blueIndex].texture = null;
            bluehealthIcons[blueIndex].texture = null;
            blueIndex++;
        }
    }

    public void addBoardcastMsg(string Msg, string time)
    {
        MsgBorad.text = Msg;
        TimeBoard.text = time;
    }

    public void setHealth(int health)
    {
        if (health > 2) { return; }
        else if(health == 2) 
        {
            fullHeart.SetActive(true);

            halfHerat.SetActive(false);
            deadHeart.SetActive(false);

            Alive = true;
            MapObj.GetComponent<SpriteRenderer>().color = Color.white;
        }
        else if(health == 1) 
        {
            halfHerat.SetActive(true);

            fullHeart.SetActive(false);
            deadHeart.SetActive(false);
            Alive = true;

            MapObj.GetComponent<SpriteRenderer>().color = Color.white;
        }
        else
        {
            deadHeart.SetActive(true);

            fullHeart.SetActive(false);
            halfHerat.SetActive(false);
            Alive = false;

            //To do ���
            MapObj.GetComponent<SpriteRenderer>().color = Color.gray;
        }
    }

    public void requestBindMoveBtn()
    {
        if(!Added)
        {
            upBtn.onClick.AddListener(() => NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>()?.MovePlayer(Vector3.up));
            downBtn.onClick.AddListener(() => NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>()?.MovePlayer(Vector3.down));
            leftBtn.onClick.AddListener(() => NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>()?.MovePlayer(Vector3.left));
            rightBtn.onClick.AddListener(() => NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>()?.MovePlayer(Vector3.right));

            Added = true;
        }

    }

    
    void OnHitLandmark(GameObject hitObject)
    {
        // �������д����¼��Ĵ���

        Landmark hitLandmark = hitObject.GetComponent<Landmark>();

        BigmapPanel.SetActive(false);
        
        photoClient.Instance.LastHitLandmarkName = hitLandmark.LandmarkName;
        photoClient.Instance.LastHitLandmark = hitLandmark;

        //To do �˴��ж���3���Ļ���6����
        UIManagers.GetComponentInChildren<LandmarkUIManager>().panel.SetActive(true);

    }

    public void onClickCamera()
    {
        if (!Alive) return;

        BigmapPanel.SetActive(false);
        UIManagers.GetComponentInChildren<GamingCameraUIManager>().panel.SetActive(true);
    }

    public void onClickAlbum()
    {
        if (!Alive) return;

        BigmapPanel.SetActive(false);
        UIManagers.GetComponentInChildren<AlbumUIManager>().panel.SetActive(true);
    }

    public void onClickAttack()
    {
        if (!Alive || cd > 0) return;
        
        BigmapPanel.SetActive(false);
        UIManagers.GetComponentInChildren<GamingAttackUIManager>().panel.SetActive(true);
    }

    public void onClickSkill()
    {

    }

    public void onClickBoard()
    {

    }

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

    public void refreshUi()
    {
        GameManager.WorldTopic worldTopic = GameManager.instance.getPartyWorldTopic(
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().playerinfo.userParty
        );

        camBtnImg.texture = GameManager.FindTextureWithTopic(worldTopic, "icon_photo");


        bigmapRender.sprite = Resources.Load<Sprite>($"{worldTopic}/{worldTopic}_background_map");

    }
}