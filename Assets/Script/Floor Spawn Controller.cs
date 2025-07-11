using UnityEngine;

public class FloorSpawnController : MonoBehaviour
{
    // プレイヤーのTransform（Inspectorでドラッグして設定）
    [SerializeField] private Transform player;

    // 生成する床のPrefab（Inspectorで設定）
    [SerializeField] private GameObject platformPrefab;

    // プレイヤーが着地する対象のタグ（例："Ground"）
    [SerializeField] private string groundTag = "Ground";

    // 床の生成位置のオフセット（左右候補）
    [SerializeField] private Vector2 rightOffset = new Vector2(3f, 3.5f);
    [SerializeField] private Vector2 leftOffset = new Vector2(-3f, 3.5f);

    // プレイヤーがすでに着地していたかを判定するフラグ
    private bool hasLanded = false;

    public void SpawnNewPlatform()
    {
        // 一度だけ生成する（連続着地防止）
        if (!hasLanded)
        {
            hasLanded = true;

            // 左右どちらかをランダムに選択
            Vector2 offset = (Random.value < 0.5f) ? rightOffset : leftOffset;

            Vector3 spawnPosition = new Vector3(
                player.position.x + offset.x,
                player.position.y + offset.y,
                player.position.z
            );

            Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
        }
    }

    public void ResetLandingFlag()
    {
        // 床の生成済みフラグをリセット（次回のジャンプで生成可能に）
        hasLanded = false;
    }

    void OnCollisionExit(Collision collision)
    {
        // プレイヤーが床から離れたら、次の着地イベントを許可する
        if (collision.gameObject.CompareTag(groundTag))
        {
            hasLanded = false;
        }
    }
}
