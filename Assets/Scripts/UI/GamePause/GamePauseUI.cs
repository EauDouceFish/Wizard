using UnityEngine;
using UnityEngine.UI;
using QFramework;

public class GamePauseUI : UIPanelBase
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject GamePauseUIPanel;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {

        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(OnResumeButtonClicked);
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitButtonClicked);
        }

        base.Start();
    }

    private void OnResumeButtonClicked()
    {
        this.SendCommand<ToggleGamePauseCommand>();
    }

    private void OnExitButtonClicked()
    {
        this.SendCommand<ToggleGamePauseCommand>();
        this.SendCommand<BackToMainMenuCommand>();
    }

    private void OnGamePausedEvent(GamePausedEvent evt)
    {
        if (evt.isPaused)
        {
            GamePauseUIPanel.SetActive(true);
        }
        else
        {
            GamePauseUIPanel.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(OnExitButtonClicked);
        }
    }
}