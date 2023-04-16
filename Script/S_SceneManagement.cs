using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class S_SceneManagement : MonoBehaviour
{
    S_Unity s_Unity;
    public bool P_SceneChage;
    public bool M_SceneChage;
    public void Awake()
    {
        s_Unity = GameObject.Find("UnityManager").GetComponent<S_Unity>();

        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            s_Unity.BGMPlay(0);
            s_Unity.UpdateSlider();
            s_Unity.Paid();

            s_Unity.BtnSet();
        }

    }
    public void Play()//시작 버튼
    {
        s_Unity.TouchSFX(6);
        s_Unity.Paid();
        P_SceneChage = true;
    }
    public void Main()//메인으로 버튼
    {
        s_Unity.TouchSFX(6);
        s_Unity.Paid();
        M_SceneChage = true;
    }

}
