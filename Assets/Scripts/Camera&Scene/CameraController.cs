using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] NoiseSettings noiseProfile;

    // Cached References
    GameObject playerGameObject;
    public static CameraController Instance { get; private set; }
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;
    private float startingShakeIntensity = 0f;
    private float shakeDuration = 0f;
    private float shakeTimer = 0f;

    private void Awake()
    {
        Instance = this;
        GetAccessToComponents();
        SetPlayerAsFollowTarget();
    }

    private void GetAccessToComponents()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.
            GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_NoiseProfile = null;
        playerGameObject = GameObject.FindGameObjectWithTag("Player");
    }

    private void SetPlayerAsFollowTarget()
    {
        cinemachineVirtualCamera.Follow = playerGameObject.transform;
    }

    public void CameraShake(float shakeIntensity, float timer)
    {
        cinemachineBasicMultiChannelPerlin.m_NoiseProfile = noiseProfile;
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = shakeIntensity;
        startingShakeIntensity = shakeIntensity;
        shakeDuration = timer;
        shakeTimer = timer;
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            // Decrease timer
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0)
            {
                // Slowly decrease screenshake until it stops
                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain =
                Mathf.Lerp(startingShakeIntensity, 0f, 1 - shakeTimer / shakeDuration);
            }
        }
    }
}
