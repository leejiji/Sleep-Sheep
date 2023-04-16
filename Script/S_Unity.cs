using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class S_Unity : MonoBehaviour
{
    public static S_Unity instance;

    public AudioMixer masterMixer;
    public AudioSource SFXSource;
    public AudioSource BGMSource;
    public AudioClip[] Ad_SFXCliip;
    public AudioClip[] Ad_BGMCliip;

    [HideInInspector]
    public Slider[] audioSlider;
    public bool PaidIn = true;

    [HideInInspector]
    public List<float> soundVolume;

    [HideInInspector]
    public bool isPaidOver = false;

    [HideInInspector]
    public Button[] OnOffBtn;

    public bool isVibe;
    public bool isCameraShake;

    public Sprite[] OnOffSprite;

    public void Awake()
    {
        instance = this;

        for (int i = 0; i < 3; i++)
            soundVolume.Add(0);
    }

    public void UpdateSlider()
    {
        OnOffBtn = new Button[2];

        audioSlider[0] = GameObject.Find("Canvas").transform.Find("UI_StopPanel").transform.Find("UI_MasterSlider").GetComponent<Slider>();
        audioSlider[1] = GameObject.Find("Canvas").transform.Find("UI_StopPanel").transform.Find("UI_BGMSlider").GetComponent<Slider>();
        audioSlider[2] = GameObject.Find("Canvas").transform.Find("UI_StopPanel").transform.Find("UI_SFXSlider").GetComponent<Slider>();

        audioSlider[0].onValueChanged.AddListener(MasterControl);
        audioSlider[1].onValueChanged.AddListener(BGMControl);
        audioSlider[2].onValueChanged.AddListener(SFXControl);

        OnOffBtn[0] = GameObject.Find("Canvas").transform.Find("UI_StopPanel").transform.Find("UI_CameraOnBtn").GetComponent<Button>();
        OnOffBtn[1] = GameObject.Find("Canvas").transform.Find("UI_StopPanel").transform.Find("UI_VibeOnBtn").GetComponent<Button>();

        OnOffBtn[0].onClick.AddListener(CameraShakeBtn);
        OnOffBtn[1].onClick.AddListener(VibeBtn);

        for (int i = 0; i < soundVolume.Count; i++)
            audioSlider[i].value = soundVolume[i];
    }
    public void ToggleAudioVolume()
    {
        AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
    }
    public void TouchSFX(int type)
    {
        SFXSource.PlayOneShot(Ad_SFXCliip[type]);
    }
    public void BGMPlay(int type)
    {
        BGMSource.clip = Ad_BGMCliip[type];
        BGMSource.Play();
    }
    
    public void Paid()
    {
        StartCoroutine(PaidInOut());
    }
    IEnumerator PaidInOut()
    {
        S_SceneManagement S_SceneManager = GameObject.Find("Main Camera").GetComponent<S_SceneManagement>();
        string str = "In";

        if (PaidIn)
            str = "In";
        else if (!PaidIn)
            str = "Out";

        GameObject.Find("Canvas").transform.Find("Paid").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("Paid").GetComponent<Animator>().Play("A_Paid" + str);
        yield return new WaitForSeconds(1.0f);
        PaidIn = !PaidIn;

        if (S_SceneManager.P_SceneChage)
            SceneManager.LoadScene("GameScene");
        else if(S_SceneManager.M_SceneChage)
            SceneManager.LoadScene("MainScene");
        else
            GameObject.Find("Canvas").transform.Find("Paid").gameObject.SetActive(false);

        yield return null;  

    }

    public void MasterControl(float sound)
    {
        sound = audioSlider[0].value;
        if (sound == -40f) masterMixer.SetFloat("Master", -80);
        else masterMixer.SetFloat("Master", sound);

        soundVolume[0] = sound;
    }
    public void BGMControl(float sound)
    {
        sound = audioSlider[1].value;
        if (sound == -40f) masterMixer.SetFloat("BGM", -80);
        else masterMixer.SetFloat("BGM", sound);

        soundVolume[1] = sound;
    }
    public void SFXControl(float sound)
    {
        sound = audioSlider[2].value;
        if (sound == -40f) masterMixer.SetFloat("SFX", -80);
        else masterMixer.SetFloat("SFX", sound);

        soundVolume[2] = sound;
    }

    public void CameraShakeBtn()//세팅, 카메라 셰이크
    {
        TouchSFX(6);
        isCameraShake = !isCameraShake;

        if (isCameraShake)
            EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite = OnOffSprite[0];
        else
            EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite = OnOffSprite[1];
    }
    public void VibeBtn()//세팅, 휴대폰 진동
    {
        TouchSFX(6);
        isVibe = !isVibe;

        if (isVibe)
            EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite = OnOffSprite[0];
        else
            EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite = OnOffSprite[1];
    }

    public void BtnSet()
    {
        if (isVibe)
            GameObject.Find("Canvas").transform.Find("UI_StopPanel").transform.Find("UI_VibeOnBtn").gameObject.GetComponent<Image>().sprite = OnOffSprite[0];
        else
            GameObject.Find("Canvas").transform.Find("UI_StopPanel").transform.Find("UI_VibeOnBtn").gameObject.GetComponent<Image>().sprite = OnOffSprite[1];

        if (isCameraShake)
            GameObject.Find("Canvas").transform.Find("UI_StopPanel").transform.Find("UI_CameraOnBtn").gameObject.GetComponent<Image>().sprite = OnOffSprite[0];
        else
            GameObject.Find("Canvas").transform.Find("UI_StopPanel").transform.Find("UI_CameraOnBtn").gameObject.GetComponent<Image>().sprite = OnOffSprite[1];
    }
}
