using UnityEngine;
using UnityEngine.UI;

public class HelpUIController : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("��������p�l���iImage��Panel �� GameObject�j")]
    public GameObject helpPanel;

    [Tooltip("����{�^���ihelpPanel ���� Button�j")]
    public Button closeButton;

    [Header("Options")]
    [Tooltip("Help �\�����ɃQ�[�����|�[�Y����Ȃ� true �ɂ���iStartScene �ł͒ʏ� false�j")]
    public bool pauseWhileShown = false;

    // �������
    bool isShown = false;
    float previousTimeScale = 1f;

    void Start()
    {
        // �����͔�\��
        if (helpPanel != null)
            helpPanel.SetActive(false);

        // closeButton ���ݒ肳��Ă���΁AInspector �ɐݒ肷�����Ɏ����Ńt�b�N����
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(HideHelp);
        }
    }

    // StartScene �̃{�^������ĂԁiInspector �� OnClick �Őݒ�j
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

    // closeButton �� OnClick �ŌĂԁiInspector �Őݒ肷�邩�����ڑ��j
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

    // ��ʊO�N���b�N�ŕ������ꍇ�Ȃǂ� Update �ɏ����𑫂��܂�
}
