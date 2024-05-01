using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HPBar : UI_Base
{

    enum GameObjects
    {
        HPBar
    }

    Stat _stat;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        _stat = transform.parent.GetComponent<Stat>();  // 해당 부모 객체의 Stat을 뽑아온다. 
    }


    private void Update()
    {
        try
        {
            // 부모 transform을 추적
            Transform parent = transform.parent;    // this.parent같은 문법. 
                                                    // 체력 바 위치를 부모 객체의 머리 살짝 위로 자연스럽게 위치해준다.  
                                                    //transform.position = parent.position;
            transform.position = parent.position + Vector3.up * (parent.GetComponent<Collider>().bounds.size.y); // 발끝에서 부터 콜라이더 만큼을 위로 올려준다.
            transform.rotation = Camera.main.transform.rotation;    // 체력바가 플레이어와 같이 회전하지 않고 카메라와 바라보는 방향이 같도록 함.
                                                                    //transform.LookAt(Camera.main.transform); 
            float ratio = _stat.Hp / (float)_stat.MaxHp; // 현재 체력과 최대 체력간의 비율
            SetHpRatio(ratio);
        } catch(Exception e)
        {
            Debug.LogError("예외 발생" + e.Message);
            Debug.LogError("스택 트레이스" + e.StackTrace);
            
        }


    }


    // 체력바 게이지 조절 
    public void SetHpRatio(float ratio)
    {
        GetObject((int)GameObjects.HPBar).GetComponent<Slider>().value = ratio;
    }

}
