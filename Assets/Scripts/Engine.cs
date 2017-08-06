using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XInputDotNetPure;

public class Engine : MonoBehaviour
{
    public Material[] powerupMats = new Material[4];
    public GameObject[] powerupSpawns = new GameObject[9];
    public List<int[]> powerupSpawnSets = new List<int[]>();

    public GameObject[] playerShips = new GameObject[2];
    GamePadState[] prevStates = new GamePadState[2];
    GamePadState[] padStates = new GamePadState[2];

    Text txt;

    public int powerupsSpawned = 0;
    bool powerupsQueued = false;

    const float pauseStartThreshold = 0.25f;
    float pauseStartTimer = 0;
    enum pauseState { unpaused, paused, gameover, gamestart };
    pauseState isPaused;
    bool[] playersReady = new bool[] { false, false };

    void Start ()
    {
        txt = GameObject.Find("CountdownText").GetComponent<Text>();
        
        isPaused = pauseState.gamestart;
        StartCoroutine(CountDown(3));

        //Possible spawn location sets for powerups
        powerupSpawnSets.Add(new int[] { 0, 2, 6, 8 });
        powerupSpawnSets.Add(new int[] { 1, 3, 5, 7 });
        powerupSpawnSets.Add(new int[] { 0, 4, 8 });
        powerupSpawnSets.Add(new int[] { 2, 4, 6 });
        powerupSpawnSets.Add(new int[] { 1, 4, 7 });
        powerupSpawnSets.Add(new int[] { 3, 4, 5 });

        StartCoroutine(SpawnPowerups(3+5, 4));
    }

    IEnumerator SpawnPowerups(float delay, int set)
    {
        int abilCode;
        int[] spawnSet;
        powerupsQueued = true;
        //Go to random set if input set is -1
        if (set == -1) { set = Random.Range(0, powerupSpawnSets.Count);  }
        spawnSet = powerupSpawnSets[set];
        yield return new WaitForSeconds(delay);
        for(int i = 0; i < spawnSet.Length; i++)
        {
            abilCode = Random.Range(1, 4);
            powerupSpawns[spawnSet[i]].GetComponent<Powerup>().Activate(abilCode, powerupMats[abilCode], 5f);
            powerupsSpawned++;
        }
        powerupsQueued = false;
    }

    void Update()
    {
        if(isPaused != pauseState.gameover)
        {
            if (playerShips[0].GetComponent<MovementScript>().stock > 0 && playerShips[1].GetComponent<MovementScript>().stock > 0)
            {
                int x = GameObject.Find("Scout Ship 1").GetComponent<MovementScript>().stock;
                Image stockImg = GameObject.Find("P1Stocks").GetComponent<Image>();
                float y = stockImg.rectTransform.sizeDelta.y;
                stockImg.rectTransform.sizeDelta = new Vector2(y * x, y);
                stockImg.color = GameStats.GetColor(1);

                x = GameObject.Find("Scout Ship 2").GetComponent<MovementScript>().stock;
                stockImg = GameObject.Find("P2Stocks").GetComponent<Image>();
                y = stockImg.rectTransform.sizeDelta.y;
                stockImg.rectTransform.sizeDelta = new Vector2(y * x, y);
                stockImg.color = GameStats.GetColor(2);
            }
            else
            {
                GameObject.Find("P1ReadyText").GetComponent<Text>().text = "P1: Press A";
                GameObject.Find("P1ReadyText").GetComponent<Text>().color = GameStats.GetColor(1);
                GameObject.Find("P2ReadyText").GetComponent<Text>().text = "P2: Press A";
                GameObject.Find("P2ReadyText").GetComponent<Text>().color = GameStats.GetColor(2);
                isPaused = pauseState.gameover;
                Time.timeScale = 0.1f;
                if (playerShips[1].GetComponent<MovementScript>().stock == 0)
                {
                    txt.color = GameStats.GetColor(1);
                    txt.text = "P1 Wins!";
                    GameObject.Find("P2Stocks").GetComponent<Image>().enabled = false;
                }
                else if (playerShips[0].GetComponent<MovementScript>().stock == 0)
                {
                    txt.color = GameStats.GetColor(2);
                    txt.text = "P2 Wins!";
                    GameObject.Find("P1Stocks").GetComponent<Image>().enabled = false;
                }
            }
        }

        if(!powerupsQueued && powerupsSpawned == 0)
        {
            StartCoroutine(SpawnPowerups(10f, -1));
        }

        prevStates[0] = padStates[0];
        prevStates[1] = padStates[1];

        padStates[0] = GamePad.GetState((PlayerIndex)GameStats.P1);
        padStates[1] = GamePad.GetState((PlayerIndex)GameStats.P2);

        if(padStates[0].Buttons.A == ButtonState.Pressed && isPaused == pauseState.gameover)
        {
            GameObject.Find("P1ReadyText").GetComponent<Text>().text = "Waiting for other player";
            playersReady[0] = true;
        }
        if (padStates[1].Buttons.A == ButtonState.Pressed && isPaused == pauseState.gameover)
        {
            GameObject.Find("P2ReadyText").GetComponent<Text>().text = "Waiting for other player";
            playersReady[1] = true;
        }
        if(playersReady[0] && playersReady[1])
        {
            GameObject.Find("P1ReadyText").GetComponent<Text>().text = "GG";
            GameObject.Find("P2ReadyText").GetComponent<Text>().text = "WP";
            StartCoroutine(ReturnToMenu(0));            
        }

        if ((padStates[0].Buttons.Start == ButtonState.Pressed && prevStates[0].Buttons.Start == ButtonState.Released || padStates[1].Buttons.Start == ButtonState.Pressed && prevStates[1].Buttons.Start == ButtonState.Released) && isPaused == pauseState.paused)
        {
            //Unpause the game if it's already paused
            pauseStartTimer = 0;
            StartCoroutine(CountDown(3));
        }

        if((padStates[0].Buttons.Start == ButtonState.Pressed || padStates[1].Buttons.Start == ButtonState.Pressed) && isPaused == pauseState.unpaused)
        {
            pauseStartTimer += Time.deltaTime;
            if (pauseStartTimer >= pauseStartThreshold)
            {
                //Deploy Pause Menu if Start is held down for longer or equal to minimum threshold
                pauseStartTimer = 0;
                txt.text = "PAUSED";
                isPaused = pauseState.paused;
                Time.timeScale = 0;
            }
        }
        else
        {
            pauseStartTimer = 0;
        }
    }

    IEnumerator CountDown(int s)
    {
        Time.timeScale = 0;
        for(int i=s; i>0; i--)
        {
            txt.text = i.ToString();
            yield return new WaitForSecondsRealtime(1);
        }
        Time.timeScale = 1;
        txt.text = "FIRE!";
        yield return new WaitForSecondsRealtime(1);
        txt.text = "";
        isPaused = pauseState.unpaused;
    }

    IEnumerator ReturnToMenu(float t)
    {
        yield return new WaitForSecondsRealtime(t);
        SceneManager.LoadScene("Menu");
    }
}
