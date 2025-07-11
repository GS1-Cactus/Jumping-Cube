using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerJumpToPlatform : MonoBehaviour
{
    [Header("ジャンプ設定")]
    [SerializeField] private float jumpSpeed = 7f;           // ジャンプ速度（高いほど速く目的地へ移動）
    [SerializeField] private float jumpHeight = 4f;          // ジャンプ演出の弧の高さ
    [SerializeField] private float jumpDistance = 7f;        // 床が無いときのジャンプ距離
    [SerializeField] private string groundTag = "Ground";    // 地面の判定タグ（床オブジェクトにつける）

    [Header("落下判定")]
    [SerializeField] private float fallThreshold = 10f;      // 落下判定用の最大高度からの差分
    [SerializeField] private string resultSceneName = "Result Scene"; // 落下時に切り替えるシーン名

    private bool isJumping = false;    // ジャンプ中フラグ（多重ジャンプ防止）
    private float maxY;                // プレイヤーが到達した最高Y座標

    void Start()
    {
        // ゲーム開始時のY位置を初期最高地点として記録
        maxY = transform.position.y;
    }

    void Update()
    {
        if (isJumping) return; // ジャンプ中は入力受付を無効化

        // 左ジャンプ：Aキー入力
        if (Input.GetKeyDown(KeyCode.A))
        {
            GameObject left = FindNearestPlatform(toLeft: true);

            // 床があれば床へ、無ければ固定距離だけ左へジャンプ
            Vector3 target = (left != null)
                ? left.GetComponent<Collider>().bounds.center
                : transform.position + Vector3.left * jumpDistance;

            StartCoroutine(JumpTo(target));
        }

        // 右ジャンプ：Dキー入力
        else if (Input.GetKeyDown(KeyCode.D))
        {
            GameObject right = FindNearestPlatform(toLeft: false);
            Vector3 target = (right != null)
                ? right.GetComponent<Collider>().bounds.center
                : transform.position + Vector3.right * jumpDistance;

            StartCoroutine(JumpTo(target));
        }

        // プレイヤーのY位置を取得して落下判定処理
        float currentY = transform.position.y;

        // プレイヤーが最高高度を更新した場合は記録しなおす
        if (currentY > maxY)
        {
            maxY = currentY;
        }

        // 最大到達高度より fallThreshold 分落ちたらゲームオーバー処理
        if (currentY < maxY - fallThreshold)
        {
            SceneManager.LoadScene(resultSceneName); // リザルトシーンへ切り替え
        }
    }

    /// <summary>
    /// 指定方向（左または右）にある最も近い床オブジェクトを取得する
    /// </summary>
    GameObject FindNearestPlatform(bool toLeft)
    {
        GameObject[] platforms = GameObject.FindGameObjectsWithTag(groundTag);
        Vector3 playerPos = transform.position;

        GameObject nearest = null;
        float shortestX = Mathf.Infinity;

        foreach (GameObject platform in platforms)
        {
            Vector3 center = platform.GetComponent<Collider>().bounds.center;

            // プレイヤーから見て正しい方向にある床だけ対象
            bool isCorrectSide = toLeft ? center.x < playerPos.x : center.x > playerPos.x;
            if (!isCorrectSide) continue;

            float dist = Mathf.Abs(center.x - playerPos.x);
            if (dist < shortestX)
            {
                shortestX = dist;
                nearest = platform;
            }
        }

        return nearest;
    }

    /// <summary>
    /// ジャンプ処理（演出付きで指定座標へ移動）＋床生成＆スコア加算
    /// </summary>
    IEnumerator JumpTo(Vector3 destination)
    {
        isJumping = true;

        Vector3 start = transform.position;
        Vector3 end = new Vector3(destination.x, destination.y, start.z); // Zは固定

        float duration = Vector3.Distance(start, end) / jumpSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration); // 進行度（0〜1）

            float arc = Mathf.Sin(Mathf.PI * t) * jumpHeight; // 弧を描くY補正

            Vector3 mid = Vector3.Lerp(start, end, t); // 線形補間で移動
            transform.position = new Vector3(mid.x, mid.y + arc, mid.z);

            yield return null;
        }

        // 最終座標にぴったり合わせる
        transform.position = end;

        // 床生成処理
        FloorSpawnController spawner = FindObjectOfType<FloorSpawnController>();
        if (spawner != null)
        {
            spawner.SpawnNewPlatform();
            spawner.ResetLandingFlag();
        }

        // スコア加算処理
        ScoreController scoreSystem = FindObjectOfType<ScoreController>();
        if (scoreSystem != null)
        {
            scoreSystem.AddScore(1); // 着地でスコア＋1
        }

        isJumping = false;
    }
}