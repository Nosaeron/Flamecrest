using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XInputDotNetPure;

public class MainMenu : MonoBehaviour
{
    GamePadState padState;
    GamePadState prevState;
    
    void Start()
    {
        Random.InitState(System.DateTime.Now.Millisecond);

        if (GameStats.P1 != -1)
        {
            GameObject.Find("P1 Slot").GetComponent<Text>().text = "P1: Port " + GameStats.P1;
        }
        if (GameStats.P2 != -1)
        {
            GameObject.Find("P2 Slot").GetComponent<Text>().text = "P2: Port " + GameStats.P2;
        }
    }

	void Update()
    {
        for (int i = 0; i < 8; i++)
        {
            prevState = padState;
            padState = GamePad.GetState((PlayerIndex)i);
            if (prevState.Buttons.A == ButtonState.Released && padState.Buttons.A == ButtonState.Pressed)
            {
                if (GameStats.P1 == -1 && GameStats.P1 != i)
                {
                    GameStats.P1 = i;
                    GameObject.Find("P1 Slot").GetComponent<Text>().text = "P1: Port " + i;
                }
                else if(GameStats.P1 != i && GameStats.P2 != i)
                {
                    GameStats.P2 = i;
                    GameObject.Find("P2 Slot").GetComponent<Text>().text = "P2: Port " + i;
                }
            }
            if (prevState.Buttons.B == ButtonState.Released && padState.Buttons.B == ButtonState.Pressed)
            {
                if (GameStats.P1 == i)
                {
                    GameStats.P1 = -1;
                    GameObject.Find("P1 Slot").GetComponent<Text>().text = "P1";
                }
                else if (GameStats.P2 == i)
                {
                    GameStats.P2 = -1;
                    GameObject.Find("P2 Slot").GetComponent<Text>().text = "P2";
                }
            }
        }
        if(GameStats.P1 != -1 && GameStats.P2 != -1)
        {
            //If there's a player loaded in both player slots, allow Start to initiate the game
            if (GamePad.GetState((PlayerIndex)GameStats.P1).Buttons.Start == ButtonState.Pressed || GamePad.GetState((PlayerIndex)GameStats.P2).Buttons.Start == ButtonState.Pressed)
            {
                SceneManager.LoadScene("Game Scene");
            }
            //Otherwise display "Press START"
            else
            {
                GameObject.Find("Help Text").GetComponent<Text>().text = "PRESS START";
            }
        }
        else
        {
            GameObject.Find("Help Text").GetComponent<Text>().text = "Press A to load controller ports";
        }
    }
}
