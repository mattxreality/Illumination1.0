using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityDetect : MonoBehaviour
{
    [Tooltip("How fast to increase the light intensity")]
    [SerializeField] float increaseLight = 1f;

    [Tooltip("How fast to decrease the light intensity")]
    [SerializeField] float decreaseLight = 5f;

    [Tooltip("Distance from projectile to target that triggers action")]
    [SerializeField] float distanceTrigger = 10f;

    [SerializeField] Light lightSource;
    [SerializeField] ParticleSystem candleFlicker;

    // Interpolate light color between two colors back and forth
    [SerializeField] float flickerDuration = 1.0f;
    [SerializeField] Color lightColor01;
    [SerializeField] Color lightColor02;
    Color color0 = Color.red;
    Color color1 = Color.yellow;

    bool collisionsEnabled = true; // for debug code

    [SerializeField] float coolDownValue = 5f;
    float currCoolDownValue; // used for countdown and resetting lights & collision

    private GameObject childObject;
    private Animation anim;

    void Start()
    {
        childObject = GameObject.Find("AloeLight");
        anim = childObject.GetComponent<Animation>();
        print("childObject Name :" + childObject.name);
        print("anim name" + anim);
    }

    void Update()
    {
        /*psuedo code
         * once collision triggered, disable collision for duration of illumination
         * enable collision after illumination is finished
         */
        // print("coolDownValue " + coolDownValue);

        if (!collisionsEnabled) // if collisions are disabled
        {
            if (currCoolDownValue < 1) // check if countdown timer is finished, re-enable
            {
                collisionsEnabled = !collisionsEnabled; // toggle collision enable/disable
                lightSource.enabled = !lightSource.enabled; // turn lights on
                candleFlicker.Stop(); // discontinues particles
                anim.Stop("LightFlutter"); // stop light flutter anim
                currCoolDownValue = coolDownValue; 
            }
        }
        SetLightColor();
    }

    public IEnumerator StartCountdown(float coolDownValue)
    {
        currCoolDownValue = coolDownValue;
        while (currCoolDownValue > 0)
        {
            Debug.Log("Countdown: " + currCoolDownValue);
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
            // todo make light gradually turn on and off
            lightSource.enabled = !lightSource.enabled; // turn lights on
            candleFlicker.Play(); // activate particles
            collisionsEnabled = !collisionsEnabled; // toggle collision enable/disable
            StartCoroutine(StartCountdown(coolDownValue)); // countdown to reset lights & collision
            anim.Play("LightFlutter"); // != todo Not currently working
        }
    }

    private void SetLightColor()
    {
        // set light color
        float t = Mathf.PingPong(Time.time, flickerDuration) / flickerDuration;
        lightSource.color = Color.Lerp(lightColor01, lightColor02, t);

    }
}
