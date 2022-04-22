using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropellorAnimation : MonoBehaviour
{
    public GameObject engine;
    public GameObject rotorBlur;

    [Header("Propellor Rotation Speed Properties")]
    public AnimationCurve propRotationSpeed;
    public float rpmConversion = 1f;
    [Header("Propellor Blurring Dynamics")]
    public AnimationCurve propFade;
    public AnimationCurve propBlurFade;
    [Header("Spooling Characteristics")]
    [SerializeField] float spoolUpTime;
    [SerializeField] float spoolDownTime;

    Material propMat;
    Material blurMat;
    Propulsion prop;
    ParticleSystem particles;
    float angle;
    float time;
    float timeElapsed;


    void Start()
    {
        prop = engine.GetComponent<Propulsion>();
        propMat = GetComponent<Renderer>().material;
        blurMat = rotorBlur.GetComponent<Renderer>().material;
        if(engine.TryGetComponent<ParticleSystem>(out ParticleSystem system))
        {
            particles = system;
        }
        AnimateCruise(); // tentative
        time = -spoolUpTime;
    }

    void Update()
    {
        switch(prop.state)
        {
            case 1:
                //PlayParticles();
                Invoke(nameof(PlayParticles), 1f);
                AnimateSpoolUp();
                break;
            case 2:
                AnimateCruise();
                break;
            case 3:
                if (prop.GetCurrentThrust == 0)
                {
                    AnimateSpoolDown();
                }
                else
                {
                    AnimateCruise();
                }
                break;
        }
       
    }

    void AnimateCruise()
    {
        angle = -1 * propRotationSpeed.Evaluate(prop.GetCurrentThrust) * rpmConversion * 6f * Time.deltaTime;
        transform.Rotate(0, 0, angle, Space.Self);

        Color propColor = propMat.color;
        propColor.a = propFade.Evaluate(prop.GetCurrentThrust);
        propMat.color = propColor;

        Color blurColor = blurMat.color;
        blurColor.a = propBlurFade.Evaluate(prop.GetCurrentThrust);
        blurMat.color = blurColor;
    }

    void AnimateSpoolUp()
    {
         angle = -1 * propRotationSpeed.Evaluate(time) * rpmConversion * 6f * Time.deltaTime;
         time += Time.deltaTime;
         transform.Rotate(0, 0, angle, Space.Self);
        
        if (time >= 0)
        {
            prop.state = 2;
            time = -spoolUpTime;
        }
    }

    void AnimateSpoolDown()
    {
        angle = -1 * Mathf.Lerp(propRotationSpeed.Evaluate(0f), 0f, timeElapsed / spoolDownTime)
            * rpmConversion * 6f * Time.deltaTime;
        transform.Rotate(0, 0, angle, Space.Self);

        timeElapsed += Time.deltaTime;

        if(timeElapsed >= spoolDownTime)
        {
            prop.state = 0;
            timeElapsed = 0f;
        }

    }

    void PlayParticles()
    {
        if (particles != null && !particles.isPlaying)
        {
            particles.Play();
        }
    }
}
