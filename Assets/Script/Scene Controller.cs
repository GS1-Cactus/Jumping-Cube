using UnityEngine;
using UnityEngine.SceneManagement; // シーン遷移の命令に必要！

public class SceneController : MonoBehaviour
{
    // Inspectorから指定できるように、シーン名を公開する
    [SerializeField] private string sceneName = "GameScene";

    // ボタンが押されたときに呼ばれる関数
    public void OnStartButtonClicked()
    {
        // 指定された名前のシーンに切り替える
        SceneManager.LoadScene(sceneName);
        Debug.Log("Scene changed to: " + sceneName);
    }
}


