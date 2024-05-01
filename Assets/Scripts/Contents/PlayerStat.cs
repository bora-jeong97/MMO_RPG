using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : Stat
{
    [SerializeField]
	protected int _exp;
    [SerializeField]
	protected int _gold;

	public int Exp { get { return _exp; } 
		set {
			_exp = value;
			// 레벨업 체크

			int level = Level;
			// 다음 레벨이 없을 때까지 레벨업. 
			while (true)
            {
				Data.Stat stat;
				// 다음 레벨에 해당되는 정보가 없으면 끝냄.(최대 레벨 도달)
				if (Managers.Data.StatDict.TryGetValue(level + 1, out stat) == false)	
					break;
				// 다음 레벨업보다 현재 경험치가 못하면 레벨업 안하고 끝냄.
				if (_exp < stat.totalExp)	
					break;
				// 레벨업
				level++;
            }

			// 기존 레벨과 달라진 경우 새로 세팅
			if(level != Level)
            {
				Debug.Log("Level Up!");
				Debug.Log($"기존 레벨 : {Level}");
				Level = level;
				SetStat(Level);
				Debug.Log($"현재 레벨 : {Level}");
            }
		} }
	public int Gold { get { return _gold; } set { _gold = value; } }

	private void Start()
	{
		_level = 1;
		_exp = 0;
		_defense = 5;
		_moveSpeed = 5.0f;
		_gold = 0;

		SetStat(_level);
	}

	// 레벨업시 해당 레벨로 스텟 초기화
	public void SetStat(int level)
    {
		Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
		Data.Stat stat = dict[level];

		_hp = stat.maxHp;
		_maxHp = stat.maxHp;
		_attack = stat.attack;
	}

    protected override void OnDead(Stat attacker)
    {
		Debug.Log("Player Dead");
    }
}
