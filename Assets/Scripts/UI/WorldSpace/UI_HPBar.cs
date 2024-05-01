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
        _stat = transform.parent.GetComponent<Stat>();  // �ش� �θ� ��ü�� Stat�� �̾ƿ´�. 
    }


    private void Update()
    {
        try
        {
            // �θ� transform�� ����
            Transform parent = transform.parent;    // this.parent���� ����. 
                                                    // ü�� �� ��ġ�� �θ� ��ü�� �Ӹ� ��¦ ���� �ڿ������� ��ġ���ش�.  
                                                    //transform.position = parent.position;
            transform.position = parent.position + Vector3.up * (parent.GetComponent<Collider>().bounds.size.y); // �߳����� ���� �ݶ��̴� ��ŭ�� ���� �÷��ش�.
            transform.rotation = Camera.main.transform.rotation;    // ü�¹ٰ� �÷��̾�� ���� ȸ������ �ʰ� ī�޶�� �ٶ󺸴� ������ ������ ��.
                                                                    //transform.LookAt(Camera.main.transform); 
            float ratio = _stat.Hp / (float)_stat.MaxHp; // ���� ü�°� �ִ� ü�°��� ����
            SetHpRatio(ratio);
        } catch(Exception e)
        {
            Debug.LogError("���� �߻�" + e.Message);
            Debug.LogError("���� Ʈ���̽�" + e.StackTrace);
            
        }


    }


    // ü�¹� ������ ���� 
    public void SetHpRatio(float ratio)
    {
        GetObject((int)GameObjects.HPBar).GetComponent<Slider>().value = ratio;
    }

}
