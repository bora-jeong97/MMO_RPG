using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
	[SerializeField]
	protected Vector3 _destPos;
	[SerializeField]
	protected Define.State _state = Define.State.Idle;
	[SerializeField]
	protected GameObject _lockTarget; // 상대 타겟을 저장

	// GameManager에서 쓰기 위함.
	public Define.WorldObject WorldObjectType { get; protected set; } = Define.WorldObject.Unknown;

	// 에니메이션 상태 프로퍼티
	public virtual Define.State State
	{
		get { return _state; }
		set
		{
			_state = value;

			Animator anim = GetComponent<Animator>();
			switch (_state) // Animator 변수
			{
				case Define.State.Die:
					break;
				case Define.State.Idle: // 멈춤
										// anim.Play("WAIT"); // 약간씩 끊기는 느낌
					anim.CrossFade("WAIT", 0.1f);   // 부드럽게 무빙
					break;
				case Define.State.Moving:
					anim.CrossFade("RUN", 0.1f);
					break;
				case Define.State.Skill:
					anim.CrossFade("ATTACK", 0.1f, -1, 0); // 맨 끝에 0을 넣어서 Loop를 틀어놓은 것처럼 처음부터 계속 반복 재생 가능하다.public void CrossFade(string stateName, float normalizedTransitionDuration, int layer, float normalizedTimeOffset);
					break;
			}
		}
	}


	protected void Start()
    {
		Init();
    }



	void Update()
	{
		switch (State)
		{
			case Define.State.Die:
				UpdateDie();
				break;
			case Define.State.Moving:
				UpdateMoving();
				break;
			case Define.State.Idle:
				UpdateIdle();
				break;
			case Define.State.Skill:
				UpdateSkill();
				break;
		}
	}



	public abstract void Init();
	protected virtual void UpdateDie() { }
	protected virtual void UpdateMoving() { }
	protected virtual void UpdateIdle() { }
	protected virtual void UpdateSkill() { }
}
