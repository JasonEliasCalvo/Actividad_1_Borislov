using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialController : MonoBehaviour
{
    [Header("UI Tutorial Elements")]
    public GameObject TutorialPanel;
    public GameObject TutorialVideoPanel;
    public GameObject TutorialImagePanel;

    public TextMeshProUGUI titleText;
    public VideoPlayer tutorialVideo;
    public Image tutorialImage;
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

        if (info == null)
        {
            Debug.LogError($"Tutorial con ID {tutorialID} no encontrado.");
            return;
        }

        GameManager.instance.MovingCamera(false);
        GameManager.instance.InitialGameEnd();

        titleText.text = info.title;
        instructionText.text = info.instruction;

        tutorialVideo.Stop();

        if (info.tutorialImage != null)
        {
            TutorialImagePanel.SetActive(true);
            TutorialVideoPanel.SetActive(false);

            tutorialImage.sprite = info.tutorialImage;
        }
        else if (info.tutorialVideo != null)
        {
            TutorialImagePanel.SetActive(false);
            TutorialVideoPanel.SetActive(true);

            tutorialVideo.clip = info.tutorialVideo;
            tutorialVideo.Play();
        }
        else
        {
            Debug.LogWarning($"El tutorial '{info.title}' no tiene imagen ni video.");

            TutorialImagePanel.SetActive(false);
            TutorialVideoPanel.SetActive(false);
        }

        UIManager.instance.ShowPanelOn(TutorialPanel);
    }

    public void HideTutorial()
    {
        GameManager.instance.MovingCamera(true);
        GameManager.instance.InitialGameStart();
        UIManager.instance.ShowPanelOff(TutorialPanel);
    }
}
