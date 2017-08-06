using UnityEngine;

public class Projectile : MonoBehaviour
{
    float acceleration;
    float velocity;
    float size;
    float duration;

    //Turn Rate is the maximum angle (in degrees) that can be rotated per second
    float turnRate = 0;
    GameObject target;

    Ability sourceAbility;

    void Awake()
    {
        velocity = 0;
        duration = 90f;
    }

    public void Initialize(Ability abil)
    {
        sourceAbility = abil;
    }
    
    public void Deactivate()
    {
        Destroy();
    }

    public void SetSpeed(float v)
    {
        velocity = v;
    }

    public void SetAcceleration(float a)
    {
        acceleration = a;
    }

    public void SetHomingTarget(GameObject tar, float theta)
    {
        target = tar;
        turnRate = theta;
    }

    void Update()
    {
        if(target != null)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation((target.transform.position - transform.position).normalized), turnRate * Time.deltaTime);
        }
        velocity += acceleration * Time.deltaTime;
        transform.position += (Quaternion.Euler(0, -45, 0) * (transform.rotation * new Vector3(Time.deltaTime * velocity, 0, Time.deltaTime * velocity)));
    }

    void OnTriggerEnter(Collider c)
    {
        if(c.gameObject.layer == 8)
        {
            Destroy();
        }
        else if (c.gameObject.name.Contains("Scout Ship") && c.gameObject.GetComponent<PlayerTeam>().teamNumber != GetComponent<PlayerTeam>().teamNumber)
        {
            c.gameObject.GetComponent<MovementScript>().LoseStock();
            Destroy();
        }
    }

    void OnEnable()
    {
        Invoke("Destroy", duration);
    }

    void Destroy()
    {
        sourceAbility.AdjustRecast(gameObject);
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        CancelInvoke();
    }
}
