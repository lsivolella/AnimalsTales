using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    [Header("Virtual Cameras")]
    [SerializeField] List<CinemachineVirtualCamera> virtualCameras;
    [Header("Camera Shake")]
    [SerializeField] float shakeAmplitude = 1f;
    [SerializeField] float shakeFrequency = 1f;
    [SerializeField] float shakeDuration = 1f;

    // Cached References
    GameObject playerGameObject;
    public static CameraShake instance { get; private set; }
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;

    private void Awake()
    {
        instance = this;
        GetAccessToComponents();
        SetPlayerAsFollowTarget();
    }

    private void GetAccessToComponents()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.
            GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        playerGameObject = GameObject.FindGameObjectWithTag("Player");
    }

    private void SetPlayerAsFollowTarget()
    {
        cinemachineVirtualCamera.enabled = true;
        cinemachineVirtualCamera.Follow = playerGameObject.transform;

        if (virtualCameras.Count > 1)
        {
            virtualCameras[1].gameObject.SetActive(false);
        }
    }

    public void CallShakeCoroutine()
    {
        StartCoroutine("ShakeCoroutine");
    }

    IEnumerator ShakeCoroutine()
    {
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = shakeAmplitude;
        cinemachineBasicMultiChannelPerlin.m_FrequencyGain = shakeFrequency;

        yield return new WaitForSeconds(shakeDuration);

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
        cinemachineBasicMultiChannelPerlin.m_FrequencyGain = 0f;
    }

    public void TurnCinematicCameraOn()
    {
        cinemachineVirtualCamera.gameObject.SetActive(false);
        virtualCameras[1].gameObject.SetActive(true);
    }

    public void TurnCinematicCameraOff()
    {
        virtualCameras[1].gameObject.SetActive(false);
        cinemachineVirtualCamera.gameObject.SetActive(true);
    }
}
