using UnityEngine;
using TMPro;

public class ResultScoreController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultScoreText;

    void Start()
    {
        // ゲーム中のスコアを取得して表示する
        resultScoreText.text = "最終スコア：" + ScoreController.finalScore;
    }
}
