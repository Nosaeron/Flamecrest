using UnityEngine;

public class PowerupCollisionDetector : MonoBehaviour
{
    void OnTriggerEnter(Collider c)
    {
        int abilityCode = GetComponentInParent<Powerup>().GetAbilityCode();
        if (c.gameObject.name == "Scout Ship 1")
        {
            GameObject.Find("Base 1").GetComponentInChildren<AimingScript>().UpgradeAbility(abilityCode);
            GameObject.Find("UI").GetComponent<Engine>().powerupsSpawned--;
            gameObject.SetActive(false);
        }
        else if (c.gameObject.name == "Scout Ship 2")
        {
            GameObject.Find("Base 2").GetComponentInChildren<AimingScript>().UpgradeAbility(abilityCode);
            GameObject.Find("UI").GetComponent<Engine>().powerupsSpawned--;
            gameObject.SetActive(false);
        }
    }
}
