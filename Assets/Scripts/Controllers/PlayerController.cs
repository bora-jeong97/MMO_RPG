using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : BaseController
{

	int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);

	PlayerStat _stat;
	bool _stopSkill = false;


    public override void Init()
    {
		WorldObjectType = Define.WorldObject.Player;
		_stat = gameObject.GetComponent<PlayerStat>();

		// 마우스
		Managers.Input.MouseAction -= OnMouseEvent;
		Managers.Input.MouseAction += OnMouseEvent;
		// 있는지 체크 후 기존에 없는 경우만 UI 체력바 오브젝트 및 컴포넌트 넣어주기
		if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
			Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform); // transform this.플레이어에게 붙여서 만듬. 
		// 키보드
		//Managers.Input.KeyAction -= OnKeyboard; // 혹시라도 실수로 keyAction에 두번 넣었을 경우가 있을 시 1번만 호출하기 위함.
		//Managers.Input.KeyAction += OnKeyboard;
	}


	// 움직임 
	protected override void UpdateMoving()
	{
		// 몬스터가 내 사정거리보다 가까우면 공격
		if (_lockTarget != null)
		{
			_destPos = _lockTarget.transform.position;
			float distance = (_destPos - transform.position).magnitude;	// 플레이어와 타겟의 거리
			if (distance <= 1)	// 1보다 작을 때 스킬을 작동
			{
				State = Define.State.Skill;
				return;
			}
		}

		// 마우스 클릭시 player 이동 구현
		Vector3 dir = _destPos - transform.position; // 방향
		dir.y = 0;	// y좌표는 이동하지 않도록 막음.

		if (dir.magnitude < 0.1f)
		{
			State = Define.State.Idle;
		}
		else
		{
			#region Legacy
			// 길찾기 컴포넌트 
/*			NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
			float moveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude); // 방향(거리는 1로변환) * 속도 * 시간
			nma.Move(dir.normalized * moveDist);    // 길찾기 컴포넌트에 방향 백터를 넣어준다.*/
			#endregion
			Debug.DrawRay(transform.position + Vector3.up * 0.5f, dir.normalized, Color.green);
			// Block에 닿으면 멈추는 코드
			if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, LayerMask.GetMask("Block")))	// position이 발바닥 위치라 Vector3.up으로 약간 올려준다. 
			{
				if (Input.GetMouseButton(0) == false)	// 마우스 누르고 있는 중이라면 멈추지 않도록 함. 
					State = Define.State.Idle;
				return;
			}

			float moveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude); // 방향(거리는 1로변환) * 속도 * 시간
			transform.position += dir.normalized * moveDist;	// dir.normalized : 이동 방향 / moveDist : 이동거리
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
			transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);	// 부드럽게 회전
		}
	}

	void OnHitEvent()	// 스킬 작동 후 이벤트 Inspector > Event 0.67초 Function으로 지정함.
	{

		if(_lockTarget != null)
        {
			// TODO
			// 타겟의 Stat과 내 Stat을 비교해서 공격시 체력바를 조절하는 코드
			Stat targetStat = _lockTarget.GetComponent<Stat>();
			targetStat.OnAttacked(_stat);
		}

		// TODO
		if (_stopSkill)
		{
			State = Define.State.Idle;
		}
		else
		{
			State = Define.State.Skill;
		}
	}

	void OnMouseEvent(Define.MouseEvent evt)
	{
		switch (State)
		{
			case Define.State.Idle:
				OnMouseEvent_IdleRun(evt);
				break;
			case Define.State.Moving:
				OnMouseEvent_IdleRun(evt);
				break;
			case Define.State.Skill:
				{
					if (evt == Define.MouseEvent.PointerUp)
						_stopSkill = true;
				}
				break;
		}
	}

	void OnMouseEvent_IdleRun(Define.MouseEvent evt)
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		bool raycastHit = Physics.Raycast(ray, out hit, 100.0f, _mask);
		//Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f); // 디버깅 선 표시 1초 동안 유지

		switch (evt)
		{
			case Define.MouseEvent.PointerDown:	// 마우스를 처음으로 눌렀을 때 몬스터를 선택했는지, 땅을 선택했는지
				{
					if (raycastHit)	// 타겟이 있는경우
					{
						// 목적지로 이동하는 로직
						_destPos = hit.point;
						State = Define.State.Moving;
						_stopSkill = false;

						if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)	
							_lockTarget = hit.collider.gameObject; // 몬스터를 누른경우 lockTarget을 몬스터로
						else
							_lockTarget = null;
					}
				}
				break;
			case Define.MouseEvent.Press:	// 마우스로 누르고 있을 때
				{
					if (_lockTarget != null)
						_destPos = _lockTarget.transform.position; // 그 타겟에게 접근
					else if (_lockTarget == null && raycastHit) // 타겟이 없는 경우
                    {
						if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
							_destPos = hit.collider.transform.position;	// 몬스터 위로 콜라이더를 타지 않게 오류 해결   
						else
							_destPos = hit.point;	// 그 곳으로 접근
                    }
				}
				break;
			case Define.MouseEvent.PointerUp:
				_stopSkill = true;
				break;
		}
	}

	// 키보드 누를시 플레이어 이동
/*	void OnKeyboard()   
	{
		if (Input.GetKey(KeyCode.W))
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward), 0.2f); // a와 b사이를 0~1만큼 부드럽게 움직임
			transform.position += Vector3.forward * Time.deltaTime * _speed;
			//transform.Translate(Vector3.forward * Time.deltaTime * _speed);
		}
		if (Input.GetKey(KeyCode.S))
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.back), 0.2f);
			transform.position += Vector3.back * Time.deltaTime * _speed;
			//transform.Translate(Vector3.forward * Time.deltaTime * _speed);
		}
		if (Input.GetKey(KeyCode.A))
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.left), 0.2f);
			transform.position += Vector3.left * Time.deltaTime * _speed;
			//transform.Translate(Vector3.forward * Time.deltaTime * _speed);
		}
		if (Input.GetKey(KeyCode.D))
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right), 0.2f);
			transform.position += Vector3.right * Time.deltaTime * _speed;
			//transform.Translate(Vector3.forward * Time.deltaTime * _speed);
		}

		//_moveToDest = false;
	}
*/



}
