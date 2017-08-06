using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Abilities/Explosive Shot")]
public class AbilityShotExplosive : AbilityShotBasic
{
    public GameObject shrapnel;

    public float bombVelocity;
    public float bombSpread;
    public int bombNumBullets;

    public int poolTotal2;
    protected List<GameObject> shrapnels;

    protected List<int> activeBombs;

    public override void Initialize(int team)
    {
        Initialize(team, null);
    }

    public override void Initialize(int team, GameObject tar)
    {
        base.Initialize(team, tar);

        shrapnels = new List<GameObject>();
        for (int i = 0; i < poolTotal2; i++)
        {
            GameObject obj = (GameObject)Instantiate(shrapnel);
            obj.GetComponent<PlayerTeam>().Initialize(team);
            obj.SetActive(false);
            obj.GetComponent<Projectile>().Initialize(this);
            shrapnels.Add(obj);
        }

        activeBombs = new List<int>();
    }

    public override void PreCastAbility(Transform source)
    {
        if(triggerRecast)
        {
            for (int j = 0; j < activeBombs.Count; j++)
            {
                if (projectiles[activeBombs[j]].activeInHierarchy)
                {
                    projectiles[activeBombs[j]].GetComponent<Projectile>().Deactivate();
                }
            }
            activeBombs.Clear();
        }
    }

    public override void CastAbility(Transform t)
    {
        if(!triggerRecast)
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
                    projectiles[i].GetComponent<Projectile>().SetAcceleration(0);
                    activeBombs.Add(i);
                    tempCount++;
                }
                if (tempCount >= numBullets)
                {
                    break;
                }
            }
        }
    }

    public override bool RecastConditionValid()
    {
        return activeBombs.Count > 0;
    }

    public override void AdjustRecast(GameObject source)
    {
        for (int j = 0; j < activeBombs.Count; j++)
        {
            if (projectiles[activeBombs[j]] == source)
            {
                int tempCount = 0;
                for (int i = 0; i < shrapnels.Count; i++)
                {
                    if (!shrapnels[i].activeInHierarchy)
                    {
                        shrapnels[i].transform.position = projectiles[activeBombs[j]].transform.position;
                        shrapnels[i].transform.rotation = Quaternion.Euler(0, -bombSpread / 2.0f + tempCount * bombSpread / (bombNumBullets - 1), 0) * projectiles[activeBombs[j]].transform.rotation;
                        shrapnels[i].SetActive(true);
                        shrapnels[i].GetComponent<Projectile>().SetSpeed(bombVelocity);
                        tempCount++;
                    }
                    if (tempCount >= bombNumBullets)
                    {
                        break;
                    }
                }

                activeBombs.Remove(activeBombs[j]);
                break;
            }
        }
    }

    public override void CleanupAbility(Transform t)
    {
    }
}