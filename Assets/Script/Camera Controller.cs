using UnityEngine;

public class CameraController : MonoBehaviour
{
    // プレイヤーなどの追従対象
    [SerializeField] private Transform target;

    // プレイヤーとの相対位置（カメラの高さ・奥行き調整）
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, -10f);

    // Y座標の最低制限距離（初期の最低ライン）
    [SerializeField] private float baseMinY = 0f;

    // カメラがこれまでに到達した最大Y位置（最低Yの基準になる）
    private float dynamicMinY;

    void Start()
    {
        // 初期値として最低Y座標を設定
        dynamicMinY = baseMinY;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // プレイヤー位置＋オフセットで目標座標を取得
        Vector3 desiredPosition = target.position + offset;

        // カメラが過去より高い位置に上がったなら、その高さに応じて最低Yを更新
        dynamicMinY = Mathf.Max(desiredPosition.y, dynamicMinY);

        // カメラが下がりすぎないように制限（現在の最低Yより下には行かない）
        desiredPosition.y = Mathf.Max(desiredPosition.y, dynamicMinY);

        // カメラ位置を更新
        transform.position = desiredPosition;
    }
}