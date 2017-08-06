using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public string abilityName = "";
    public Sprite icon;

    public int powerUpStage;
    public float bonusPerStage;
    public float startup;
    public float delay;
    public float recovery;
    public float cooldown;

    public bool enableRecast;
    public Sprite iconRecast;
    public float startupRecast;
    public float recoveryRecast;
    public float cooldownRecast;
    public bool triggerRecast = false;

    protected bool isReady = false;
    protected GameObject target;
    
    public abstract void Initialize(int i);
    public abstract void Initialize(int i, GameObject target);
    public abstract void PreCastAbility(Transform source);
    public abstract void CastAbility(Transform source);
    public abstract void CleanupAbility(Transform source);

    public virtual void UpgradeAbility()
    {
        powerUpStage++;
        cooldown *= 0.9f;
    }

    public Sprite GetIcon()
    {
        if(triggerRecast) { return iconRecast; }
        return icon;
    }

    public float GetStartup()
    {
        if (triggerRecast) { return startupRecast; }
        return startup;
    }

    public float GetRecovery()
    {
        if (triggerRecast) { return recoveryRecast; }
        return recovery;
    }

    public float GetCooldown()
    {
        if(triggerRecast) { return cooldownRecast; }
        return cooldown;
    }

    public void ToggleRecast()
    {
        if(enableRecast)
        {
            triggerRecast = !triggerRecast;
        }
    }

    public virtual void AdjustRecast(GameObject source)
    {
        return;
    }

    public virtual bool RecastConditionValid()
    {
        return true;
    }
}