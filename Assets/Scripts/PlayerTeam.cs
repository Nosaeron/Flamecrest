using UnityEngine;

public class PlayerTeam : MonoBehaviour
{
    public int teamNumber;

    public void Initialize(int tn)
    {
        teamNumber = tn;
        deployColor();
    }

    public int deployColor()
    {
        transform.root.gameObject.GetComponent<Renderer>().material.SetColor("_OutlineColor", GameStats.GetColor(teamNumber));
        return teamNumber;
    }
}
