using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_CameraShake : MonoBehaviour
{
    public float ShakeAmount;
    float ShakeTime;
    Vector3 initialPosition;

    public void VibrateForTime(float time)
    {
        ShakeTime = time;
    }

    void Start()
    {
        initialPosition = new Vector3(0f, 0f, -5f);
    }
    void Update()
    {
        if(ShakeTime > 0)
        {
            transform.position = Random.insideUnitSphere * ShakeAmount + initialPosition;
            ShakeTime -= Time.deltaTime;
        }
        else
        {
            ShakeTime = 0.0f;
            transform.position = initialPosition;
        }
    }
}
