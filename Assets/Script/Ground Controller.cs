using UnityEngine;

public class GroundController : MonoBehaviour
{
    // プレイヤーが乗っていたかどうかのフラグ
    private bool playerWasOnThis = false;

    // プレイヤーが接触してからの経過時間
    private float contactTime = 0f;

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
}