using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 엔진이랑 관련되면 Managers > Core
// 엔진이 아닌 해당 게임 컨텐츠와 관련되면 Managers > Contents
public class GameManagerEx 
{
    // Int <-> GameObject
    GameObject _player;
    HashSet<GameObject> _monster = new HashSet<GameObject>();   // Dictionary와는 달리  Key값이 없고 중복된 값이 없도록 저장할 때는 HashSet
    //Dictionary<int, GameObject> _monster = new Dictionary<int, GameObject>();

    // 요청할 때 작동하도록 Action
    public Action<int> OnSpawnEvent;    // 숫자로 관리한다. 추가는 +1 삭제는 -1

    public GameObject GetPlayer() { return _player; }

    // 스폰 - 생성
    public GameObject Spawn(Define.WorldObject type, string path, Transform parent = null)
    {
        GameObject go = Managers.Resource.Instantiate(path, parent);

        switch (type)
        {
            case Define.WorldObject.Monster:
                _monster.Add(go);
                if (OnSpawnEvent != null)
                    OnSpawnEvent.Invoke(1); // +1
                break;
            case Define.WorldObject.Player:
                _player = go;
                break;
        }
        return go;
    }


    // 월드 오브젝트 타입 판별
    public Define.WorldObject GetWorldObjectType(GameObject go)
    {
        BaseController bc = go.GetComponent<BaseController>();
        if(bc == null)
        {
            return Define.WorldObject.Unknown;
        }
        
        return bc.WorldObjectType;
    }


    // 디스폰 - 없앰
    public void Despawn(GameObject go)
    {
        Define.WorldObject type = GetWorldObjectType(go);

        switch (type)
        {
            case Define.WorldObject.Monster:
                if (_monster.Contains(go))
                    _monster.Remove(go);
                if (OnSpawnEvent != null)
                    OnSpawnEvent.Invoke(-1);    // -1
                break;
            case Define.WorldObject.Player:
                if (_player == go)
                    _player = null;
                break;
        }

        Managers.Resource.Destroy(go);
    }
}
