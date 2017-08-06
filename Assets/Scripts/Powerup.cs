using System.Collections;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    private int abilityCode;
    private GameObject powerChild;
    private Material abilityMat;

    void Awake()
    {
        powerChild = transform.Find("Powerup Orb").gameObject;
        powerChild.SetActive(false);

        transform.Find("Beacon").gameObject.GetComponent<Light>().enabled = false;
    }

    public void Activate(int abilID, Material mat, float delay)
    {
        abilityCode = abilID;
        abilityMat = mat;
        StartCoroutine(ToggleChildren(delay));
    }

    IEnumerator ToggleChildren(float delay)
    {
        transform.Find("Beacon").gameObject.GetComponent<Light>().enabled = true;
        yield return new WaitForSeconds(delay);
        powerChild.SetActive(true);
        powerChild.GetComponent<MeshRenderer>().material = abilityMat;
        transform.Find("Beacon").gameObject.GetComponent<Light>().enabled = false;
    }

    public int GetAbilityCode()
    {
        return abilityCode;
    }
}
