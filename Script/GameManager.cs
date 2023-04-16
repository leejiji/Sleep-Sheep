using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<GameObject> ObjectList = new List<GameObject>(); //스폰된 오브젝트 관리 리스트

    [HideInInspector]
    public List<Dictionary<string, object>> GameData; //csv파일 불러옴
    [HideInInspector]
    public List<Dictionary<string, object>> ItemData; //csv파일 불러옴
    [HideInInspector]
    public List<Dictionary<string, object>> LevelData; //csv파일 불러옴

    //...UI 관련...//
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

    //...시스템 관련...//
    public int i_nowScore = 0; //현재 플레이 스코어
    public int i_nowSheep = 0; //현재 플레이 잡은 양 수
    public int i_highScore = 0; //최대 스코어
    public int i_highSheep = 0 ; //최대 잡은 양 수

    public int i_Object = 0; //현재 나와있는 오브젝트 수

    public bool isGameover = false; //게임오버 되었는지
    public bool isGameStart = false; //게임스타트 되었는지

    public bool isScoreItem = false; //스코어 아이템
    public bool isStopItem = false; //시간정지 아이템
    public float f_ItemTime = 0; //아이템 시간

    S_Unity s_Unity;
    
    //...레벨 관련...//
    public int i_Level = 1; //레벨
    public float f_maxtime = 20f; //최대 제한 시간
    public float f_time = 20f; //실시간 시간
    public List<float> Level_Time, Level_Appearance_Speed;
    float f_gametime = 0f; //누적 게임 시간 (레벨업하면 0으로 초기화)
    int i_Chagne = 1; //밤낮 바뀐 횟수

    //...스폰 관련...//
    public float f_objectSpawn = 0.8f; //오브젝트 등장 최대 시간



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
        //...시스템 업데이트 관련...//
        if (!isGameover && isGameStart)
        {
            if(!isStopItem)
                f_time -= Time.deltaTime;
            f_gametime += Time.deltaTime;
        } //게임 진행 중에 타이머 작동 및 누적 시간 플러스

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

        //...아이템 업데이트 관련...//
        if (f_ItemTime > 0) {
            if (isStopItem || isScoreItem)
                f_ItemTime -= Time.deltaTime;
        }
        else {
            isScoreItem = false;
            isStopItem = false;
            ItemPanel.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
        }

        //...UI 업데이트 관련...//
        u_TimeSlider.value = f_time; //게임시간
        u_NowScoreText.text = i_nowScore.ToString();
        u_NowSheepText.text = "X" + i_nowSheep.ToString();

        //...안드로이드 뒤로가기...//
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

    public void StopGame()//게임 멈춤, 옵션버튼
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
    public void QuitGame()//게임 종료
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
    IEnumerator GameOver()//게임오버 패널 업데이트
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
    IEnumerator LevelUp()//레벨업 시 시간 업데이트
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

    
    public void Item(float Object_Special_Time, int Object_Special_Type)//게임오버 패널 업데이트
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
