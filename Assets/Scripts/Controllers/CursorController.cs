using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
	// << 연산자는 비트 시프트(Shift) 연산자
	// 주어진 숫자를 왼쪽으로 주어진 비트 수만큼 이동시킵니다.
	// 1 을 이진수로 표현했을 때, 오른쪽에서 n번째 비트를 왼쪽으로 이동시킨 값을 생성합니다.
	// 이동된 비트는 그만큼의 2의 거듭제곱 값을 나타냅니다.
	// 256 | 512 = 768 따라서 _mask 변수에는 768이 저장됨. 
	int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);

	Texture2D _attackIcon;
	Texture2D _handIcon;

	enum CursorType
	{
		None,
		Attack,
		Hand,
	}

	CursorType _cursorType = CursorType.None;

	void Start()
    {
		_attackIcon = Managers.Resource.Load<Texture2D>("Textures/Cursor/Attack");
		_handIcon = Managers.Resource.Load<Texture2D>("Textures/Cursor/Hand");
	}


	// 커서 모양 변경 update
    void Update()
    {
		if (Input.GetMouseButton(0))	// 마우스를 이미 누르고 있는 상태라면 커서 모양 변경 없음.
			return;

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 100.0f, _mask))
		{
			
			if (hit.collider.gameObject.layer == (int)Define.Layer.Monster) // 공격 마우스 커서 (몬스터를 향할 때)
			{
				if (_cursorType != CursorType.Attack)	
				{
					Cursor.SetCursor(_attackIcon, new Vector2(_attackIcon.width / 5, 0), CursorMode.Auto);  //_attackIcon.width / 5는 해당 icon의 x좌표를 5분의 1정도 오른쪽으로 가기 위함. 0은 왼쪽 끝 좌상단을 의미한다.
					_cursorType = CursorType.Attack;
				}
			}
			else // 손 마우스 커서 (몬스터 외)
			{
				if (_cursorType != CursorType.Hand)		
				{
					Cursor.SetCursor(_handIcon, new Vector2(_handIcon.width / 3, 0), CursorMode.Auto);
					_cursorType = CursorType.Hand;
				}
			}
		}
	}
}
