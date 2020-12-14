using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealhBarUI : MonoBehaviour
{
    // TODO: figure out why player is suddenly losing reference to the HealthBarUI
    // public static HealhBarUI instance { get; private set; }
    [SerializeField] Image mask;

    private float originalSize;

    private void Awake()
    {
       // instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        originalSize = mask.rectTransform.rect.width;
    }

    public void SetValue(float value)
    {
        mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize * value);
    }
}
