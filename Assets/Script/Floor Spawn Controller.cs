using UnityEngine;

public class FloorSpawnController : MonoBehaviour
{
    // プレイヤーのTransform（プレイヤーの位置を取得するために使用）
    [SerializeField] private Transform player;

    // 床プレハブ（生成時に使うオブジェクトの元）
    [SerializeField] private GameObject platformPrefab;

    // 地面に使うタグ（接触判定などで使用）
    [SerializeField] private string groundTag = "Ground";

    // 床を左右に生成するときの位置補正（プレイヤー位置に足す）
    [SerializeField] private Vector2 rightOffset = new Vector2(3f, 3.5f);  // 右側へのオフセット
    [SerializeField] private Vector2 leftOffset  = new Vector2(-3f, 3.5f); // 左側へのオフセット

    // 床の多重生成を防ぐためのフラグ（着地直後に true → ジャンプ後に false）
    private bool hasLanded = false;

    // 床に色をつける処理の補助関数（Materialがついていれば色を変更）
    void SetPlatformColor(GameObject platform, Color color)
    {
        Renderer renderer = platform.GetComponent<Renderer>();
        if (renderer != null)
        {
            // 床の見た目に色を設定
            renderer.material.color = color;
        }
    }

    void Start()
    {
        Color confirmedColor = player.GetComponent<Renderer>().material.color;
        SpawnNewPlatform(confirmedColor); // ✅ 初期床も引数ありで生成
    }

    public void SpawnNewPlatform(Color playerColor)
    {
        hasLanded = true;

        // 全色リストを取得
        Color[] sharedColors = FindObjectOfType<PlayerJumpToPlatform>().possibleColors;

        // プレイヤー色以外から1つ選ぶ
        Color altColor = playerColor;
        int safety = 0;
        while (altColor == playerColor && safety < 20)
        {
            altColor = sharedColors[Random.Range(0, sharedColors.Length)];
            safety++;
        }

        // 正解床をどちらにするか（true → 右、false → 左）
        bool assignToRight = Random.value < 0.5f;

        // 右床生成＋色設定
        Vector3 rightSpawn = player.position + (Vector3)rightOffset;
        GameObject rightPlatform = Instantiate(platformPrefab, rightSpawn, Quaternion.identity);
        SetPlatformColor(rightPlatform, assignToRight ? playerColor : altColor);

        // この床が正解かどうかを GroundController に渡す！
        rightPlatform.GetComponent<GroundController>().isCorrect = assignToRight;


        // 左床生成＋色設定
        Vector3 leftSpawn = player.position + (Vector3)leftOffset;
        GameObject leftPlatform = Instantiate(platformPrefab, leftSpawn, Quaternion.identity);
        SetPlatformColor(leftPlatform, assignToRight ? altColor : playerColor);

        // 左床にも同様に正解フラグを渡す
        leftPlatform.GetComponent<GroundController>().isCorrect = !assignToRight;
    }

    public void ResetLandingFlag()
    {
        // ジャンプ後に床を再度生成可能にするためのリセット処理
        hasLanded = false;
    }

    void OnCollisionExit(Collision collision)
    {
        // プレイヤーが床から離れたときの処理（着地フラグを解除する）
        if (collision.gameObject.CompareTag(groundTag))
        {
            hasLanded = false;
        }
    }
}