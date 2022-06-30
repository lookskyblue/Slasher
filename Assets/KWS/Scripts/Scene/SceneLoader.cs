using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private CanvasGroup scene_loader_canvas_group;
    [SerializeField] private Image progress_bar;
    [SerializeField] private Text game_info_text;
    [SerializeField] private string[] game_info_list;
    private System.Random rand = new System.Random();

    private string load_scene_name;
    private static SceneLoader instance = null;
    public static SceneLoader Instance
    {
        get
        {
            if(instance == null)
            {
                SceneLoader scene_loader = FindObjectOfType<SceneLoader>();

                if(scene_loader != null)
                {
                    instance = scene_loader;
                }

                else
                {
                    instance = Create();
                }
            }

            return instance;
        }

        private set
        {
            instance = value;
        }
    }

    private static SceneLoader Create()
    {
        SceneLoader scene_loader_prefab = Resources.Load<SceneLoader>("SceneLoader");
        
        return Instantiate(scene_loader_prefab);
    }

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void LoadScene(string scene_name)
    {
        gameObject.SetActive(true);

        load_scene_name = scene_name;
        SceneManager.sceneLoaded += LoadSceneEnd;
        StartCoroutine(Load(scene_name));
    }
    IEnumerator Load(string scene_name)
    {
        progress_bar.fillAmount = 0f;

        SettingGameInfoText();

        yield return StartCoroutine(Fade(true));

        AsyncOperation op = SceneManager.LoadSceneAsync(scene_name);
        op.allowSceneActivation = false;

        float total_time = 0f;

        while(op.isDone == false)
        {
            yield return null;

            total_time += (Time.deltaTime * 0.4f);
            
            float prog = op.progress * 0.4f;

            prog = Mathf.Lerp(prog, op.progress, total_time);

            if(prog < 0.9f)
            {
                progress_bar.fillAmount = Mathf.Lerp(progress_bar.fillAmount, prog, total_time);
                
                if(progress_bar.fillAmount >= prog)
                {
                    total_time = 0f;
                }
            }

            else
            {
                progress_bar.fillAmount = Mathf.Lerp(progress_bar.fillAmount, 1f, total_time);

                if(progress_bar.fillAmount == 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
        op.allowSceneActivation = true;
    }
    void SettingGameInfoText()
    {
        int rand_value = rand.Next(0, game_info_list.Length);

        game_info_text.text = game_info_list[rand_value];
    }
    IEnumerator Fade(bool is_fade_out)
    {
        float total_time = 0f;

        while(total_time < 1f)
        {
            total_time += Time.deltaTime;
            scene_loader_canvas_group.alpha = Mathf.Lerp((is_fade_out == true ? 0 : 1), (is_fade_out == true ? 1 : 0), total_time);
            yield return null;
        }

        if(is_fade_out == false)
        {
            gameObject.SetActive(false);
        }
    }
    void LoadSceneEnd(Scene scene, LoadSceneMode load_scene_moad)
    {
        if(scene.name.Equals(load_scene_name) == true)
        {
            StartCoroutine(Fade(false));
            SceneManager.sceneLoaded -= LoadSceneEnd;
            GameManager.instance.ReportEndOfSceneLoad(scene);
        }
    }
}
