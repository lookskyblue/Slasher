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
    [SerializeField]
    private float battle_result_fade_out_time;
    [SerializeField]
    private CanvasGroup canvas_group;
    [SerializeField]
    private Text battle_result_text;
    [SerializeField]
    private Text gold_text;
    [SerializeField]
    private Text exp_text;
    private float compensation_gold = 0;
    private float compensation_exp = 0;
    public void ShowBattleResult(BattleResult battle_result, float gold = 0f, float exp = 0f)
    {
        compensation_gold = gold;
        compensation_exp = exp;

        StartCoroutine(ShowBattleResultUI(battle_result, gold, exp));
    }
    IEnumerator ShowBattleResultUI(BattleResult battle_result, float gold, float exp)
    {
        yield return new WaitForSeconds(3f);
        yield return null;

        float total_time = 0f;

        if (BattleResult.battle_success == battle_result) battle_result_text.text = "Battle Success";
        else if (BattleResult.battle_failed == battle_result) battle_result_text.text = "Battle Failed";
        
        battle_result_ui.SetActive(true);
        gold_text.text = "획득한 골드:  " + gold.ToString();
        exp_text.text = "획득한 경험치: " + exp.ToString();
        canvas_group.alpha = 0;

        while (total_time < battle_result_fade_out_time)
        {
            total_time += Time.deltaTime;
            canvas_group.alpha = (total_time / battle_result_fade_out_time);
            yield return null;
        }
    }
    public void PushGoToTownButton()
    {
        GameManager.instance.LoadScene("Town");
        GameManager.instance.SendCompensation(compensation_gold, compensation_exp);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            PushGoToTownButton();
        }
    }
}
