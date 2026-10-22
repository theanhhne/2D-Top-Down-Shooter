
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    private const string TutorialCompletedKey = "TutorialCompleted";

    public static TutorialManager Instance { get; private set; }

    [Header("Tutorial UI")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TMP_Text tutorialTitle;
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button skipButton;

    [Header("Tutorial Settings")]
    [SerializeField] private bool enableTutorial = true;

    [Tooltip("Bật để test lại tutorial dù đã hoàn thành trước đó.")]
    [SerializeField] private bool forceShowTutorial = false;

    private int currentStep;
    private bool tutorialActive;

    private readonly string[] titles =
    {
        "DI CHUYỂN",
        "NGẮM VÀ BẮN",
        "SỐNG SÓT",
        "COLLECTABLE"
    };

    private readonly string[] messages =
    {
        "Dùng WASD hoặc phím mũi tên để di chuyển. Hãy thử di chuyển nhân vật!",
        "Di chuyển để ngắm về phía zombie, sau đó nhấn chuột trái để bắn!",
        "Theo dõi thanh máu, né zombie và tiêu diệt chúng để sống sót!",
        "Một vật phẩm vừa xuất hiện! Di chuyển đến gần nó để nhặt. Vật phẩm có thể giúp bạn hồi phục hoặc tăng sức mạnh vũ khí!"
    };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError(
                "Có nhiều hơn một TutorialManager trong scene!",
                this
            );

            enabled = false;
            return;
        }

        Instance = this;

        if (nextButton != null)
            nextButton.onClick.AddListener(FinishTutorial);

        if (skipButton != null)
            skipButton.onClick.AddListener(FinishTutorial);

        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);

        // Tắt tutorial hoàn toàn trong Inspector.
        if (!enableTutorial)
            return;

        // Nếu đã hoàn thành thì không hiện lại,
        // trừ khi đang bật chế độ test.
        if (!forceShowTutorial &&
            PlayerPrefs.GetInt(TutorialCompletedKey, 0) == 1)
        {
            return;
        }

        tutorialActive = true;
        currentStep = 0;

        if (tutorialPanel != null)
            tutorialPanel.SetActive(true);

        ShowCurrentStep();
    }

    private void ShowCurrentStep()
    {
        if (currentStep < 0 || currentStep >= messages.Length)
            return;

        if (tutorialTitle != null)
            tutorialTitle.text = titles[currentStep];

        if (tutorialText != null)
            tutorialText.text = messages[currentStep];

        if (nextButton != null)
        {
            TMP_Text buttonText =
                nextButton.GetComponentInChildren<TMP_Text>();

            if (buttonText != null)
                buttonText.text = "Hoàn thành";
        }
    }

    // Bước 1: Người chơi bắt đầu di chuyển.
    public void NotifyMovement()
    {
        if (!tutorialActive || currentStep != 0)
            return;

        currentStep = 1;
        ShowCurrentStep();
    }

    // Bước 2: Người chơi thực sự bắn đạn.
    public void NotifyShot()
    {
        if (!tutorialActive || currentStep != 1)
            return;

        currentStep = 2;
        ShowCurrentStep();
    }

    // Bước 4: Collectable xuất hiện.
    // Chuyển thẳng sang hướng dẫn nhặt vật phẩm.
    public void NotifyCollectableSpawned()
    {
        if (!tutorialActive || currentStep == 3)
            return;

        currentStep = 3;
        ShowCurrentStep();
    }

    public void NotifyCollectablePickedUp()
{
    if (!tutorialActive || currentStep != 3)
        return;

    FinishTutorial();
}

    public void FinishTutorial()
    {
        if (!tutorialActive)
            return;

        tutorialActive = false;

        // Khi test, không ghi đè trạng thái hoàn thành đã lưu.
        if (!forceShowTutorial)
        {
            PlayerPrefs.SetInt(TutorialCompletedKey, 1);
            PlayerPrefs.Save();
        }

        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (nextButton != null)
            nextButton.onClick.RemoveListener(FinishTutorial);

        if (skipButton != null)
            skipButton.onClick.RemoveListener(FinishTutorial);

        if (Instance == this)
            Instance = null;
    }
}
