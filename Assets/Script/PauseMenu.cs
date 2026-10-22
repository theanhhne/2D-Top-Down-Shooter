
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject actionPanel;

    [Header("Pause Button - Always Visible")]
    [SerializeField] private GameObject openMenuButton;

    [Header("Scene Settings")]
    [SerializeField] private string mainMenuSceneName = "Main Menu";
    [SerializeField] private SceneController sceneController;

    private bool isPaused;
    private bool isLoadingScene;

    private void Awake()
    {
        Time.timeScale = 1f;
        isPaused = false;
        isLoadingScene = false;

        // Nút mở menu luôn hiện
        if (openMenuButton != null)
            openMenuButton.SetActive(true);

        // Menu và bảng lựa chọn ẩn lúc bắt đầu
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (actionPanel != null)
            actionPanel.SetActive(false);
    }

    private void Update()
{
    if (Keyboard.current != null &&
        Keyboard.current.escapeKey.wasPressedThisFrame)
    {
        TogglePauseMenu();
    }
}

    // Nút Pause trên màn hình
    public void OpenPauseMenu()
{
    if (isPaused || isLoadingScene)
        return;

    isPaused = true;

    if (pausePanel != null)
        pausePanel.SetActive(true);

    if (actionPanel != null)
        actionPanel.SetActive(true);

    Time.timeScale = 0f;
}

    // Đóng menu và tiếp tục chơi
    public void ResumeGame()
    {
        if (isLoadingScene)
            return;

        isPaused = false;

        if (actionPanel != null)
            actionPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;
    }

    // Hiện bảng Resume, Restart, Home
    public void ShowActionPanel()
    {
        if (!isPaused || isLoadingScene)
            return;

        if (actionPanel != null)
            actionPanel.SetActive(true);
    }

    public void HideActionPanel()
    {
        if (actionPanel != null)
            actionPanel.SetActive(false);
    }

    // Chơi lại scene hiện tại
    public void RestartGame()
    {
        if (isLoadingScene)
            return;

        isLoadingScene = true;
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    // Về Main Menu có hiệu ứng fade
    public void ReturnToMainMenu()
    {
        if (isLoadingScene)
            return;

        if (sceneController == null)
        {
            Debug.LogError(
                "PauseMenu: Chưa gán SceneController trong Inspector!"
            );
            return;
        }

        isLoadingScene = true;
        Time.timeScale = 1f;

        sceneController.LoadScene(mainMenuSceneName);
    }

    // Bật/tắt menu bằng nút hoặc Esc
    public void TogglePauseMenu()
    {
        if (isLoadingScene)
            return;

        if (isPaused)
            ResumeGame();
        else
            OpenPauseMenu();
    }


    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
