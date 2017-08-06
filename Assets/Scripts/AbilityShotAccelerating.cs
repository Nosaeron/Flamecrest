using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Abilities/Accelerating Shot")]
public class AbilityShotAccelerating : AbilityShotBasic
{
    public float acceleration;

    protected List<int> activeMissiles;

    public override void Initialize(int team)
    {
        Initialize(team, null);
    }

    public override void Initialize(int team, GameObject tar)
    {
        base.Initialize(team, tar);

        activeMissiles = new List<int>();
    }

    public override void PreCastAbility(Transform source)
    {
        if(triggerRecast)
        {
            //Projectiles already out, increase acceleration
            for (int j = 0; j < activeMissiles.Count; j++)
            {
                if (projectiles[activeMissiles[j]].activeInHierarchy)
                {
                    projectiles[activeMissiles[j]].GetComponent<Projectile>().SetAcceleration(acceleration);
                }
            }
            activeMissiles.Clear();
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
                    activeMissiles.Add(i);
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
        return activeMissiles.Count > 0;
    }

    public override void AdjustRecast(GameObject source)
    {
        for (int j = 0; j < activeMissiles.Count; j++)
        {
            if (projectiles[activeMissiles[j]] == source)
            {
                activeMissiles.Remove(activeMissiles[j]);
                break;
            }
        }
    }

    public override void CleanupAbility(Transform t)
    {
    }
}