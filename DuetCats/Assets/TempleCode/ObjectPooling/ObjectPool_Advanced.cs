using BulletHell;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class ObjectPool_Advanced : SingletonMonoBehaviour<ObjectPool_Advanced>
{
    #region Data
    private Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();
    public Dictionary<string, List<GameObject>> objectActive = new Dictionary<string, List<GameObject>>();
    public Dictionary<string, List<GameObject>> objectWaitToActive = new Dictionary<string, List<GameObject>>();
    // public Dictionary<GameObject, Pool<ProjectileData>.Node> id = new Dictionary<GameObject, Pool<ProjectileData>.Node>();
    #endregion

    #region Public
    public GameObject GetObject(GameObject gameObject)
    {
        GameObject result;
        if (objectPool.ContainsKey(gameObject.name))
        {
            if (objectPool[gameObject.name].Count == 0)
                result = CreateNewObject(gameObject);
            else
            {
                GameObject _object = objectPool[gameObject.name].Dequeue();
                _object.SetActive(true);

                result = _object;
            }
            objectActive[gameObject.name].Add(result);
            objectWaitToActive[gameObject.name].Add(result);
        }
        else
        {
            CreateDictionaryID(gameObject.name);
            result = CreateNewObject(gameObject);
            objectActive[gameObject.name].Add(result);
            objectWaitToActive[gameObject.name].Add(result);
        }
        return result;
    }
    public void ReturnGameObject(GameObject gameObject)
    {
        if (objectPool.ContainsKey(gameObject.name))
        {
            objectPool[gameObject.name].Enqueue(gameObject);
            objectActive[gameObject.name].Remove(gameObject);
        }
        else
        {
            CreateDictionaryID(gameObject.name);
            objectPool[gameObject.name].Enqueue(gameObject);

        }

        //   id[gameObject].ReturnNode();
        //   id.Remove(gameObject);
        gameObject.SetActive(false);

    }

    public List<GameObject> GetListObjectActive(string prefabName)
    {
        if (!objectActive.ContainsKey(prefabName))
        {
            CreateDictionaryID(prefabName);
        }
        return objectActive[prefabName];
    }
    public List<GameObject> GetListWaitActive(string prefabName)
    {
        if (!objectWaitToActive.ContainsKey(prefabName))
        {


        }
        return objectWaitToActive[prefabName];
    }

    #endregion

    #region Private
    private GameObject CreateNewObject(GameObject gameObject)
    {
        GameObject newGO = Instantiate(gameObject);
        newGO.name = gameObject.name;
        return newGO;
    }
    private void CreateDictionaryID(string key)
    {
        Queue<GameObject> newObjectQueue = new Queue<GameObject>();
        List<GameObject> newObjectList = new List<GameObject>();
        List<GameObject> newObjectListWait = new List<GameObject>();
        objectPool.Add(key, newObjectQueue);
        objectActive.Add(key, newObjectList);
        objectWaitToActive.Add(key, newObjectListWait);

    }

    #endregion

}
