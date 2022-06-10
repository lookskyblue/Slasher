using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectPoolingManager : MonoBehaviour
{
    [System.Serializable]
    public class ObjectInfo
    {
        public GameObject prefab;
        public Queue<GameObject> queue = new Queue<GameObject>();
        public string key;
        public int amount;
    }

    public ObjectInfo[] object_infos;
    private Dictionary<string, Queue<GameObject>> object_pooling_dic = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<string, GameObject> object_dic = new Dictionary<string, GameObject>();

    private static ObjectPoolingManager instance = null;
    public static ObjectPoolingManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }

            return instance;
        }
    }
    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }

        else
            Destroy(gameObject);

        for (int i = 0; i < object_infos.Length; i++)
        {
            object_pooling_dic.Add(object_infos[i].key, object_infos[i].queue);
            object_dic.Add(object_infos[i].key, object_infos[i].prefab);
            
            Initialize(object_infos[i]);
        }
    }
    void Initialize(ObjectInfo object_info)
    {
        for(int i = 0; i < object_info.amount; i++)
        {
            GameObject obj = Instantiate(object_info.prefab, transform);
            
            obj.SetActive(false);
            object_info.queue.Enqueue(obj);
        }
    }

    public void ReturnObjectToPoolingQueue(string key, GameObject obj)
    {
        object_pooling_dic[key].Enqueue(obj);
        obj.SetActive(false);
        obj.transform.SetParent(transform);

        if (key.CompareTo("Damage") == 0)
        {
            Text text = obj.GetComponent<Text>();
            InitDamageTextTransform(ref text);
        }
    }

    public GameObject GetObjectFromPoolingQueue(string key)
    {
        GameObject obj;

        if (object_pooling_dic[key].Count > 0)
        {
            obj = object_pooling_dic[key].Dequeue();
            obj.transform.SetParent(null);
        }

        else
        {
            obj = Instantiate(object_dic[key]);
        }

        obj.SetActive(true);

        return obj;
    }

    public void InitDamageTextTransform(ref Text text)
    {
        GameObject prefab = object_dic["Damage"];

        text.transform.localPosition = prefab.transform.position;
        text.transform.localRotation = prefab.transform.rotation;
        text.transform.localScale = prefab.transform.localScale;
    }
}
