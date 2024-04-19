using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(GenerateGUID))]
public class SceneItemsManager : Singleton<SceneItemsManager>, ISaveable
{

    private Transform parentItem;
    [SerializeField] private GameObject itemPrefab = null;

    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    public GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    private void AfterSceneLoad()
    {
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }

    protected override void Awake()
    {

        base.Awake();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void DestroySceneItems()
    {
        Items[] itemsInScene = FindObjectsOfType<Items>();

        for (int i = itemsInScene.Length - 1; i > -1; i++)
        {
            Destroy(itemsInScene[i].gameObject);
        }

    }

    public void InstantiateSceneItem(int itemCode, Vector3 itemPosition)
    {
        GameObject itemGameObejct = Instantiate(itemPrefab, itemPosition, Quaternion.identity, parentItem);
        Items items = itemGameObejct.GetComponent<Items>();
        items.Init(itemCode);

    }

    public void InstantiateSceneItems(List<SceneItem> sceneItemList, Vector3 itemPosition)
    {
        GameObject itemGameObject;

        foreach (SceneItem sceneItem in sceneItemList)
        {
            itemGameObject = Instantiate(itemPrefab, new Vector3(sceneItem.position.x, sceneItem.position.y, sceneItem.position.z), Quaternion.identity, parentItem);

            Items items = itemGameObject.GetComponent<Items>();
            items.ItemCode = sceneItem.itemCode;
            items.name = sceneItem.itemName;

        }

    }

    private void OnDisable()
    {
        ISaveableDeregister();
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
    }

    private void OnEnable()
    {
        ISaveableRegister();
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public void ISaveableRegister()
    {

        SaveLoadManager.Instance.iSaveableObjectList.Add(this);

    }

    public void ISaveableRestoreScene(string sceneName)
    {
        if (GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            if (sceneSave.listSceneItemDictionary != null && sceneSave.listSceneItemDictionary.TryGetValue("sceneItemList", out List<SceneItem> sceneItemList))
            {
                DestroySceneItems();
            }
        }
    }

    public void ISaveableStoreScene(string sceneName)
    {
        GameObjectSave.sceneData.Remove(sceneName);

        List<SceneItem> sceneItemList = new List<SceneItem>();
        Items[] itemsInScene = FindObjectsOfType<Items>();

        foreach (var item in itemsInScene)
        {
            SceneItem sceneItem = new SceneItem();
            sceneItem.itemCode = item.ItemCode;
            sceneItem.position = new Vector3Serializable(item.transform.position.x, item.transform.position.y, item.transform.position.z);
            sceneItem.itemName = item.name;

            sceneItemList.Add(sceneItem);

        }

        SceneSave sceneSave = new SceneSave();
        sceneSave.listSceneItemDictionary = new Dictionary<string, List<SceneItem>>();
        sceneSave.listSceneItemDictionary.Add("sceneItemList", sceneItemList);

        GameObjectSave.sceneData.Add(sceneName, sceneSave);
    }

    public GameObjectSave ISaveableSave()
    {
        ISaveableStoreScene(SceneManager.GetActiveScene().name);

        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
       if(gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }
}
