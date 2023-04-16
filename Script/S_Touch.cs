using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class S_Touch : MonoBehaviour
{
    [SerializeField]
    private GameObject[] Effect;
    Vector3 MousePos;
    Camera Camera;
    S_CameraShake ShakeCamera;

    private void Start()
    {
        Camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        ShakeCamera = GameObject.Find("Main Camera").GetComponent<S_CameraShake>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && GameManager.instance.isGameover == false && GameManager.instance.isGameStart == true && Time.timeScale == 1)
        {
            S_Unity unityManager = GameObject.Find("UnityManager").GetComponent<S_Unity>();

            MousePos = Input.mousePosition;
            MousePos = Camera.ScreenToWorldPoint(MousePos);

            Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 touchPos = new Vector2(wp.x, wp.y);
            transform.position = touchPos;

            RaycastHit2D hit = Physics2D.Raycast(MousePos, transform.forward, 15f);

            long[] pattern = new long[4];
            for(int i = 0; i < 4; i++)
                pattern[i] = 100;

            if (hit.transform.gameObject.tag == "Sheep")
            {
                Debug.Log("양 터치");
                GameManager.instance.i_nowSheep += 1;
                DeleteObject(hit);

                GameObject instObj = Instantiate(Effect[0], transform.position, Quaternion.identity) as GameObject; //터치 이펙트
                unityManager.TouchSFX(1);
            }
            else if (hit.transform.gameObject.tag == "Wolf")
            {
                Debug.Log("늑대 터치");
                if (unityManager.isCameraShake)
                    ShakeCamera.VibrateForTime(0.07f);
                if (unityManager.isVibe)
                    S_Vibe.Vibrate(pattern, -1);
                DeleteObject(hit);
                GameObject instObj = Instantiate(Effect[0], transform.position, Quaternion.identity) as GameObject; //터치 이펙트
                unityManager.TouchSFX(2);
            }
            else if(hit.transform.gameObject.tag == "UI")
            {
                Debug.Log("UI 터치");
                unityManager.TouchSFX(0);
            }
            else if (hit.transform.gameObject.tag == "Item")
            {
                Debug.Log("아이템 터치");
                DeleteObject(hit);
                GameManager.instance.Item(hit.transform.gameObject.GetComponent<Object_Info>().Object_Special_Time,
                        hit.transform.gameObject.GetComponent<Object_Info>().Object_Special_Type);

                GameObject instObj = Instantiate(Effect[0], transform.position, Quaternion.identity) as GameObject; //터치 이펙트
                unityManager.TouchSFX(3);
            }
            else
            {
                Debug.Log("배경 터치");
                if (unityManager.isCameraShake)
                    ShakeCamera.VibrateForTime(0.07f);
                if (unityManager.isVibe)
                    S_Vibe.Vibrate(pattern, -1);

                GameManager.instance.i_nowScore -= 50;
                GameObject instObj = Instantiate(Effect[0], transform.position, Quaternion.identity) as GameObject; //터치 이펙트
                unityManager.TouchSFX(4);
            }
            
            if (hit.transform.gameObject.tag == "Sheep" || hit.transform.gameObject.tag == "Wolf") //읽기 오류 나서 여따 붙임, 배경에 오브젝트 추가되면 삭제
            {
                int i_plus_score = hit.transform.gameObject.GetComponent<Object_Info>().Object_Score;
                int i_sub_score = hit.transform.gameObject.GetComponent<Object_Info>().Object_Score_Lost;
                float i_plus_Time = hit.transform.gameObject.GetComponent<Object_Info>().Object_Time_Recovery;
                float i_sub_Time = hit.transform.gameObject.GetComponent<Object_Info>().Object_Time_Reduce;

                if(GameManager.instance.isScoreItem)
                    GameManager.instance.i_nowScore += i_plus_score;
                GameManager.instance.i_nowScore += i_plus_score;
                GameManager.instance.i_nowScore -= i_sub_score;
                GameManager.instance.f_time += i_plus_Time * (GameManager.instance.f_maxtime / 100);
                GameManager.instance.f_time -= i_sub_Time * (GameManager.instance.f_maxtime / 100);
            }            
        }
    }

    void DeleteObject(RaycastHit2D hit)
    {
        for (int i = 0; i < GameManager.instance.ObjectList.Count; i++)
        {
            if (GameManager.instance.ObjectList[i].gameObject == hit.transform.gameObject)
            {
                Destroy(hit.transform.gameObject);
                GameManager.instance.ObjectList.RemoveAt(i);
                GameManager.instance.i_Object--;
            }
        }
    }
}
