using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script works flawlessly for a single object,
 * but when I add a second prefab with the same script
 * they interfere with each others light controls. The
 * particle system Play and Stop works ok for all, so
 * I know the problem is in the logic for controlling 
 * the lights. 
 * 
 * The light logic is designed to do three things:
 * 1) increase lights quickly from 0 to Intensity value
 * 2) keep the light on for the duration of 'Light Duration'
 * 3) slowly turn light Intensity back to 0.
 * Mattxreality 3/3/2019
 * 
 * Ideas:
 * 1) Instantiate lights and particle systems as needed. 
 * Don't have as part of prefab.
 */

 class ProximityDetect : MonoBehaviour
{
    #region Light Member Variables
    [Tooltip("Duration of light FX")]
    [Range(2, 10)] [SerializeField] float lightDuration = 5f;
    [SerializeField] float lightIntensity;
    [SerializeField] Light lightSource;
    [SerializeField] ParticleSystem plantParticle;
    private float lightIncrease;
    private float lightDecrease;

    // starting value for the lerp
    private static float t = 0.0f;
    private static float r = 0.0f;
    private float minimum = 0.0f;

    // Interpolate light color between two colors back and forth
    [SerializeField] float lightColorOscillationRate = 1.0f;
    [SerializeField] Color lightColor01;
    [SerializeField] Color lightColor02;
    #endregion

    // State Machine to manage light
    private enum LightState {Idle, Increasing, Sustaining, Decreasing};
    private LightState currentState = LightState.Idle;

    private float coolDownValue;
    private float currCoolDownValue; // used for countdown and resetting lights & collision

    private bool collisionsEnabled = true; // for debug code

    private GameObject childObject;
    private Animation m_Animator;

    private void Start()
    {
        // todo get light animation to work
        Light childObject = gameObject.GetComponentInChildren(typeof(Light)) as Light;  // GameObject.Find("Light");
        m_Animator = childObject.GetComponent<Animation>();

        print("childObject Name :" + childObject.name);
        print("anim name" + m_Animator);

        // set light control values
        lightIncrease = lightDuration * 0.3f;
        lightDecrease = lightDuration * 0.5f;
        coolDownValue = lightDuration + lightIncrease + lightDecrease;
    }

    private void Update()
    {
        SetLightColor();
        DebugMyStuff();

        if (!collisionsEnabled) // if collisions are disabled
        {
            if (1 < currCoolDownValue && currCoolDownValue < lightDecrease)
            {
                currentState = LightState.Decreasing;
            }

            if (currCoolDownValue < 1) // check if countdown timer is finished, re-enable
            {
                collisionsEnabled = !collisionsEnabled; // toggle collision enable/disable
                lightSource.enabled = !lightSource.enabled; // turn lights on
                plantParticle.Stop(); // discontinues particles
                currentState = LightState.Idle;
            }
        }

        switch (currentState)
        {
            case LightState.Idle:
                // reset values to original state
                //lightSource.intensity = 0f;
                t = 0.0f;
                r = 0.0f;
                m_Animator.Stop("LightFlutter"); // != todo Not currently working
                m_Animator.Stop("GrassLightAnimation"); // != todo Not currently working
                break;
            case LightState.Increasing:
                // increase light intensity to 'lightIntensity'
                //lightSource.intensity = Mathf.Lerp(minimum, lightIntensity, t);
                //t += (lightIncrease * .1f) * Time.deltaTime;
                break;
            case LightState.Decreasing:
                // reduce light intensity to zero
                //lightSource.intensity = Mathf.Lerp(lightIntensity, minimum, r);
                //r += (lightDecrease * .1f) * Time.deltaTime;
                break;
        }
    }

    private void DebugMyStuff()
    {
        if (Input.GetKeyDown("p"))
        {
            // press 'P' to see values for the following
            print("currCoolDownValue = " + currCoolDownValue);
            print("CoolDownValue = " + coolDownValue);
            print("LightDuration = " + lightDuration);
            print("LightIncrease = " + lightIncrease);
            print("LightDecrease = " + lightDecrease);
            print("Value of 't' =" + t);
            print("Value of 'r' =" + r);
            print("CurrentState = " + currentState);
        }
    }

    private IEnumerator StartCountdown(float coolDownValue)
    {
        // counts down based on 'Light Duration" value
        currCoolDownValue = coolDownValue;
        while (currCoolDownValue > 0)
        {
            // Debug.Log("Countdown: " + currCoolDownValue);
            yield return new WaitForSeconds(1.0f);
            currCoolDownValue--;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!collisionsEnabled) { return; } // if collisions are disabled, stop processing

        if (other.tag == "projectile")
        {
            // todo index animation and use number. Name is too explicit
            m_Animator.Play("LightFlutter");
            
            plantParticle.Play(); // activate particles
            collisionsEnabled = !collisionsEnabled; // toggle collision to 'disable'
            lightSource.enabled = !lightSource.enabled; // turn lights on
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
