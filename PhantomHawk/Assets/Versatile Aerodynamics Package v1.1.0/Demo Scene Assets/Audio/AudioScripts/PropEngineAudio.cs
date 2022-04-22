using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropEngineAudio : MonoBehaviour
{

    public GameObject idleSource;
    public GameObject humSource;
    public AnimationCurve idleVolume;
    public AnimationCurve humVolume;
    public AnimationCurve idlePitch;
    public AnimationCurve humPitch;

    Propulsion engine;
    AudioSource idleAudio;
    AudioSource humAudio;

    int previousState;
    
    void Start()
    {
        engine = GetComponent<Propulsion>();
        idleAudio = idleSource.GetComponent<AudioSource>();
        humAudio = humSource.GetComponent<AudioSource>();
        previousState = engine.state;
    }

    
    void Update()
    {
        switch(engine.state)
        {
            case 1:
                // play engine starting sound effects hear
                break;
            case 2:
                if (previousState != engine.state)
                {
                    idleAudio.Play();
                    humAudio.Play();
                }
                idleAudio.volume = idleVolume.Evaluate(engine.GetCurrentThrust);
                humAudio.volume = humVolume.Evaluate(engine.GetCurrentThrust);
                idleAudio.pitch = 1f + idlePitch.Evaluate(engine.GetCurrentThrust);
                humAudio.pitch = 1f + humPitch.Evaluate(engine.GetCurrentThrust);
                break;
            case 3:
                // play engine spooling down sound effects here
                break;
        }

        previousState = engine.state;

    }
}
