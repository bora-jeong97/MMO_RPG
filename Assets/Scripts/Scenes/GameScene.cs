using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;

        #region 인벤토리
        // Managers.UI.ShowSceneUI<UI_Inven>();
        #endregion

        Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;

        // 커서 컨트롤러를 game Scene에서 불러주기 위함.
        gameObject.GetOrAddComponent<CursorController>();

        
        GameObject player =  Managers.Game.Spawn(Define.WorldObject.Player, "UnityChan");
        Camera.main.gameObject.GetOrAddComponent<CameraController>().SetPlayer(player); // 카메라에 해당 플레이어 적용

        GameObject go = new GameObject { name = "SpawningPool" };
        SpawningPool pool = go.GetOrAddComponent<SpawningPool>();
        pool.SetKeepMonsterCount(5);
        
    }

    public override void Clear()
    {
        
    }
}
