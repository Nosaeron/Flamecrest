using System.Collections;
using UnityEngine;
using XInputDotNetPure;

public class MovementScript : MonoBehaviour
{
    public float movementSpeed;

    public int stock = 5;
    public bool invulnerable = false;

    Vector3 movementAngle;
    Vector3 movementVector;

    PlayerIndex portNumber;
    GamePadState padState;
    int pLayer;
    
    void Awake()
    {
        int teamNumber = GetComponent<PlayerTeam>().deployColor();
        if (teamNumber == 1) { portNumber = (PlayerIndex)GameStats.P1; }
        else if (teamNumber == 2) { portNumber = (PlayerIndex)GameStats.P2; }

        stock = 5;
        invulnerable = false;
        GetComponent<Rigidbody>().freezeRotation = true;

        pLayer = gameObject.layer;
    }
	
	void Update()
    {
        padState = GamePad.GetState(portNumber);
        
        movementAngle.x = padState.ThumbSticks.Left.X;
        movementAngle.z = padState.ThumbSticks.Left.Y;

        //Dead Zone
        if(Mathf.Sqrt(movementAngle.x*movementAngle.x + movementAngle.z*movementAngle.z) < 0.125f)
        {
            movementAngle.x = 0;
            movementAngle.z = 0;
        }

        movementVector.x = movementAngle.x * movementSpeed;
        movementVector.z = movementAngle.z * movementSpeed;

        transform.LookAt(transform.position + movementVector);

        transform.position += movementVector * Time.deltaTime;  
        transform.position = new Vector3(transform.position.x, 7, transform.position.z);
        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        GetComponent<Rigidbody>().rotation = transform.rotation;
    }

    IEnumerator BecomeInvulnerable(float t)
    {
        SetInvulnerable(true);
        gameObject.layer = 10;
        yield return new WaitForSeconds(2f);
        SetInvulnerable(false);
        gameObject.layer = pLayer;
    }

    void SetInvulnerable(bool invul)
    {
        invulnerable = invul;
        if(invul) { transform.root.gameObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(0.33f, 0.33f, 0.33f, 1f)); }
        else { transform.root.gameObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, 1f)); }
    }

    public void LoseStock()
    {
        //Lose a stock
        stock--;
        StartCoroutine(Rumbler(0.2f));

        //You're dead if you have no stocks left
        if (stock <= 0)
        {
            StartCoroutine(BecomeInvulnerable(999.0f));
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        //Otherwise you get temporary invulnerability and no longer trigger collisions
        else
        {
            StartCoroutine(BecomeInvulnerable(2.0f));
        }
    }

    IEnumerator Rumbler(float t)
    {
        GamePad.SetVibration(portNumber, 15000, 15000);
        yield return new WaitForSeconds(t);
        GamePad.SetVibration(portNumber, 0, 0);
    }
}
