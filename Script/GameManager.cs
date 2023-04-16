using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<GameObject> ObjectList = new List<GameObject>(); //������ ������Ʈ ���� ����Ʈ

    [HideInInspector]
    public List<Dictionary<string, object>> GameData; //csv���� �ҷ���
    [HideInInspector]
    public List<Dictionary<string, object>> ItemData; //csv���� �ҷ���
    [HideInInspector]
    public List<Dictionary<string, object>> LevelData; //csv���� �ҷ���

    //...UI ����...//
    public Text u_NowScoreText;
    public Text u_HighScoreText;
    public Text u_NowSheepText;
    public Text u_HighSheepText;

    public Text u_OverNowScoreText;
    public Text u_OverHighScoreText;
    public Text u_OverNowSheepText;
    public Text u_OverHighSheepText;

    public Slider u_TimeSlider;

    public GameObject GameOverPanel;
    public GameObject ItemPanel;

    public GameObject u_LevelPanel;
    public Sprite[] LevelSprite;

    //...�ý��� ����...//
    public int i_nowScore = 0; //���� �÷��� ���ھ�
    public int i_nowSheep = 0; //���� �÷��� ���� �� ��
    public int i_highScore = 0; //�ִ� ���ھ�
    public int i_highSheep = 0 ; //�ִ� ���� �� ��

    public int i_Object = 0; //���� �����ִ� ������Ʈ ��

    public bool isGameover = false; //���ӿ��� �Ǿ�����
    public bool isGameStart = false; //���ӽ�ŸƮ �Ǿ�����

    public bool isScoreItem = false; //���ھ� ������
    public bool isStopItem = false; //�ð����� ������
    public float f_ItemTime = 0; //������ �ð�

    S_Unity s_Unity;
    
    //...���� ����...//
    public int i_Level = 1; //����
    public float f_maxtime = 20f; //�ִ� ���� �ð�
    public float f_time = 20f; //�ǽð� �ð�
    public List<float> Level_Time, Level_Appearance_Speed;
    float f_gametime = 0f; //���� ���� �ð� (�������ϸ� 0���� �ʱ�ȭ)
    int i_Chagne = 1; //�㳷 �ٲ� Ƚ��

    //...���� ����...//
    public float f_objectSpawn = 0.8f; //������Ʈ ���� �ִ� �ð�



    void Awake()
    {
        GameData = CSVReader.Read("20210613_Sleep&Sheep_201813170");
        ItemData = CSVReader.Read("20210613_Sleep&Sheep_201813170_2");
        LevelData = CSVReader.Read("20210613_Sleep&Sheep_201813170_3");

        s_Unity = GameObject.Find("UnityManager").GetComponent<S_Unity>();

        s_Unity.Paid();
        s_Unity.UpdateSlider();
        s_Unity.BtnSet();

        StartCoroutine(GameStart());
    }

    void Start()
    {
        instance = this;
        i_highScore = PlayerPrefs.GetInt("HighScore");
        i_highSheep = PlayerPrefs.GetInt("HighSheep");
        
        u_HighScoreText.text = i_highScore.ToString();
        u_HighSheepText.text = "X" + i_highSheep.ToString();
        u_TimeSlider.maxValue = f_maxtime;

        for (int i = 0; i < LevelData.Count; i++)
        {
            Level_Time.Add(float.Parse(LevelData[i]["Level_Time"].ToString()) / 10);
            Level_Appearance_Speed.Add(float.Parse(LevelData[i]["Level_Appearance_Speed"].ToString()) / 10);
        }
    }

    void Update()
    {
        //...�ý��� ������Ʈ ����...//
        if (!isGameover && isGameStart)
        {
            if(!isStopItem)
                f_time -= Time.deltaTime;
            f_gametime += Time.deltaTime;
        } //���� ���� �߿� Ÿ�̸� �۵� �� ���� �ð� �÷���

        if (f_time <= 0)
        {
            isGameover = true;
            f_time = 0f;
            StartCoroutine(GameOver());
        }
        else if (f_time > f_maxtime)
            f_time = f_maxtime;

        if (f_gametime >= 20f * i_Level && i_Level < 10)
            StartCoroutine(LevelUp());
        else if(f_gametime >= 60f * i_Chagne && isGameStart && !isGameover)
            StartCoroutine(ChagneForm());

        //...������ ������Ʈ ����...//
        if (f_ItemTime > 0) {
            if (isStopItem || isScoreItem)
                f_ItemTime -= Time.deltaTime;
        }
        else {
            isScoreItem = false;
            isStopItem = false;
            ItemPanel.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
        }

        //...UI ������Ʈ ����...//
        u_TimeSlider.value = f_time; //���ӽð�
        u_NowScoreText.text = i_nowScore.ToString();
        u_NowSheepText.text = "X" + i_nowSheep.ToString();

        //...�ȵ���̵� �ڷΰ���...//
        if (Input.GetKey(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
                StopGame();
            else if(Time.timeScale == 0)
                GameObject.Find("Canvas").transform.Find("UI_QuitPanel").gameObject.SetActive(true);

            if(isGameover)
                GameObject.Find("Canvas").transform.Find("UI_QuitPanel").gameObject.SetActive(true);
        }
    }

    public void StopGame()//���� ����, �ɼǹ�ư
    {
        s_Unity.TouchSFX(6);

        if (Time.timeScale == 0)
            Time.timeScale = 1;
        else if(Time.timeScale == 1 && isGameover == false)
        {
            Time.timeScale = 0;
            GameObject.Find("Canvas").transform.Find("UI_StopPanel").gameObject.SetActive(true);
        }
    }
    public void QuitGame()//���� ����
    {
        s_Unity.TouchSFX(6);
        Application.Quit();
    }
    IEnumerator GameStart()
    {
        GameObject.Find("UnityManager").GetComponent<AudioSource>().Pause();

        yield return new WaitForSeconds(1.0f);
        Debug.Log("3");
        GameObject.Find("Canvas").transform.Find("UI_StartPanel").transform.Find("UI_3").gameObject.SetActive(true);
        s_Unity.TouchSFX(5);

        yield return new WaitForSeconds(1.0f);
        Debug.Log("2");
        GameObject.Find("Canvas").transform.Find("UI_StartPanel").transform.Find("UI_3").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("UI_StartPanel").transform.Find("UI_2").gameObject.SetActive(true);
        s_Unity.TouchSFX(5);
        
        yield return new WaitForSeconds(1.0f);
        Debug.Log("1");
        GameObject.Find("Canvas").transform.Find("UI_StartPanel").transform.Find("UI_2").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("UI_StartPanel").transform.Find("UI_1").gameObject.SetActive(true);
        s_Unity.TouchSFX(5);
        
        yield return new WaitForSeconds(1.0f);
        GameObject.Find("Canvas").transform.Find("UI_StartPanel").transform.Find("UI_1").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("UI_StartPanel").gameObject.SetActive(false);

        s_Unity.BGMPlay(1);
        GameObject.Find("UnityManager").GetComponent<AudioSource>().Play();
        isGameStart = true;
        
        yield return 0;
    }
    IEnumerator GameOver()//���ӿ��� �г� ������Ʈ
    {

        if (i_nowScore > i_highScore)
        {
            PlayerPrefs.SetInt("HighScore", i_nowScore);
            i_highScore = i_nowScore;
        }
        if (i_nowSheep > i_highSheep)
        {
            PlayerPrefs.SetInt("HighSheep", i_nowSheep);
            i_highSheep = i_nowSheep;
        }
        
        u_OverNowScoreText.text = i_nowScore.ToString();
        u_OverHighScoreText.text = i_highScore.ToString();
        u_OverNowSheepText.text = i_nowSheep.ToString();
        u_OverHighSheepText.text = i_highSheep.ToString();

        GameOverPanel.SetActive(true);
        yield return null;
    }
    IEnumerator ChagneForm()
    {
        i_Chagne++;

        if (GameObject.Find("BackGround").GetComponent<Animator>().GetBool("isNoon") == true)
        {
            s_Unity.BGMPlay(2);
            GameObject.Find("BackGround").GetComponent<Animator>().SetBool("isNoon", false);
        }
        else
        {
            s_Unity.BGMPlay(1);
            GameObject.Find("BackGround").GetComponent<Animator>().SetBool("isNoon", true);
        }
        StopCoroutine(ChagneForm());
        yield return null;
    }
    IEnumerator LevelUp()//������ �� �ð� ������Ʈ
    {
        i_Level++;
        u_LevelPanel.GetComponent<Image>().sprite = LevelSprite[i_Level - 1];
        float maxtime = 0.8f;
        float thistime = f_time / f_maxtime;

        for(int i = 0; i < LevelData.Count; i++)
        {
            if (i_Level == i + 1)
            {
                f_maxtime = Level_Time[i];
                f_objectSpawn = maxtime / Level_Appearance_Speed[i];
            }
        }

        u_TimeSlider.maxValue = f_maxtime;
        f_time = thistime * f_maxtime;
        StopCoroutine(LevelUp());
        yield return null;
    }

    
    public void Item(float Object_Special_Time, int Object_Special_Type)//���ӿ��� �г� ������Ʈ
    {
        f_ItemTime = Object_Special_Time;

        if (Object_Special_Type == 1)
        {
            isScoreItem = true;
            s_Unity.TouchSFX(8);
            ItemPanel.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 0.45f, 1f);
        }
        else if (Object_Special_Type == 2)
        {
            isStopItem = true;
            s_Unity.TouchSFX(7);
            ItemPanel.GetComponent<SpriteRenderer>().color = new Color(0.45f, 1f, 1f, 1f);
        }
    }
}
