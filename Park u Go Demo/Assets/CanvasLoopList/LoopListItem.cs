using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoopListItem : MonoBehaviour {


    public static float[] posX =  new float[5]  { -600f, -300f, 0f,   300f, 600f };
    public static float[] size =  new float[5]  { 180f,  210f,  250f, 210f, 180f };
    public static float[] Alpha = new float[5]  { 0.5f,  0.9f,  1f,   0.9f, 0.5f };

    private int MID = 2;

    public int dataIndex;
    public int uiIndex;

    [SerializeField]
    private TMP_Text nameText;
    [SerializeField]
    private RawImage heroImg;


    private void Start()
    {
        nameText = GetComponentInChildren<TMP_Text>();
        heroImg = GetComponent<RawImage>();
    }

    public void SetData(Data data) {
        
        nameText.text = data.name;
        if(!data.name.Equals(""))
        {
            heroImg.texture = GameManager.instance.FindHeroIcon_Ui(data.hero, data.party);
        }
        else
        {
            heroImg.texture = null;
        }
    }

    public void ShiftRight(float offset, float duration) 
    {
        StartCoroutine(ShiftCoroutine(1, duration, offset));
    }
    
    public void ShiftLeft(float offset, float duration) 
    {
        StartCoroutine(ShiftCoroutine(-1, duration, offset));
    }

    private IEnumerator ShiftCoroutine(int direction, float duration, float offset) 
    {
        direction /= Mathf.Abs(direction);
        uiIndex += direction;

        Vector3 newPos = new(posX[(MID + uiIndex)], 0, 0);
        GetComponent<RectTransform>().sizeDelta = new Vector2(size[(MID + uiIndex)], size[(MID + uiIndex)]);
        GetComponent<RawImage>().color = new Vector4(1f, 1f, 1f, Alpha[(MID + uiIndex)]);

        float currentTime = 0;
        while (currentTime < duration) {
            currentTime += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(transform.localPosition, newPos, currentTime / duration);
            yield return null;
        }
        transform.localPosition = newPos;
    }

}
