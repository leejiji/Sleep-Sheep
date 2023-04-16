using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Spawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] sheepPrefab;
    [SerializeField]
    private GameObject[] wolfPrefab;
    [SerializeField]
    private GameObject[] itemPrefab;

    float f_RandX, f_RandY, f_Timer = 0;
    Vector2 RandomPos;


    void Update()
    {
        if(GameManager.instance.i_Object < 6)
            f_Timer -= Time.deltaTime;
        
        if(f_Timer <= 0 && GameManager.instance.i_Object < 6 && GameManager.instance.isGameover == false && GameManager.instance.isGameStart == true)
            Spawn();
    }
    void Spawn()
    {
        f_Timer = Random.Range(0f, GameManager.instance.f_objectSpawn);
        f_RandX = Random.Range(-2.16f, 2.16f);
        f_RandY = Random.Range(-2.5f, 2f);

        if(GameManager.instance.i_Object > 0)
        {
            for (int i = 0; i < GameManager.instance.ObjectList.Count; i++)
            {
                if (Mathf.Abs(f_RandX - GameManager.instance.ObjectList[i].GetComponent<Transform>().position.x) < 0.5f)
                {
                    f_RandX = (f_RandX > 0f) ? f_RandX - 1f : f_RandX + 1f;
                }
                if (Mathf.Abs(f_RandY - GameManager.instance.ObjectList[i].GetComponent<Transform>().position.y) < 0.5f)
                {
                    f_RandY = (f_RandY > 0f) ? f_RandY - 1f : f_RandY + 1f;
                }
            }
        }
        

        RandomPos = new Vector2(f_RandX, f_RandY);
        
        int flipRand = Random.Range(0,2);

        if (Random.Range(1, 100) <= 75) //양 스폰
        {
            float rand = Random.Range(0, 100);
            int i = 0;

            if (rand <= 69.25)
                i = 0;
            else if (rand > 69.25 && rand <= 89)
                i = 1;
            else
                i = 2;

            GameObject _obj = Instantiate(sheepPrefab[i], RandomPos, Quaternion.identity) as GameObject;

            if (flipRand == 0)
                _obj.GetComponent<SpriteRenderer>().flipX = true;
            else
                _obj.GetComponent<SpriteRenderer>().flipX = false;

            GameManager.instance.ObjectList.Add(_obj);
            GameManager.instance.i_Object++;

            ObjectLayer(_obj);
        }
        else if(Random.Range(1, 100) > 75 && Random.Range(1, 100) <= 95) //늑대 스폰
        {
            float rand = Random.Range(0, 100);
            int i = 0;
            if (rand <= 70)
                i = Random.Range(0, 2);
            else
                i = Random.Range(2, 5);

            GameObject _obj = Instantiate(wolfPrefab[i], RandomPos, Quaternion.identity) as GameObject;

            if (flipRand == 0)
                _obj.GetComponent<SpriteRenderer>().flipX = true;
            else
                _obj.GetComponent<SpriteRenderer>().flipX = false;

            GameManager.instance.ObjectList.Add(_obj);

            GameManager.instance.i_Object++;
            ObjectLayer(_obj);
        }
        else if(Random.Range(1, 100) > 95 && GameManager.instance.isStopItem == false && GameManager.instance.isScoreItem == false
            && GameManager.instance.i_Level >= 4)
        {
            GameObject _obj = Instantiate(itemPrefab[Random.Range(0, 2)], RandomPos, Quaternion.identity) as GameObject;
            
            GameManager.instance.ObjectList.Add(_obj);

            GameManager.instance.i_Object++;
            ObjectLayer(_obj);
        }


    }

    void ObjectLayer(GameObject _obj)
    {
        for (int i = 0; i < GameManager.instance.ObjectList.Count; i++)
        {
            if (GameManager.instance.ObjectList[i].gameObject == _obj)
                _obj.GetComponent<SpriteRenderer>().sortingOrder += i;
        }
    }
}
