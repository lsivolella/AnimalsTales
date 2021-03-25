using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawController : MenuCanvasController
{
    private enum PawSide { Left, Right };

    [SerializeField]
    private PawSide pawSide;


    protected override void IdentifySelectedOption()
    {
        if (pawSide == PawSide.Left)
            base.IdentifySelectedOption();
        else
            return;
    }

    protected override void GetSelectionInput()
    {
        if (pawSide == PawSide.Left)
            base.GetSelectionInput();
        else
            return;
    }
}
