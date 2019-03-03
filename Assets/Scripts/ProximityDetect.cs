using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityDetect : MonoBehaviour
{
    #region Light Member Variables
    [Tooltip("Duration of light FX")]
    [Range(2, 10)] [SerializeField] float lightDuration = 5f;
    [SerializeField] float lightIntensity;
    [SerializeField] Light lightSource;
    [SerializeField] ParticleSystem candleFlicker;
    float lightIncrease;
    float lightDecrease;

    // starting value for the lerp
    static float t = 0.0f;
    static float r = 0.0f;
    float minimum = 0.0f;

    // Interpolate light color between two colors back and forth
    [SerializeField] float lightColorOscillationRate = 1.0f;
    [SerializeField] Color lightColor01;
    [SerializeField] Color lightColor02;
    #endregion

    // State Machine to manage light
    enum LightState {Idle, Increasing, Sustaining, Decreasing};
    private LightState currentState = LightState.Idle;

    float coolDownValue;
    float currCoolDownValue; // used for countdown and resetting lights & collision

    bool collisionsEnabled = true; // for debug code

    private GameObject childObject;
    private Animation anim;

    void Start()
    {
        // todo get light animation to work
        childObject = GameObject.Find("Light");
        anim = childObject.GetComponent<Animation>();
        print("childObject Name :" + childObject.name);
        print("anim name" + anim);

        // set light control values
        lightIncrease = lightDuration * 0.3f;
        lightDecrease = lightDuration * 0.5f;
        coolDownValue = lightDuration + lightIncrease + lightDecrease;
    }

    void Update()
    {

        if (!collisionsEnabled) // if collisions are disabled
        {
            if (1 < currCoolDownValue && currCoolDownValue < lightDecrease)
            {
                currentState = LightState.Decreasing;
            }

            if (currCoolDownValue < 1) // check if countdown timer is finished, re-enable
            {
                collisionsEnabled = !collisionsEnabled; // toggle collision enable/disable
                //lightSource.enabled = !lightSource.enabled; // turn lights on
                candleFlicker.Stop(); // discontinues particles
                // anim.Stop("LightFlutter"); // stop light flutter anim
                // currCoolDownValue = coolDownValue;
                currentState = LightState.Idle;
            }
        }
        SetLightColor();

        if (Input.GetKeyDown("p"))
        {
            print("currCoolDownValue = " + currCoolDownValue);
            print("CoolDownValue = " + coolDownValue);
            print("LightDuration = " + lightDuration);
            print("LightIncrease = " + lightIncrease);
            print("LightDecrease = " + lightDecrease);
            print("Value of 't' =" + t);
            print("Value of 'r' =" + r);
            //print("CurrentState = " + currentState);
        }
        print("CurrentState = " + currentState);

        switch (currentState)
        {
            case LightState.Idle:
                // do something
                lightSource.intensity = 0f;
                t = 0.0f;
                r = 0.0f;
                anim.Stop("LightFlutter"); // != todo Not currently working
                anim.Stop("GrassLightAnimation"); // != todo Not currently working
                break;
            case LightState.Increasing:
                // do something
                
                lightSource.intensity = Mathf.Lerp(minimum,lightIntensity,t);
                t += (lightIncrease*.1f) * Time.deltaTime;

                break;
            case LightState.Decreasing:
                // do something
                lightSource.intensity = Mathf.Lerp(lightIntensity, minimum, r);

                //if (t > 1.0f)
                //{
                //    float temp = lightIntensity;
                //    lightIntensity = minimum;
                //    minimum = temp;
                //    t = 0.0f;
                //}
                r += (lightDecrease*.1f) * Time.deltaTime;
                break;
            case LightState.Sustaining:
                // do something
                break;
        }

    }

    private IEnumerator StartCountdown(float coolDownValue)
    {
        currCoolDownValue = coolDownValue;
        while (currCoolDownValue > 0)
        {
            // Debug.Log("Countdown: " + currCoolDownValue);
            yield return new WaitForSeconds(1.0f);
            currCoolDownValue--;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (!collisionsEnabled) { return; } // if collisions are disabled, stop

        if (other.tag == "projectile")
        {
            //lightSource.enabled = !lightSource.enabled; // turn lights on
            anim.Play("LightFlutter"); // != todo Not currently working
            anim.Play("GrassLightAnimation"); // != todo Not currently working
            candleFlicker.Play(); // activate particles
            collisionsEnabled = !collisionsEnabled; // toggle collision enable/disable
            
            StartCoroutine(StartCountdown(coolDownValue)); // countdown to reset lights & collision

            currentState = LightState.Increasing;
        }
    }

    private void SetLightColor()
    {
        // set light color
        float t = Mathf.PingPong(Time.time, lightColorOscillationRate) / lightColorOscillationRate;
        lightSource.color = Color.Lerp(lightColor01, lightColor02, t);

    }
}
