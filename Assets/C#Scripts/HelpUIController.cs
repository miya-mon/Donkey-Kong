using UnityEngine;
using UnityEngine.UI;

public class HelpUIController : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("操作説明パネル（ImageやPanel の GameObject）")]
    public GameObject helpPanel;

    [Tooltip("閉じるボタン（helpPanel 内の Button）")]
    public Button closeButton;

    [Header("Options")]
    [Tooltip("Help 表示中にゲームをポーズするなら true にする（StartScene では通常 false）")]
    public bool pauseWhileShown = false;

    // 内部状態
    bool isShown = false;
    float previousTimeScale = 1f;

    void Start()
    {
        // 初期は非表示
        if (helpPanel != null)
            helpPanel.SetActive(false);

        // closeButton が設定されていれば、Inspector に設定する代わりに自動でフックする
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(HideHelp);
        }
    }

    // StartScene のボタンから呼ぶ（Inspector の OnClick で設定）
    public void ShowHelp()
    {
        if (helpPanel == null) return;

        helpPanel.SetActive(true);
        isShown = true;

        if (pauseWhileShown)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }
    }

    // closeButton の OnClick で呼ぶ（Inspector で設定するか自動接続）
    public void HideHelp()
    {
        if (helpPanel == null) return;

        helpPanel.SetActive(false);
        isShown = false;

        if (pauseWhileShown)
        {
            Time.timeScale = previousTimeScale;
        }
    }

    // 画面外クリックで閉じたい場合などは Update に処理を足せます
}
