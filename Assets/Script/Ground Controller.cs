using UnityEngine;

public class GroundController : MonoBehaviour
{
    // プレイヤーが乗っていたかどうかのフラグ
    private bool playerWasOnThis = false;


    // プレイヤーが接触してからの経過時間
    private float contactTime = 0f;


    // この床が正解床かどうか
    public bool isCorrect = false;


    // 削除までの滞在時間（Inspectorで調整可能にしてもOK）
    [SerializeField] private float destroyAfterSeconds = 0.5f;


    void Update()
    {
        // プレイヤーが乗っていた場合、滞在時間をカウント
        if (playerWasOnThis)
        {
            contactTime += Time.deltaTime;

            // 一定時間以上乗っていたら床を削除
            if (contactTime >= destroyAfterSeconds)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        // プレイヤーが床に乗っている間はフラグを true にしておく
        if (collision.gameObject.CompareTag("Player"))
        {
            playerWasOnThis = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // プレイヤーが離れた時（ジャンプした時）にも床を削除する
        if (playerWasOnThis && collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // プレイヤーがこの床に着地した瞬間
        if (collision.gameObject.CompareTag("Player") && isCorrect)
        {
            // この床が正解床なら、もう片方の床（不正解床）を探して削除する

            // "Ground" タグが付いている全床を取得
            GameObject[] allFloors = GameObject.FindGameObjectsWithTag("Ground");

            foreach (GameObject floor in allFloors)
            {
                // 自分自身は除外
                if (floor == gameObject) continue;

                // 相手床のスクリプトから「isCorrect = false(不正解床)」か確認
                GroundController other = floor.GetComponent<GroundController>();
                if (other != null && !other.isCorrect)
                {
                    Destroy(floor);  // 不正解床を削除
                    break;           // 1枚だけ削除して終了
                }
            }
        }
    }
}