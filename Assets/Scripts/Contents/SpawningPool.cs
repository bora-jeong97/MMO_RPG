using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// 몬스터 스폰 풀
public class SpawningPool : MonoBehaviour
{
    [SerializeField]    
    int _monsterCount = 0;   // 현재 몬스터 마리 수
    int _reserveCount = 0;   // 예약생성 카운트

    [SerializeField]
    int _keepMonsterCount = 0; // 몬스터당 유지시킬 마리 수 

    [SerializeField]
    Vector3 _spawnPos;  // 스폰 중심점

    [SerializeField]
    float _spawnRadius = 15.0f; // 중심점 기준 랜덤 주변부

    [SerializeField]
    float _spawnTime = 5.0f;    // 스폰까지 걸리는 시간

    public void AddMonsterCount(int value){_monsterCount += value;}
    public void SetKeepMonsterCount(int count) { _keepMonsterCount = count; }
    


    // Start is called before the first frame update
    void Start()
    {
        Managers.Game.OnSpawnEvent -= AddMonsterCount;
        Managers.Game.OnSpawnEvent += AddMonsterCount;
    }

    // Update is called once per frame
    void Update()
    {
        // 예약 생성 값과 실제 몬스터의 합이 최소 유지 수보다 적으면 스폰
        while(_reserveCount + _monsterCount < _keepMonsterCount)
        {
            StartCoroutine("ReserveSpawn");
        }
    }

    // 스폰
    IEnumerator ReserveSpawn()  // wait 시간을 두기 위해 코루틴으로 설정함.
    {
        _reserveCount++;
        // 생성시간
        yield return new WaitForSeconds(Random.Range(0, _spawnTime));   // 0~_spawnTime시간 중에 랜덤으로 생성됨
        GameObject obj = Managers.Game.Spawn(Define.WorldObject.Monster, "Knight");

        // 갈 수 있는 영역인지 체크
        NavMeshAgent nma = obj.GetOrAddComponent<NavMeshAgent>();


        // 랜덤위치 생성
        Vector3 randPos;
        while (true)
        {
            Vector3 randDir = Random.insideUnitSphere * Random.Range(0, _spawnRadius);   // 랜덤 방향과 거리
            randDir.y = 0;
            randPos = _spawnPos + randDir;  // 스폰 중심점으로부터 randDir 방향으로 멀어진곳.

            // 갈 수 있는 길이 나올 때까지 찾는다.
            NavMeshPath path = new NavMeshPath();
            if (nma.CalculatePath(randPos, path)) // 갈 수 없는 길이면 false를 뱉는 함수.
                break;
        }

        obj.transform.position = randPos;
        _reserveCount--;

    }
}
