using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BGMManager : MonoBehaviour
{
    [System.Serializable]
    public struct BGMData
    {
        public string name;
        public AudioClip audio_clip;
        public float delay_play_time;
    }

    public static BGMManager instance;
    [SerializeField] private BGMData[] bgm_data;
    private AudioSource audio_source;
    private Dictionary<string, BGMData> bgm_dic = new Dictionary<string, BGMData>();
    
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }

        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        audio_source = GetComponent<AudioSource>();

        for (int i = 0; i < bgm_data.Length; i++)
        {
            bgm_dic.Add(bgm_data[i].name, bgm_data[i]);
        }
    }
    public void PlayBgm(string name)
    {
        if(bgm_dic.ContainsKey(name) == false)
        {
            Debug.LogError("Do not exist bgm name: " + name);

            return;
        }

        audio_source.clip = bgm_dic[name].audio_clip;
        audio_source.PlayDelayed(bgm_dic[name].delay_play_time);
    }
    public void StopBgm()
    {
        audio_source.Stop();
    }
}
