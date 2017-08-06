using UnityEngine;

[CreateAssetMenu (menuName = "Abilities/Hit Scan")]
public class AbilityHitScan : Ability
{
    public float maxWidth;
    public float minWidth;

    public float growthDuration;
    public float activeDuration;

    protected Color baseColor;

    protected Transform trans;

    protected LaserHandler pewPew;

    public override void Initialize(int team)
    {
        Initialize(team, null);
    }

    public override void Initialize(int team, GameObject tar)
    {
        target = tar;

        baseColor = GameStats.GetColor(team);
    }

    public override void PreCastAbility(Transform source)
    {
        pewPew = source.GetComponent<LaserHandler>();

        pewPew.Initialize(source, minWidth, activeDuration, baseColor, 0.4f, true, false, target);
    }

    public override void CastAbility(Transform source)
    {
        pewPew.SetRotatable(false);
        pewPew.SetColor(baseColor, 1f);
        pewPew.SetWidth(maxWidth, growthDuration);
        pewPew.SetHitbox(true);
    }

    public override bool RecastConditionValid()
    {
        return false;
    }

    public override void CleanupAbility(Transform source)
    {
        return;
    }
}