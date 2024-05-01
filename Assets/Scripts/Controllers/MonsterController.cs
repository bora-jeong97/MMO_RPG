using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : BaseController
{

	Stat _stat;
	[SerializeField]
	float _scanRange = 10;  // 플레이어 인지 범위

	[SerializeField]
	float _attackRange = 2;	// 공격 사정거리


	public override void Init()
	{
		WorldObjectType = Define.WorldObject.Monster;
		_stat = gameObject.GetComponent<Stat>();

		// 있는지 체크 후 기존에 없는 경우만 UI 체력바 오브젝트 및 컴포넌트 넣어주기
		if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
			Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform); // transform this.플레이어에게 붙여서 만듬. 
	}

    protected override void UpdateIdle()
    {
		
		// Player 태그를 단 게임 객체가 사정거리 안에 들어오면 상태를 Idle > Moving으로 변경(추적)
		GameObject player = Managers.Game.GetPlayer();
		if (player == null)
			return;

		float distance = (player.transform.position - transform.position).magnitude;
		if(distance <= _scanRange)
        {
			_lockTarget = player;
			
			// 플레이어 체력이 0이면 멈춤.
			Stat targetStat = _lockTarget.GetComponent<Stat>();
			if (targetStat.Hp <= 0)
				return;

			// 그렇지 않으면 이동
			State = Define.State.Moving;
			return;
        }
    }


	// 몬스터 AI 스캔범위 내 추적(_scanRange), 사정거리 내 공격(_attackRange)
	protected override void UpdateMoving()
    {
		// 플레이어가 내 사정거리보다 가까우면 공격
		if (_lockTarget != null)
		{
			_destPos = _lockTarget.transform.position;
			float distance = (_destPos - transform.position).magnitude; // 플레이어와 타겟의 거리
			if (distance <= _attackRange)  // 타겟과의 거리가 사정거리보다 작으면 발동
			{
				// 길찾기 컴포넌트 
				NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
				nma.SetDestination(transform.position);   // 해당 포지션으로 이동하는 길찾기 AI - 현재 위치로 가게 해두었으니 공격 사정거리인경우 더이상 이동안함.
				State = Define.State.Skill;
				return;
			}
		}

		// 플레이어가 내 사정거리보다 가깝지 않아서 추적
		// 이동 
		Vector3 dir = _destPos - transform.position; // 방향
		if (dir.magnitude < 0.1f)	// 최대한 가까워지면 달리지 않고 멈춘다. 
		{
			State = Define.State.Idle;
		}
		else
		{
			// 길찾기 컴포넌트 
			NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
			nma.SetDestination(_destPos);	// 해당 포지션으로 이동하는 길찾기 AI
			nma.speed = _stat.MoveSpeed;

			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
		}
	}

	// 타겟에게 공격할때 타겟을 바라보도록 방향 지정
	protected override void UpdateSkill()
    {
		if (_lockTarget != null)
		{
			// 방향 
			Vector3 dir = _lockTarget.transform.position - transform.position;
			Quaternion quat = Quaternion.LookRotation(dir);
			transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);    // 부드럽게 회전
		}
	}


	// 한 번 공격후 계속 공격할지 멈출지 정한다
	void OnHitEvent()   
	{
		if (_lockTarget != null)
		{
			// 타겟의 Stat과 내 Stat을 비교해서 공격시 체력바를 조절하는 코드
			Stat targetStat = _lockTarget.GetComponent<Stat>();
			targetStat.OnAttacked(_stat);

			if (targetStat.Hp > 0)
            {
				float distance = (_lockTarget.transform.position - transform.position).magnitude;
				if (distance <= _attackRange) // 공격 사정거리 내면 다시 공격
					State = Define.State.Skill;
				else // 공격 사정거리 외면 추적
					State = Define.State.Moving;	
            }
            else
            {
				GameObject.Destroy(targetStat.gameObject); // 타겟을 없앤다.
				State = Define.State.Idle;	// 타겟이 죽은경우 멈춤
			}
		}
		else
		{
			State = Define.State.Idle;
		}

	}

}
