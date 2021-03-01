using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public void ActivateHat()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
