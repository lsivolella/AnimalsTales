using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] Image mask;

    public static HealthBarUI instance { get; private set; }
    private float originalSize;

    private void Awake()
    {
        SetUpSingleton();
        GetMaskOriginalSize();
    }

    private void SetUpSingleton()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance.gameObject);
        }
        else
            Destroy(instance.gameObject);
    }

    private void GetMaskOriginalSize()
    {
        originalSize = mask.rectTransform.rect.width;
    }

    public void SetValue(float value)
    {
        mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize * value);
    }
}
