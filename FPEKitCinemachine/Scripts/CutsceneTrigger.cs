using UnityEngine;
using Cinemachine;
using Whilefun.FPEKit;

public class CutsceneTrigger : MonoBehaviour {

    [SerializeField, Tooltip("The Cinemachine cutscene camera you want this trigger to cut to")]
    private CinemachineVirtualCameraBase cameraToTrigger;

    [SerializeField, Tooltip("The length of time (seconds) the cutscene camera should remain active")]
    private float cameraCutDuration = 3.0f;

    private float cameraCutCountdown = 0.0f;
    private bool cutsceneCameraEnabled = false;

    void Start()
    {

        // Tell the camera to follow the player once the player exists (it will be instantiated by FPECore)
        cameraToTrigger.GetComponent<CinemachineVirtualCamera>().LookAt = FPEPlayer.Instance.gameObject.transform;
        turnOffCamera();

    }

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger '"+gameObject.name+"'. Switching to cutscene for " + cameraCutDuration + " seconds.");
            turnOnCamera();
        }

    }

    void Update()
    {

        if (cutsceneCameraEnabled)
        {

            cameraCutCountdown -= Time.deltaTime;

            if(cameraCutCountdown <= 0.0f)
            {
                turnOffCamera();
                
            }

        }

    }

    private void turnOnCamera()
    {

        cameraToTrigger.gameObject.SetActive(true);
        cutsceneCameraEnabled = true;

    }

    private void turnOffCamera()
    {

        cameraToTrigger.gameObject.SetActive(false);
        cutsceneCameraEnabled = false;
        cameraCutCountdown = cameraCutDuration;

    }

}
