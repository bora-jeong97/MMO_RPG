using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    [SerializeField]
    protected int _level;
    [SerializeField]
    protected int _hp;
    [SerializeField]
    protected int _maxHp;
    [SerializeField]
    protected int _attack;
    [SerializeField]
    protected int _defense;
    [SerializeField]
    protected float _moveSpeed;

    public int Level { get { return _level; } set { _level = value; } }
    public int Hp { get { return _hp; } set { _hp = value; } }
    public int MaxHp { get { return _maxHp; } set { _maxHp = value; } }
    public int Attack { get { return _attack; } set { _attack = value; } }
    public int Defense { get { return _defense; } set { _defense = value; } }
    public float MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }

    private void Start()
    {
        _level = 1;
        _hp = 100;
        _maxHp = 100;
        _attack = 10;
        _defense = 5;
        _moveSpeed = 5.0f;
    }


    // attacker의 Stat과 해당 go의 Stat을 비교해서 공격시 체력바를 조절하는 코드
    public virtual void OnAttacked(Stat attacker)
    {
        // Stat targetStat = _lockTarget.GetComponent<Stat>();
        int damage = Mathf.Max(0, attacker.Attack - Defense);  // attacker의 공격과 go 디펜스 값과 0중 큰 값을 가져가서 데미지로 한다.
        Hp -= damage;
        if (Hp <= 0)
        {
            Hp = 0; // 음수가 되지 않게 0으로 처리
            OnDead(attacker);
        }

    }


    // 죽었을 때 처리
    // 디스폰과 레벨업
    protected virtual void OnDead(Stat attacker)
    {
        PlayerStat playerStat = attacker as PlayerStat;
        if(playerStat != null)
        {
            playerStat.Exp += 5;
        }

        Managers.Game.Despawn(gameObject);
    }
}
