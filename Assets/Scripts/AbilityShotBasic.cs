using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Basic Shot")]
public class AbilityShotBasic : Ability
{
    public GameObject projectile;

    public float velocity;
    public float spread;
    public int numBullets;

    public int poolTotal;
    protected List<GameObject> projectiles;

    public bool isHoming;
    public float turnRate;

    public override void Initialize(int team)
    {
        Initialize(team, null);
    }

    public override void Initialize(int team, GameObject tar)
    {
        target = tar;
        projectiles = new List<GameObject>();
        for (int i = 0; i < poolTotal; i++)
        {
            GameObject obj = (GameObject)Instantiate(projectile);
            obj.GetComponent<PlayerTeam>().Initialize(team);
            obj.SetActive(false);
            obj.GetComponent<Projectile>().Initialize(this);
            if (isHoming) { obj.GetComponent<Projectile>().SetHomingTarget(target, turnRate); }
            projectiles.Add(obj);
        }
    }

    public override void PreCastAbility(Transform source)
    {
        return;
    }

    public override void CastAbility(Transform t)
    {
        int tempCount = 0;
        for (int i = 0; i < projectiles.Count; i++)
        {
            if (!projectiles[i].activeInHierarchy)
            {
                projectiles[i].transform.position = t.position;
                if (numBullets == 1) { projectiles[i].transform.rotation = t.rotation; }
                else { projectiles[i].transform.rotation = Quaternion.Euler(0, -spread / 2.0f + tempCount * spread / (numBullets - 1), 0) * t.rotation; }
                projectiles[i].SetActive(true);
                projectiles[i].GetComponent<Projectile>().SetSpeed(velocity);
                tempCount++;
            }
            if (tempCount >= numBullets)
            {
                break;
            }
        }
    }

    public override void CleanupAbility(Transform t)
    {
        return;
    }
}