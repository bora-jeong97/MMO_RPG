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
	protected GameObject _lockTarget; // ��� Ÿ���� ����

	// GameManager���� ���� ����.
	public Define.WorldObject WorldObjectType { get; protected set; } = Define.WorldObject.Unknown;

	// ���ϸ��̼� ���� ������Ƽ
	public virtual Define.State State
	{
		get { return _state; }
		set
		{
			_state = value;

			Animator anim = GetComponent<Animator>();
			switch (_state) // Animator ����
			{
				case Define.State.Die:
					break;
				case Define.State.Idle: // ����
										// anim.Play("WAIT"); // �ణ�� ����� ����
					anim.CrossFade("WAIT", 0.1f);   // �ε巴�� ����
					break;
				case Define.State.Moving:
					anim.CrossFade("RUN", 0.1f);
					break;
				case Define.State.Skill:
					anim.CrossFade("ATTACK", 0.1f, -1, 0); // �� ���� 0�� �־ Loop�� Ʋ����� ��ó�� ó������ ��� �ݺ� ��� �����ϴ�.public void CrossFade(string stateName, float normalizedTransitionDuration, int layer, float normalizedTimeOffset);
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
