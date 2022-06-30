using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleResult
{
    battle_failed,
    battle_success
}
public class BattleResultUIManager : MonoBehaviour
{
    [SerializeField]
    GameObject battle_result_ui;
    [Tooltip("배틀 결과창 UI가 서서히 선명해지는 속도를 나타냅니다.")]
    [SerializeField] private float battle_result_fade_out_time;
    [SerializeField] private CanvasGroup canvas_group;
    [SerializeField] private Text battle_result_text;
    [SerializeField] private Text gold_text;
    [SerializeField] private Text exp_text;
    [SerializeField] private float wait_time_to_show_for_battle_result;
    [SerializeField] private AudioClip battle_fail_sound;
    [SerializeField] private AudioClip battle_success_sound;
    private AudioSource auido_source;
    private int compensation_gold = 0;
    private int compensation_exp = 0;

    void Awake()
    {
        auido_source = GetComponent<AudioSource>();
    }
    public void ShowBattleResult(BattleResult battle_result, int gold = 0, int exp = 0)
    {
        compensation_gold = gold;
        compensation_exp = exp;

        StartCoroutine(ShowBattleResultUI(battle_result, gold, exp));
    }
    IEnumerator ShowBattleResultUI(BattleResult battle_result, int gold, int exp)
    {
        yield return new WaitForSeconds(wait_time_to_show_for_battle_result);

        if (BattleResult.battle_success == battle_result)
        {
            auido_source.PlayOneShot(battle_success_sound);
            battle_result_text.text = "Battle Success";
        }
        else if (BattleResult.battle_failed == battle_result)
        {
            auido_source.PlayOneShot(battle_fail_sound);
            battle_result_text.text = "Battle Failed"; 
        }
        
        battle_result_ui.SetActive(true);
        gold_text.text = gold == 0 ? "0" : GetThousandCommaText(gold);
        exp_text.text = exp == 0 ? "0" : GetThousandCommaText(exp);
        canvas_group.alpha = 0;

        float total_time = 0f;

        while (total_time < battle_result_fade_out_time)
        {
            total_time += Time.deltaTime;
            canvas_group.alpha = (total_time / battle_result_fade_out_time);
            yield return null;
        }
    }

    string GetThousandCommaText(int value) { return string.Format("{0:#,###}", value); }
    
    public void PushGoToTownButton()
    {
        GameManager.instance.LoadScene("Town");
        GameManager.instance.SendCompensation(compensation_gold, compensation_exp);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PushGoToTownButton();
        }
    }
}
