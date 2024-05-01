using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{


    public enum WorldObject
    {
        Unknown,
        Player,
        Monster
    }

    public enum State
	{
		Die,
		Moving,
		Idle,
		Skill,
	}

public enum Layer
    {
        Monster = 8,
        Ground = 9,
        Block = 10,
    }

    public enum Scene
    {
        Unknown,
        Login,
        Lobby,
        Game,
    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }

    public enum UIEvent
    {
        Click,
        Drag,
    }

    public enum MouseEvent
    {
        Press,  // 누르고 있는 상태
        PointerDown,    // 마우스를 뗀 상태에서 처음으로 눌렀을 때
        PointerUp,  // 마우스를 한 번이라도 누른 뒤에 떼는 것.
        Click,  // 마우스를 한 번이라도 누른 뒤에 0.2초 안에 떼는 것 
    }

    public enum CameraMode
    {
        QuarterView,
    }
}
