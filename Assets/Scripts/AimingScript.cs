using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class AimingScript : MonoBehaviour
{
    public GameObject enemy;

    public Ability[] abilities = new Ability[4];
    float[] cooldownRemaining = new float[4];
    public GameObject[] cdUI = new GameObject[4];

    PlayerIndex portNumber;
    GamePadState padState;

    bool isIdle = true;
    int castedAbility = -1;

    void Awake()
    {
        int teamNumber = GetComponent<PlayerTeam>().deployColor();
        if (teamNumber == 1) { portNumber = (PlayerIndex)GameStats.P1; }
        else if (teamNumber == 2) { portNumber = (PlayerIndex)GameStats.P2; }

        Image[] images;
        //Initialize Abilities
        for(int i = 0; i < abilities.Length; i++)
        {
            abilities[i] = Object.Instantiate(abilities[i]) as Ability;
            abilities[i].Initialize(teamNumber, enemy);
            if (abilities[i].powerUpStage == 0)
            {
                cooldownRemaining[i] = 9999;
                cdUI[i].transform.Find("CooldownMask").GetComponent<Image>().fillAmount = 1;
            }
            else
            {
                cooldownRemaining[i] = 0;
            }
            images = cdUI[i].GetComponentsInChildren<Image>();
            foreach(Image image in images)
            {
                image.sprite = abilities[i].icon;
            }
            cdUI[i].GetComponentInChildren<Text>().enabled = false;
        }

        Color lineColor = GameStats.GetColor(teamNumber);
        GetComponent<LineRenderer>().materials[0].color = lineColor;
    }

    IEnumerator CastAbility(int i)
    {
        isIdle = false;
        castedAbility = i;
        cdUI[i].transform.Find("CooldownMask").GetComponent<Image>().fillAmount = 1f;
        abilities[i].PreCastAbility(transform);
        yield return new WaitForSeconds(abilities[i].GetStartup());
        abilities[i].CastAbility(transform);
        yield return new WaitForSeconds(abilities[i].GetRecovery());
        abilities[i].CleanupAbility(transform);
        abilities[i].ToggleRecast();
        StartCoroutine(CoolDown(i));
        isIdle = true;
        castedAbility = -1;
    }

    IEnumerator CoolDown(int i)
    {
        Text cdText = cdUI[i].GetComponentInChildren<Text>();
        cdText.enabled = true;
        cdText.text = "";
        cooldownRemaining[i] = abilities[i].GetCooldown();
        cdUI[i].GetComponent<Image>().sprite = abilities[i].GetIcon();
        //Vector3 startLoc = cdUI[i].transform.position;
        //Vector3 offsetLoc = startLoc - new Vector3(0, 38.5f, 0);
        //float progress = 0;
        while (cooldownRemaining[i] > 0)
        {
            yield return new WaitForSeconds(Mathf.Min(0.0167f, cooldownRemaining[i]));
            cooldownRemaining[i] -= Mathf.Min(0.0167f, cooldownRemaining[i]);
            cdText.text = cooldownRemaining[i].ToString("F1");
            cdUI[i].transform.Find("CooldownMask").GetComponent<Image>().fillAmount = cooldownRemaining[i] / abilities[i].cooldown;
            //progress += 0.167f;
            //cdUI[i].transform.position = Vector3.Lerp(startLoc, offsetLoc, progress);
        }
        cdUI[i].transform.Find("CooldownMask").GetComponent<Image>().fillAmount = 0f;
        //cdUI[i].transform.position = startLoc;
        cdText.enabled = false;
    }

    void Update()
    {
        padState = GamePad.GetState(portNumber);

        //Always point towards the 2nd stick's location
        transform.LookAt(new Vector3(transform.position.x + padState.ThumbSticks.Right.X, transform.position.y, transform.position.z + padState.ThumbSticks.Right.Y));

        if(isIdle)
        {
            if (padState.Triggers.Right > 0.0f)
            {
                if (cooldownRemaining[0] <= 0) { StartCoroutine(CastAbility(0)); }
            }
            else if (padState.Buttons.RightShoulder == ButtonState.Pressed)
            {
                if (cooldownRemaining[1] <= 0) { StartCoroutine(CastAbility(1)); }
            }
            else if (padState.Triggers.Left > 0.0f)
            {
                if (cooldownRemaining[2] <= 0) { StartCoroutine(CastAbility(2)); }
            }
            else if (padState.Buttons.LeftShoulder == ButtonState.Pressed)
            {
                if (cooldownRemaining[3] <= 0) { StartCoroutine(CastAbility(3)); }
            }
        }
        
        //Check if any recasts have expired
        for (int i = 0; i < abilities.Length; i++)
        {
            if(castedAbility != i && abilities[i].triggerRecast && !abilities[i].RecastConditionValid())
            {
                abilities[i].triggerRecast = false;
                StartCoroutine(CoolDown(i));
            }
        }
    }

    public void UpgradeAbility(int abilityNum)
    {
        //If this is the first upgrade, unlock it
        if(abilities[abilityNum].powerUpStage == 0)
        {
            cooldownRemaining[abilityNum] = 0;
            cdUI[abilityNum].transform.Find("CooldownMask").GetComponent<Image>().fillAmount = 0;
        }
        //Power up the move
        abilities[abilityNum].UpgradeAbility();
    }
}
