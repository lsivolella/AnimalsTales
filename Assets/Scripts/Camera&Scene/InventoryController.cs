using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameMaster;

public class InventoryController : MonoBehaviour
{
    GameMaster gameMaster;

    private void Start()
    {
        GetAccessToComponents();
        CheckHatStatus();
    }

    private void GetAccessToComponents()
    {
        gameMaster = GameMaster.instance;
    }

    private void CheckHatStatus()
    {
        if (gameMaster.hatStatus == HatStatus.Inventory)
            ActivateHat();
    }

    public void ActivateHat()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void DeactivateHat()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
