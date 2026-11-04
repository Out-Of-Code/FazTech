using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CameraSystem : MonoBehaviour
{
    [Header("Camera Setup")]
    [SerializeField] private Camera[] SecurityCameras;
    [SerializeField] private RenderTexture CameraFeedTexture;

    [Header("UI")]
    [SerializeField] private GameObject CameraFeed;
    [SerializeField] private RawImage CameraDisplay;

    public UnityEvent OnCameraOpen;
    public UnityEvent OnCameraClose;
    
    public SimpleFPSController Player;

    private int CurrentCameraIndex = 1;
    private bool CamerasOpen = false;
    private bool CanUse = true;
    private GameObject currentCameraButton;
    
    void Start()
    {
        CameraFeed.SetActive(false);

        foreach (Camera Cam in SecurityCameras)
        {
            if (Cam != null)
                
                Cam.transform.parent.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (CamerasOpen)
        {
            Player.canMove = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    public void ToggleCameras()
    {
        if (CamerasOpen)
            CloseCameras();
        else
            OpenCameras();
    }

    public void OpenCameras()
    {
        if (!CanUse)
            return;
        OnCameraOpen.Invoke();
        CamerasOpen = true;
        CameraFeed.SetActive(true);
        ShowCamera(CurrentCameraIndex);
    }

    public void CloseCameras()
    {
        OnCameraClose.Invoke();
        CamerasOpen = false;
        CameraFeed.SetActive(false);

        foreach (Camera Cam in SecurityCameras)
        {
            if (Cam != null)
                Cam.transform.parent.gameObject.SetActive(false);
        }
        Player.canMove = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void NextCamera()
    {
        CurrentCameraIndex++;

        if (CurrentCameraIndex >= SecurityCameras.Length)
            CurrentCameraIndex = 0;

        if (CamerasOpen)
            ShowCamera(CurrentCameraIndex);
    }

    public void PreviousCamera()
    {
        CurrentCameraIndex--;

        if (CurrentCameraIndex < 0)
            CurrentCameraIndex = SecurityCameras.Length - 1;

        if (CamerasOpen)
            ShowCamera(CurrentCameraIndex);
    }

    private void ShowCamera(int Index)
    {
        foreach (Camera Cam in SecurityCameras)
        {
            if (Cam != null)
                Cam.transform.parent.gameObject.SetActive(false);
        }

        GameObject SelectedCamera = SecurityCameras[Index-1].transform.parent.gameObject;

        if (SelectedCamera != null)
        {
            SelectedCamera.SetActive(true);
        }
    }
    
    public void SelectCamera(int Index)
    {
        if (!CamerasOpen || !CanUse)
            return;

        if (Index < 0 || Index >= SecurityCameras.Length+1)
            return;
        if (currentCameraButton)
        {
            currentCameraButton.GetComponent<Image>().color = Color.white;
        }

        currentCameraButton = EventSystem.current.currentSelectedGameObject;
        currentCameraButton.GetComponent<Image>().color = Color.green;
        CurrentCameraIndex = Index;
        ShowCamera(CurrentCameraIndex);
        print("Showing " + SecurityCameras[Index-1].transform.parent.transform.name);
    }

    public void DisableCamera()
    {
        CanUse = false;
    }
}
