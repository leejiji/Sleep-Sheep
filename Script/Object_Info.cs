using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Info : MonoBehaviour
{
    public int Object_Code, Object_Score, Object_Score_Lost, Object_Special_Code, Object_Special_Type = 0;
    public float Object_Time_Recovery, Object_Time_Reduce, Object_Special_Time = 0;
    
    float f_Timer;

    private void Start()
    {
        for(int i = 0; i < GameManager.instance.GameData.Count; i++)
        {
            if(GameManager.instance.GameData[i]["Object_Code"].ToString() == Object_Code.ToString())
            {
                Object_Score = int.Parse(GameManager.instance.GameData[i]["Object_Score"].ToString());
                Object_Score_Lost = int.Parse(GameManager.instance.GameData[i]["Object_Score_Lost"].ToString());
                Object_Time_Recovery = float.Parse(GameManager.instance.GameData[i]["Object_Time_Recovery"].ToString()) / 100;
                Object_Time_Reduce = float.Parse(GameManager.instance.GameData[i]["Object_Time_Reduce"].ToString()) / 100;
                Object_Special_Code = int.Parse(GameManager.instance.GameData[i]["Object_Special_Code"].ToString());
                f_Timer = float.Parse(GameManager.instance.GameData[i]["Object_Appearance_Time"].ToString()) / 10;
            }
        }
        for(int i = 0; i < GameManager.instance.ItemData.Count; i++)
        {
            if (Object_Special_Code.ToString() == GameManager.instance.ItemData[i]["Object_Special_Code"].ToString())
            {
                Object_Special_Time = float.Parse(GameManager.instance.ItemData[i]["Object_Special_Time"].ToString()) / 10;
                Object_Special_Type = int.Parse(GameManager.instance.ItemData[i]["Object_Special_Type"].ToString());
            }
        }
    }
    void Update()
    {
        if(this.gameObject.tag == "Sheep" || this.gameObject.tag == "Wolf" || this.gameObject.tag == "Item_Score" || this.gameObject.tag == "Item_Stop")
        {
            f_Timer -= Time.deltaTime;

            if (f_Timer <= 0)
            {
                for (int i = 0; i < GameManager.instance.ObjectList.Count; i++)
                {
                    if (GameManager.instance.ObjectList[i].gameObject == this.gameObject)
                    {
                        Destroy(this.gameObject);
                        GameManager.instance.ObjectList.RemoveAt(i);
                        GameManager.instance.i_Object--;
                    }
                }
            }
        }
        
    }
}
