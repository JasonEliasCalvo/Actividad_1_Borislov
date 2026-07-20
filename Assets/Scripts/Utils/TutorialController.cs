using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class TutorialController : MonoBehaviour
{
    [Header("UI Tutorial Elements")]
    public GameObject TutorialPanel;
    public TextMeshProUGUI titleText;
    public VideoPlayer tutorialVideo;
    public TextMeshProUGUI instructionText;
    public TutorialDatabase tutorialDatabase;

    public static TutorialController instance;

    public void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void ShowTutorial(int tutorialID)
    {
        TutorialInfo info = tutorialDatabase.GetTutorialByID(tutorialID);

        GameManager.instance.MovingCamera(false);
        GameManager.instance.InitialGameEnd();

        titleText.text = info.title;
        tutorialVideo.clip = info.tutorialVideo;
        instructionText.text = info.instruction;
        UIManager.instance.ShowPanelOn(TutorialPanel);
        tutorialVideo.Play();
        info = null;
    }

    public void HideTutorial()
    {
        GameManager.instance.MovingCamera(true);
        GameManager.instance.InitialGameStart();
        UIManager.instance.ShowPanelOff(TutorialPanel);
    }
}
