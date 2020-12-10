using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    [Header("Camera Shake")]
    [SerializeField] float shakeAmplitude = 1f;
    [SerializeField] float shakeFrequency = 1f;
    [SerializeField] float shakeDuration = 1f;

    public static CameraShake Instance { get; private set; }
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;

    private void Awake()
    {
        Instance = this;
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.
            GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
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
}
