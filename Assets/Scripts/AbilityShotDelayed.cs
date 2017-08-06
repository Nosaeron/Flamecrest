using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Abilities/Delayed Shot")]
public class AbilityShotDelayed : AbilityShotBasic
{
    public override void Initialize(int team)
    {
        return;
    }

    public override void CastAbility(Transform t)
    {
        return;
    }

    public override void PreCastAbility(Transform source)
    {
        return;
    }

    public override void CleanupAbility(Transform t)
    {
        return;
    }
}