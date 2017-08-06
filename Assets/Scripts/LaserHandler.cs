using UnityEngine;

public class LaserHandler : MonoBehaviour
{
    LineRenderer lr;
    Ray[] rays = new Ray[3];
    RaycastHit hit;

    GameObject target;

    Transform source;
    float targetWidth;
    float growthRate;
    float durationPool;

    bool canRotate;
    bool canHitDetect;

    Color baseColor;
    float baseAlpha;

    void Start()
    {
        lr = gameObject.GetComponent<LineRenderer>();
        lr.enabled = false;

        for (int i = 0; i < rays.Length; i++)
        {
            rays[i] = new Ray(Vector3.zero, Vector3.zero);
        }
    }

	void Update()
    {
        if(lr.enabled)
        {
            AnimationCurve curve = new AnimationCurve();
            if (Mathf.Approximately(lr.startWidth, targetWidth) || (lr.startWidth < targetWidth && lr.startWidth + growthRate * Time.deltaTime >= targetWidth) || (lr.startWidth > targetWidth && lr.startWidth + growthRate * Time.deltaTime <= targetWidth))
            {
                curve.AddKey(0, targetWidth);
                curve.AddKey(1, targetWidth);
                lr.widthCurve = curve;
            }
            else if (lr.startWidth + growthRate * Time.deltaTime <= 0.01)
            {
                lr.enabled = false;
            }
            else
            {
                curve.AddKey(0, lr.startWidth + growthRate * Time.deltaTime);
                curve.AddKey(1, lr.startWidth + growthRate * Time.deltaTime);
                lr.widthCurve = curve;
            }

            durationPool -= Time.deltaTime;
            if (durationPool <= 0)
            {
                durationPool = 99;
                SetColor(baseColor, baseAlpha);
                SetWidth(0, 0.15f);
                SetRotatable(false);
                SetHitbox(false);
            }

            if (canRotate)
            {
                rays[0].direction = source.rotation * Vector3.forward;
                if (Physics.Raycast(rays[0], out hit, 300, 1 << 8))
                {
                    lr.SetPosition(1, hit.point);
                }
            }

            if (canHitDetect)
            {
                //Create one ray for the center, the top edge, and the bottom edge of the line
                rays[1].origin = source.position + new Vector3(0, 0, lr.startWidth / 2);
                rays[1].direction = rays[0].direction;
                rays[2].origin = source.position - new Vector3(0, 0, lr.startWidth / 2);
                rays[2].direction = rays[0].direction;

                int pLayer = 13 - gameObject.GetComponent<PlayerTeam>().deployColor();

                for (int i = 0; i < rays.Length; i++)
                {
                    if (Physics.Raycast(rays[i], out hit, 300, 1 << pLayer))
                    {
                        //Damage the target if there is one
                        target.GetComponent<MovementScript>().LoseStock();
                        break;
                    }
                }
            }
        }
	}

    public void Initialize(Transform t, float width, float dur, Color c, float alpha, bool rotatable, bool hitDetection, GameObject tar)
    {
        source = t;
        target = tar;

        targetWidth = width;

        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0, width);
        curve.AddKey(1, width);
        lr.widthCurve = curve;

        baseColor = c;
        baseAlpha = alpha;
        SetColor(baseColor, baseAlpha);

        SetRotatable(rotatable);
        SetHitbox(hitDetection);

        durationPool = dur;
        
        lr.SetPosition(0, source.position);
        rays[0].origin = source.position;
        rays[0].direction = source.rotation * Vector3.forward;
        if (Physics.Raycast(rays[0], out hit, 300, 1 << 8))
        {
            //Draw a tracer
            lr.SetPosition(1, hit.point);
        }

        lr.enabled = true;
    }

    public void SetColor(Color c, float alpha)
    {
        Gradient g = new Gradient();
        g.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c, 0.0f), new GradientColorKey(c, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
        lr.colorGradient = g;
    }

    public void SetWidth(float maxWidth, float duration)
    {
        targetWidth = maxWidth;
        if (duration > 0) { growthRate = (maxWidth - lr.startWidth) / duration; }
        else { growthRate = (maxWidth - lr.startWidth) / 0.01f ; }
    }

    public void SetRotatable(bool enabled)
    {
        canRotate = enabled;
    }

    public void SetHitbox(bool enabled)
    {
        canHitDetect = enabled;
    }
}
