using UnityEngine;
using TMPro;  // TextMeshPro を使うための名前空間

public class ScoreController : MonoBehaviour
{
    // スコア値（外部から加算する）
    public int score = 0;

    // リザルト用のスコア保持変数（静的＝シーンをまたいでも維持される）
    public static int finalScore = 0;

    // 表示する TextMeshProUGUI（Inspectorで設定）
    [SerializeField] private TextMeshProUGUI scoreText;

    void Update()
    {
        // 毎フレームスコア表示を更新（必要に応じてイベント駆動に変更してもOK）
        scoreText.text = "スコア：" + score;
    }

    /// <summary>
    /// スコアを加算する関数（外部から呼び出す用）
    /// </summary>
    public void AddScore(int amount)
    {
        score += amount;
        finalScore = score;
    }

    /// <summary>
    /// スコアをリセットする関数（任意使用）
    /// </summary>
    public void ResetScore()
    {
        score = 0;
        finalScore = 0;
    }
}