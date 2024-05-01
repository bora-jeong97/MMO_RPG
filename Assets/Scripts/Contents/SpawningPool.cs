using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// ���� ���� Ǯ
public class SpawningPool : MonoBehaviour
{
    [SerializeField]    
    int _monsterCount = 0;   // ���� ���� ���� ��
    int _reserveCount = 0;   // ������� ī��Ʈ

    [SerializeField]
    int _keepMonsterCount = 0; // ���ʹ� ������ų ���� �� 

    [SerializeField]
    Vector3 _spawnPos;  // ���� �߽���

    [SerializeField]
    float _spawnRadius = 15.0f; // �߽��� ���� ���� �ֺ���

    [SerializeField]
    float _spawnTime = 5.0f;    // �������� �ɸ��� �ð�

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
        // ���� ���� ���� ���� ������ ���� �ּ� ���� ������ ������ ����
        while(_reserveCount + _monsterCount < _keepMonsterCount)
        {
            StartCoroutine("ReserveSpawn");
        }
    }

    // ����
    IEnumerator ReserveSpawn()  // wait �ð��� �α� ���� �ڷ�ƾ���� ������.
    {
        _reserveCount++;
        // �����ð�
        yield return new WaitForSeconds(Random.Range(0, _spawnTime));   // 0~_spawnTime�ð� �߿� �������� ������
        GameObject obj = Managers.Game.Spawn(Define.WorldObject.Monster, "Knight");

        // �� �� �ִ� �������� üũ
        NavMeshAgent nma = obj.GetOrAddComponent<NavMeshAgent>();


        // ������ġ ����
        Vector3 randPos;
        while (true)
        {
            Vector3 randDir = Random.insideUnitSphere * Random.Range(0, _spawnRadius);   // ���� ����� �Ÿ�
            randDir.y = 0;
            randPos = _spawnPos + randDir;  // ���� �߽������κ��� randDir �������� �־�����.

            // �� �� �ִ� ���� ���� ������ ã�´�.
            NavMeshPath path = new NavMeshPath();
            if (nma.CalculatePath(randPos, path)) // �� �� ���� ���̸� false�� ��� �Լ�.
                break;
        }

        obj.transform.position = randPos;
        _reserveCount--;

    }
}
