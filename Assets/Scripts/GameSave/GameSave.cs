using System;
using System.Collections.Generic;

[Serializable]
public class GameSave
{

    public Dictionary<string, GameObjectSave> gameObjectData;

    public GameSave()
    {
        gameObjectData = new Dictionary<string, GameObjectSave>();
    }


}