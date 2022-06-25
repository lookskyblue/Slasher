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
    
    [SerializeField] private Transform player_transform;

    public Transform Player_Transform
    {
        get { return player_transform; }
    }

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
            InitDamageTextTransform(ref obj);
        }
    }
    public GameObject GetObjectFromPoolingQueue(string key)
    {
        GameObject obj = null;

        if(object_pooling_dic.ContainsKey(key) == false)
        {
            Debug.LogError("Object pooling key is not found.");

            return null;
        }

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

    public void InitDamageTextTransform(ref GameObject obj)
    {
        GameObject prefab = object_dic["DamageCanvas"];

        obj.transform.localPosition = prefab.transform.position;
        obj.transform.localRotation = prefab.transform.rotation;
        obj.transform.localScale = prefab.transform.localScale;
    }
}
