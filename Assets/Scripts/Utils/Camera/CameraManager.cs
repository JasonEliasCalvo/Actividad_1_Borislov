using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    public GameObject basicCam;

    public Camera manualCamera;
    public CinemachineCamera cinematicCam;

    public enum CameraStyle
    {
        Basic,
        Cinematic
    }

    public CameraStyle currentStyle;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        manualCamera.enabled = true;
        //cinematicCam.gameObject.SetActive(false);
        SwitchCameraStyle(CameraStyle.Basic);
    }

    public void SwitchCameraStyle(CameraStyle newStyle)
    {
        basicCam.SetActive(false);

        if (newStyle == CameraStyle.Basic) basicCam.SetActive(true);

        currentStyle = newStyle;
    }

    public void PlayCinematic()
    {
        manualCamera.enabled = false;
        cinematicCam.gameObject.SetActive(true);
    }

    public void EndCinematic()
    {
        cinematicCam.gameObject.SetActive(false);
        manualCamera.enabled = true;
    }
}
