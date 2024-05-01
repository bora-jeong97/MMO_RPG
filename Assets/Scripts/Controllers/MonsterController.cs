using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : BaseController
{

	Stat _stat;
	[SerializeField]
	float _scanRange = 10;  // �÷��̾� ���� ����

	[SerializeField]
	float _attackRange = 2;	// ���� �����Ÿ�


	public override void Init()
	{
		WorldObjectType = Define.WorldObject.Monster;
		_stat = gameObject.GetComponent<Stat>();

		// �ִ��� üũ �� ������ ���� ��츸 UI ü�¹� ������Ʈ �� ������Ʈ �־��ֱ�
		if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
			Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform); // transform this.�÷��̾�� �ٿ��� ����. 
	}

    protected override void UpdateIdle()
    {
		
		// Player �±׸� �� ���� ��ü�� �����Ÿ� �ȿ� ������ ���¸� Idle > Moving���� ����(����)
		GameObject player = Managers.Game.GetPlayer();
		if (player == null)
			return;

		float distance = (player.transform.position - transform.position).magnitude;
		if(distance <= _scanRange)
        {
			_lockTarget = player;
			
			// �÷��̾� ü���� 0�̸� ����.
			Stat targetStat = _lockTarget.GetComponent<Stat>();
			if (targetStat.Hp <= 0)
				return;

			// �׷��� ������ �̵�
			State = Define.State.Moving;
			return;
        }
    }


	// ���� AI ��ĵ���� �� ����(_scanRange), �����Ÿ� �� ����(_attackRange)
	protected override void UpdateMoving()
    {
		// �÷��̾ �� �����Ÿ����� ������ ����
		if (_lockTarget != null)
		{
			_destPos = _lockTarget.transform.position;
			float distance = (_destPos - transform.position).magnitude; // �÷��̾�� Ÿ���� �Ÿ�
			if (distance <= _attackRange)  // Ÿ�ٰ��� �Ÿ��� �����Ÿ����� ������ �ߵ�
			{
				// ��ã�� ������Ʈ 
				NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
				nma.SetDestination(transform.position);   // �ش� ���������� �̵��ϴ� ��ã�� AI - ���� ��ġ�� ���� �صξ����� ���� �����Ÿ��ΰ�� ���̻� �̵�����.
				State = Define.State.Skill;
				return;
			}
		}

		// �÷��̾ �� �����Ÿ����� ������ �ʾƼ� ����
		// �̵� 
		Vector3 dir = _destPos - transform.position; // ����
		if (dir.magnitude < 0.1f)	// �ִ��� ��������� �޸��� �ʰ� �����. 
		{
			State = Define.State.Idle;
		}
		else
		{
			// ��ã�� ������Ʈ 
			NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
			nma.SetDestination(_destPos);	// �ش� ���������� �̵��ϴ� ��ã�� AI
			nma.speed = _stat.MoveSpeed;

			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
		}
	}

	// Ÿ�ٿ��� �����Ҷ� Ÿ���� �ٶ󺸵��� ���� ����
	protected override void UpdateSkill()
    {
		if (_lockTarget != null)
		{
			// ���� 
			Vector3 dir = _lockTarget.transform.position - transform.position;
			Quaternion quat = Quaternion.LookRotation(dir);
			transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);    // �ε巴�� ȸ��
		}
	}


	// �� �� ������ ��� �������� ������ ���Ѵ�
	void OnHitEvent()   
	{
		if (_lockTarget != null)
		{
			// Ÿ���� Stat�� �� Stat�� ���ؼ� ���ݽ� ü�¹ٸ� �����ϴ� �ڵ�
			Stat targetStat = _lockTarget.GetComponent<Stat>();
			targetStat.OnAttacked(_stat);

			if (targetStat.Hp > 0)
            {
				float distance = (_lockTarget.transform.position - transform.position).magnitude;
				if (distance <= _attackRange) // ���� �����Ÿ� ���� �ٽ� ����
					State = Define.State.Skill;
				else // ���� �����Ÿ� �ܸ� ����
					State = Define.State.Moving;	
            }
            else
            {
				GameObject.Destroy(targetStat.gameObject); // Ÿ���� ���ش�.
				State = Define.State.Idle;	// Ÿ���� ������� ����
			}
		}
		else
		{
			State = Define.State.Idle;
		}

	}

}
