using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    protected override void Awake()
    {

        base.Awake();

        Screen.SetResolution(1280, 720, false);
    }


}
