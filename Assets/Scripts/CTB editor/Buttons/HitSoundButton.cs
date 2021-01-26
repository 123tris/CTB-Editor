using System;
using UnityEngine;
using System.Collections;
using OsuParsers.Enums.Beatmaps;

public class HitSoundButton : ToggleBehaviour
{
    [SerializeField]
    private HitSoundType hitSoundType;

    //TODO: Convoluted UI code needs to be simplified
    void Update()
    {
        if (Selection.hasSelection)
        {
            toggled = Selection.last.hitSound.HasFlag(hitSoundType);
            ApplyAlpha();
        }

        switch (hitSoundType)
        {
            case HitSoundType.Whistle:
                if (InputManager.GetButtonDown(InputAction.ToggleWhistle))
                    OnToggle();
                break;
            case HitSoundType.Finish:
                if (InputManager.GetButtonDown(InputAction.ToggleFinish))
                    OnToggle();
                break;
            case HitSoundType.Clap:
                if (InputManager.GetButtonDown(InputAction.ToggleClap))
                    OnToggle();
                break;
        }
    }

    public override void OnToggle()
    {
        base.OnToggle();
        if (Selection.hasSelection == false || Selection.last.isSliderFruit) return;
        Selection.last.hitSound ^= hitSoundType;
    }
}
