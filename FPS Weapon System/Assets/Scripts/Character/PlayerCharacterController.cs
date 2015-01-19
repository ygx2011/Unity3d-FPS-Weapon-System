#pragma warning disable 414

using UnityEngine;
using System.Collections;

[AddComponentMenu("Player Character/First Person Controller")]
[RequireComponent(typeof(CharacterController))]
public class PlayerCharacterController : MonoBehaviour {

    public bool controllable = false;
    public bool canRun = true;
    public bool canJump = true;

    public float walkSpeed = 4.0f;
    public float runSpeed = 7f;
    public float crouchSpeed = 2f;

    // If true, diagonal speed (when strafing + moving forward or back) can't exceed normal move speed;
    // otherwise it's about 1.4 times faster
    public bool limitDiagonalSpeed = true;

    // If checked, the run key toggles between running and walking. Otherwise player runs
    // if the key is held down and walks otherwise
    // There must be a button set up in the Input Manager called "Run"
    // public bool toggleRun = false;

    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    // Units that player can fall before a falling damage function is run. To disable, type "infinity" in the inspector
    public float fallingDamageThreshold = 10.0f;

    // If the player ends up on a slope which is at least the Slope Limit as
    // set on the character controller, then he will slide down
    public bool slideWhenOverSlopeLimit = false;

    // Uncomment this line if you want to use sliding functionality
    /*
    // If checked and the player is on an object tagged "Slide", he will slide
    // down it regardless of the slope limit
    // public bool slideOnTaggedObjects = false;

    // public float slideSpeed = 2.0f;
    */

    // If checked, then the player can change direction while in the air
    public bool airControl = false;

    // Small amounts of this results in bumping when walking down slopes, but large amounts results in falling too fast
    public float antiBumpFactor = .75f;

    // Player must be grounded for at least this many physics frames before
    // being able to jump again; set to 0 to allow bunny hopping
    public int antiBunnyHopFactor = 1;

    private Vector3 moveDirection = Vector3.zero;
    private bool grounded = false;
    private CharacterController controller;
    private CapsuleCollider capsule;
    private Transform mainCameraTransform;
    private Transform myTransform;
    private float speed;
    private RaycastHit hit;
    private float fallStartLevel;
    private bool falling;
    private float slideLimit;
    private float rayDistance;
    private Vector3 contactPoint;
    private int jumpTimer;
    private bool running = false;
    private bool crouching = false;
    private bool canUncrouch = false;
    private float height;
    private Vector3 crouchCenter = new Vector3(0, -.5f, 0);
    private Vector3 cameraCrouchPosition = new Vector3(0, .1f, 0);
    private Vector3 cameraPosition;
    private RaycastHit crouchHit;

    public void Awake()
    {
        mainCameraTransform = Camera.main.transform;
        cameraPosition = mainCameraTransform.localPosition;
        
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        capsule = GetComponent<CapsuleCollider>();
        myTransform = transform;
        speed = walkSpeed;
        rayDistance = controller.height * .5f + controller.radius;
        slideLimit = controller.slopeLimit - .1f;
        jumpTimer = antiBunnyHopFactor;
        height = controller.height;
    }

    void FixedUpdate()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        // If both horizontal and vertical are used simultaneously, limit speed (if allowed),
        // so the total doesn't exceed normal move speed
        float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && limitDiagonalSpeed) ? .7071f : 1.0f;

        if (grounded)
        {
            bool sliding = false;
            // See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
            // because that interferes with step climbing amongst other annoyances
            if (Physics.Raycast(myTransform.position, -Vector3.up, out hit, rayDistance))
            {
                if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                    sliding = true;
            }
            // However, just raycasting straight down from the center can fail when on steep slopes
            // So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
            else
            {
                Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, out hit);
                if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                    sliding = true;
            }

            // If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
            if (falling)
            {
                falling = false;
                if (myTransform.position.y < fallStartLevel - fallingDamageThreshold)
                    FallingDamageAlert(fallStartLevel - myTransform.position.y);
            }

            // If running isn't on a toggle, then use the appropriate speed depending on whether the run button is down
            if (/*!toggleRun &&*/ canRun)
                speed = Input.GetButton("Run") ? runSpeed : walkSpeed;
            // If sliding (and it's allowed), or if we're on an object tagged "Slide",
            // get a vector pointing down the slope we're on
            if ((sliding && slideWhenOverSlopeLimit)) 
                /* to slide on surfaces tagged "Slide" attach this condition to this If statement.
                 * || (slideOnTaggedObjects && hit.collider.tag == "Slide"))
                 */
            {
                Vector3 hitNormal = hit.normal;
                moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                Vector3.OrthoNormalize(ref hitNormal, ref moveDirection);
                // Uncomment this line if you use sliding functionality
                // moveDirection *= slideSpeed;
                controllable = false;
            }
            // Otherwise recalculate moveDirection directly from axes, adding a bit of -y to avoid bumping down inclines
            else
            {
                moveDirection = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
                moveDirection = myTransform.TransformDirection(moveDirection) * speed;
                controllable = true;
            }

            // Jump only if canJump is checked
            canRun = canJump = !crouching;
            if (canJump)
            {
                // Jump! But only if the jump button has been released and player has been grounded for a given number of frames
                if (!Input.GetButton("Jump"))
                    jumpTimer++;
                else if (jumpTimer >= antiBunnyHopFactor)
                {
                    moveDirection.y = jumpSpeed;
                    jumpTimer = 0;
                } 
            }

            // Crouch
            // Check whether player can Uncrouch in his current position
            canUncrouch = CanUncrouch();
            if (Input.GetButton("Crouch"))
            {
                Crouch();
            }
            else
                Uncrouch();
            
        }
        else
        {
            // If we stepped over a cliff or something, set the height at which we started falling
            if (!falling)
            {
                falling = true;
                fallStartLevel = myTransform.position.y;
            }

            // If air control is allowed, check movement but don't touch the y component
            if (airControl && controllable)
            {
                moveDirection.x = inputX * speed * inputModifyFactor;
                moveDirection.z = inputY * speed * inputModifyFactor;
                moveDirection = myTransform.TransformDirection(moveDirection);
            }
        }

        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller, and set grounded true or false depending on whether we're standing on something
        grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
    }

    private void Crouch()
    {
        crouching = true;
        var crouchHeight = height * .5f;
        controller.height = crouchHeight;
        capsule.height = crouchHeight;
        controller.center = crouchCenter;
        capsule.center = crouchCenter;
        mainCameraTransform.localPosition = Vector3.Lerp(mainCameraTransform.localPosition, cameraCrouchPosition, Time.deltaTime * 10f);
        speed = crouchSpeed;

    }

    private void Uncrouch()
    {
        if (!CanUncrouch())
            return;
        crouching = false;
        controller.height = height;
        capsule.height = height;
        controller.center = Vector3.zero;
        capsule.center = Vector3.zero;
        mainCameraTransform.localPosition = Vector3.Lerp(mainCameraTransform.localPosition, cameraPosition, Time.deltaTime * 10f);
        speed = walkSpeed;
    }

    private bool CanUncrouch()
    {
        bool _canUncrouch = true;
        Debug.DrawRay(transform.position, Vector3.up * 1f, Color.red);
        if (Physics.Raycast(transform.position, Vector3.up, out crouchHit, 1f, Physics.AllLayers))
        {
            if (Vector3.Distance(crouchHit.point, hit.point) < height)
                _canUncrouch = false;
        }
        return _canUncrouch;
    }

    void Update()
    {
        // If the run button is set to toggle, then switch between walk/run speed. (We use Update for this...
        // FixedUpdate is a poor place to use GetButtonDown, since it doesn't necessarily run every frame and can miss the event)
        /*
         * if (canRun && toggleRun && grounded && Input.GetButtonDown("Run"))
            speed = (speed == walkSpeed ? runSpeed : walkSpeed);
         * 
         */
    }

    // Store point that we're in contact with for use in FixedUpdate if needed
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        contactPoint = hit.point;
    }

    // If falling damage occured, this is the place to do something about it. You can make the player
    // have hitpoints and remove some of them based on the distance fallen, add sound effects, etc.
    void FallingDamageAlert(float fallDistance)
    {
        print("Ouch! Fell " + fallDistance + " units!");
    }
}
