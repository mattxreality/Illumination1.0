using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Purpose of Script:
 * On Collision, activate particlesystem and light
 * 
 * Usage:
 * 1) Create an empty GameObject
 * 2) (opt) add a 3d object as child
 * 3) Add gameObject ParticleSystem as child
 * 4) Add gameObject Light as child
 * 5) assign script to empty gameObject
 * 6) Drag light and particle system to inspector
 * 7) Save as prefab
 * 
 * -Mattxreality 3/4/2019
 * 
 * Todo Ideas:
 * 1) Instantiate lights and particle systems as needed. 
 * Don't have as part of prefab.
 */

 class ProximityEffect01 : MonoBehaviour
{
    #region Light Member Variables
    [Tooltip("Duration of light FX")]
    [Range(2, 10)] [SerializeField] float lightDuration = 5f;
    [SerializeField] float lightIntensity;
    [SerializeField] Light lightSource;
    [SerializeField] ParticleSystem plantParticle;
    private float lightIncrease;
    private float lightDecrease;

    // Interpolate light color between two colors back and forth
    [SerializeField] float lightColorOscillationRate = 1.0f;
    [SerializeField] Color lightColor01;
    [SerializeField] Color lightColor02;
    #endregion

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
        lightSource.enabled = !lightSource.enabled; // turn lights on
    }

    private void Update()
    {
        SetLightColor();
        DebugMyStuff();
        CheckColliderStatus();

    }

    private void CheckColliderStatus()
    {
        if (!collisionsEnabled) // if collisions are disabled
        {
            if (currCoolDownValue < 1) // check if countdown timer is finished, re-enable
            {
                ToggleComponentEnable();
                plantParticle.Stop(); // discontinues particles
                //m_Animator.Stop("LightFlutter"); // != todo Not currently working
            }
        }
    }

    private void ToggleComponentEnable()
    {
        lightSource.enabled = !lightSource.enabled; // turn lights on
        collisionsEnabled = !collisionsEnabled; // toggle collision to 'disable'
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!collisionsEnabled) { return; } // if collisions are disabled, stop processing

        if (other.tag == "projectile")
        {
            plantParticle.Play(); // activate particles
            ToggleComponentEnable();
            StartCoroutine(StartCountdown(coolDownValue)); // countdown to reset lights & collision
        }
    }

    private void SetLightColor()
    {
        // set light color
        float t = Mathf.PingPong(Time.time, lightColorOscillationRate) / lightColorOscillationRate;
        lightSource.color = Color.Lerp(lightColor01, lightColor02, t);
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

    private void DebugMyStuff()
    {
        if (Input.GetKeyDown("p"))
        {
            // press 'P' to see values for the following
            print("currCoolDownValue = " + currCoolDownValue);
            print("CoolDownValue = " + coolDownValue);
            print("LightDuration = " + lightDuration);
        }
    }
}
