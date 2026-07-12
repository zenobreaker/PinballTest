using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultPopup : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Button restartButton;

    private void Awake()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnClickRestart);
        }
    }
    public void Show(bool isClear)
    {
        if (titleText != null)
            titleText.text = isClear ? "Suceess" : "Fail";
        gameObject.SetActive(true); 
    }
    private void OnClickRestart()
    {
        // 1. 팝업 UI 끄기
        gameObject.SetActive(false);

        // 2. 현재 활성화된 씬의 이름을 가져와서 다시 로드 (씬 재시작)
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    private void OnDestroy()
    {
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(OnClickRestart);
        }
    }
}
