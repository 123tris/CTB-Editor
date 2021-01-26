using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewComboButton : ToggleBehaviour
{
    private bool oldToggled;

    void Start()
    {
        oldToggled = toggled;
    }

    void Update()
    {
        if (InputManager.GetButtonDown(InputAction.ToggleNewCombo))
            OnToggle();


        toggled = oldToggled;
        ApplyAlpha();

        if (Selection.hasSelection == false) return;
        if (Selection.last.isSliderFruit) return;

        toggled = Selection.last.isNewCombo;
        ApplyAlpha();
    }

    public override void OnToggle()
    {
        base.OnToggle();
        if (Selection.hasSelection && Selection.last.isSliderFruit == false)
            Selection.last.isNewCombo = toggled;
        else
            oldToggled = toggled;
    }
}
