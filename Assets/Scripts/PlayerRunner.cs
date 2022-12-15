using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]

public class PlayerRunner : MonoBehaviour
{
    public AudioSource jmpSound;
    public AudioSource hitSound;
    public float gravity = 20.0f;
    public float jumpHeight = 2.5f;

    Rigidbody r;
    bool isGrounded = false;
    Vector3 defaultScale;
    bool isCrouched = false;

 
    void Start()
    {
        r = GetComponent<Rigidbody>();
        r.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ; // Constrain this RigidBody's X and Z position i.e. only allow Y positional movement
        r.freezeRotation = true; // This RigidBody doesn't need to rotate
        r.useGravity = false;
        defaultScale = transform.localScale;
        Time.timeScale = 0f;
    }

    // Consider using the new input system in newer builds
    void Update()
    {
        // Keep rollin, rollin, rollin...
        transform.Rotate(150f * Time.deltaTime, 0f, 0f, Space.Self);
        
        
        
        // Jumping
        if ((Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.W)) && isGrounded && !GroundGenerator.instance.gameOver && Time.timeScale > 0f)
        {
            r.velocity = new Vector3(r.velocity.x, CalculateJumpVerticalSpeed(), r.velocity.z);
            PlayJumpSoundEffect();
        }

        // Crouching
        isCrouched = Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.S);
        if (isCrouched)
        {
            // Squish the player by scaling down Y a little
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(defaultScale.x, defaultScale.y * 0.4f, defaultScale.z), Time.deltaTime * 7);
        }

        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, defaultScale, Time.deltaTime * 7);
        }
    }

    private void FixedUpdate()
    {
        r.AddForce(new Vector3(0, -gravity * r.mass, 0)); // Manually applying gravity for more fine tuned control
        isGrounded = false;
    }

    private void OnCollisionStay()
    {
        isGrounded = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Finish")
        {
            // Print game over message
            GroundGenerator.instance.gameOver = true;
            PlayHitSoundEffect();
        }
    }

    private float CalculateJumpVerticalSpeed()
    {
        /* From the jump height and gravity, deduce the upwards speed
         * for the player character to reach at the apex */
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

    public void PlayJumpSoundEffect()
    {
        jmpSound.Play();
    }

    public void PlayHitSoundEffect()
    {
        hitSound.Play();
    }
}
