using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MyCharacterController : MonoBehaviour
{
    // create a fields for Rigidbody
    private Rigidbody capsuleRB;

    // create fields for input 
    private PlayerInputActions playerInputActions;
    private InputAction moveAction;
    private Animator animator;

    // create movement fields

    [Header("Required Components")]
    [SerializeField] Camera playerCamera;

    [Header("Current Move Forces")]
    [SerializeField] public float walkForce = 1f;
    [SerializeField] public float runForce = 3f;
    [SerializeField] public float maxSpeed = 4.0f;
    public Vector3 forceDirection = Vector3.zero;
    public float moveAmount;

    public float rotationDamping = 20.0F;

    // Stance stuff - 01-04-22


    public enum CharacterStance { Standing, Crouched, Prone }
    [Header("Stance Values")]
    [SerializeField] public CharacterStance _stance;

    [Header("Standing Values |      X: walk force, Y: run force, Z: MaxSpeed")]
    [SerializeField] private Vector3 _standingSpeed = new Vector3(0, 0, 0);
    [Header("Crouched Values |      X: walk force, Y: run force, Z: MaxSpeed")]
    [SerializeField] private Vector3 _crouchedSpeed = new Vector3(0, 0, 0);
    [Header("Prone Values |         X: walk force, Y: run force, Z: MaxSpeed")]
    [SerializeField] private Vector3 _proneSpeed = new Vector3(0, 0, 0);

    [Header("Standing Capsule |      X = Radius, Y= Height, Z = YOffset")]
    [SerializeField] private Vector3 _standingCapsule = Vector3.zero;
    [Header("Crouched Capsule |      X = Radius, Y= Height, Z = YOffset")]
    [SerializeField] private Vector3 _crouchedCapsule = Vector3.zero;
    [Header("Prone Capsule |         X = Radius, Y= Height, Z = YOffset")]
    [SerializeField] private Vector3 _proneCapsule = Vector3.zero;

    [Header("Colliders & Masks")]
    public CapsuleCollider _collider;
    public SphereCollider sphereCollider;
    public Collider floorCollider;

    private Collider[] _obstructions = new Collider[8]; // This 8 is an random number but it should be atleast 2, 1 for the player 1 for an obstruction
    public Collider[] platformColliders;

    private LayerMask _layerMask;

    // Stance State Names
    private const string _standToProne = "Base Layer.Stand to Prone";
    private const string _standToCrouch = "Base Layer.Base Crouching";
    private const string _crouchToProne = "Base Layer.Crouch to Prone";
    private const string _crouchToStand = "Base Layer.Base Standing";
    private const string _proneToStand = "Base Layer.Prone to Stand";
    private const string _proneToCrouch = "Base Layer.Prone to Crouch";


    // end of Stance stuff
    [Header("Enviromnent Variables")]
    public bool onMovingPlatform = false;

    //Combat
    [Header("Combat Variables")]
    public bool isAttacking = false;

    [SerializeField] public GameObject leftArm;
    [SerializeField] public CapsuleCollider leftArmCollider;


    [Header("Jump Variables")]
    [SerializeField] public float jumpForce = 10.0F;
    [SerializeField] public bool canDoubleJump = false;
    [SerializeField] public float jumpCooldownTime = 1.0F;
    [SerializeField] public bool jumpCooldown = false;
    [SerializeField] public bool hasJumped = false;
    [SerializeField] public bool hasDoubleJumped = false;
    [SerializeField] public bool landedJump = false;
    [SerializeField] public bool grounded = false;


    Ray groundedRay;
    RaycastHit groundHitInfo;
    public float groundAngle;
    public float maxGroundAngle = 130;
    public Vector3 characterForward;
    public bool sliding = false;

    [Header("Input Action Variables")]
    [SerializeField] public bool triggerHeld = false;

    [Header("Spline Variables")]
    [SerializeField] public float splineSpeed = 10;
    [SerializeField] public bool onSpline = false;
    [SerializeField] public bool jumpedOnSpline = false;
    private SplineFollower follower;
    public float pathDistanceTravelled = 0;


    #region - Awake/Start -
    private void Start()
    {
        // Stance stuff


        // Defaults
        SetCapsuleDimensions(_standingCapsule);
        walkForce = _standingSpeed.x;
        runForce = _standingSpeed.y;
        maxSpeed = _standingSpeed.z;
        _stance = CharacterStance.Standing;

        // iterate through the max number of unity layers (32) and check if the layer is ignored
        // if the mask is not ignored then flag that layer
        int _mask = 0;
        for (int i = 0; i < 32; i++)
        {
            if (!(Physics.GetIgnoreLayerCollision(gameObject.layer, i)))
            {
                _mask |= 1 << i;
            }
        }
        _layerMask = _mask;


        // end of Stance stuff
    }

    private void Awake()
    {
        // get the component - of this object instance - of the type rigidbody and assign it to our local ref
        capsuleRB = this.GetComponent<Rigidbody>();

        playerInputActions = new PlayerInputActions();

        animator = this.GetComponent<Animator>();

        playerInputActions.ThirdPersonPlayer.Trigger.performed += DoTrigger;
        playerInputActions.ThirdPersonPlayer.Trigger.canceled += DoTrigger;

        follower = this.GetComponent<SplineFollower>();
        _collider = this.GetComponent<CapsuleCollider>();
        sphereCollider = this.GetComponent<SphereCollider>();


    }


    #endregion

    #region - Enable/Disable -
    private void OnEnable()
    {
        playerInputActions.ThirdPersonPlayer.Jump.started += DoJump;
        playerInputActions.ThirdPersonPlayer.Attack.started += DoAttack;
        moveAction = playerInputActions.ThirdPersonPlayer.Movement;
        playerInputActions.ThirdPersonPlayer.Trigger.performed += DoTrigger;
        playerInputActions.ThirdPersonPlayer.Trigger.canceled += DoTrigger;
        playerInputActions.ThirdPersonPlayer.Stance.started += ChangeStance;
        playerInputActions.ThirdPersonPlayer.Stance.performed += ChangeStance;
        playerInputActions.ThirdPersonPlayer.Stance.canceled += ChangeStance;
        playerInputActions.ThirdPersonPlayer.Enable();

    }

    private void onDisable()
    {
        playerInputActions.ThirdPersonPlayer.Jump.started -= DoJump;
        playerInputActions.ThirdPersonPlayer.Attack.started -= DoAttack;
        playerInputActions.ThirdPersonPlayer.Trigger.performed -= DoTrigger;
        playerInputActions.ThirdPersonPlayer.Trigger.canceled -= DoTrigger;
        playerInputActions.ThirdPersonPlayer.Stance.started -= ChangeStance;
        playerInputActions.ThirdPersonPlayer.Stance.performed -= ChangeStance;
        playerInputActions.ThirdPersonPlayer.Stance.canceled -= ChangeStance;

        playerInputActions.ThirdPersonPlayer.Disable();

    }

    #endregion

    #region - Collisions -

    private void OnTriggerEnter(Collider other)
    {
        if (isAttacking)
        {
            if (other.CompareTag("Enemy"))
            {
                other.GetComponent<EnemyAINavMesh3D>().TakeDamage(45);
            }
        }
    }

    #endregion

    #region - Events -

    public void StartMeleeAttacking()
    {
        isAttacking = true;
    }

    public void FinishMeleeAttacking()
    {
        isAttacking = false;
    }

    #endregion

    #region - Updates - 
    private void LateUpdate()
    {

    }

    private void FixedUpdate()
    {
        moveAmount = Mathf.Clamp01(Mathf.Abs(moveAction.ReadValue<Vector2>().x) + Mathf.Abs(moveAction.ReadValue<Vector2>().y));
        IsGrounded();
        CalculateCharacterForward();
        CalculateGroundAngle();
        CheckJumpStatus();
        HandleSplineFollower();

        if (onSpline)
        {
            if (!follower.marioAutoRunStyle)
            {
                SplineMove();
            }
        }
        else
        {
            if (groundAngle < maxGroundAngle)
            {
                Move();
            }
            else
            {
                handleGravity();
                /*                if (sliding)
                                {
                                    if (characterForward.y > 0.05f)
                                    {
                                        //then make character face opposite direction/down slope
                                    }
                                }
                */
            }
        }
        DrawDebugLines();
    }

    private void HandleSplineFollower()
    {

        if (follower.marioAutoRunStyle)
        {
            if (jumpedOnSpline)
            {
                if (landedJump && follower.currentPath == null)
                {
                    follower.currentPath = follower.previousPath;

                }
            }
        }

        if (follower.onSpline)
        {
            onSpline = true;
        }
        else
        {
            onSpline = false;
        }
    }

    private void CheckJumpStatus()
    {
        if (!jumpCooldown)
        {
            if (hasJumped && grounded)
            {
                if (hasDoubleJumped)
                {
                    hasDoubleJumped = false;
                }
                landedJump = true;
                hasJumped = false;
            }
        }
    }

    #endregion

    #region - Movement -
    private void SplineMove()
    {
        /*        float distanceTravelledInJump = 0;
                if (hasJumped)
                {

                    distanceTravelledInJump += moveAction.ReadValue<Vector2>().x / 10;
                    if (landedJump)
                    {
                        follower.distanceTravelled += distanceTravelledInJump;
                    }
                }*/



        if (moveAction.ReadValue<Vector2>().x != 0)
        {
            follower.distanceTravelled += moveAction.ReadValue<Vector2>().x * splineSpeed * Time.fixedDeltaTime;
        }

    }

    private void Move()
    {
        if (!isAttacking)
        {
            if (moveAmount >= 0.5F)
            {
                forceDirection += moveAction.ReadValue<Vector2>().x * GetCameraRight(playerCamera) * runForce;
                forceDirection += moveAction.ReadValue<Vector2>().y * GetCameraForward(playerCamera) * runForce;
            }
            else
            {
                forceDirection += moveAction.ReadValue<Vector2>().x * GetCameraRight(playerCamera) * walkForce;
                forceDirection += moveAction.ReadValue<Vector2>().y * GetCameraForward(playerCamera) * walkForce;
            }

            //        capsuleRB.velocity = forceDirection * 2.5f;
            capsuleRB.AddForce(forceDirection, ForceMode.Impulse);
        }
        forceDirection = Vector3.zero;

        handleGravity();

        // Look at function controls the direction of our rb character
        LookAt();
    }

    private void LookAt()
    {
        Vector3 direction = capsuleRB.velocity;
        direction.y = 0f;

        // Check if we the player is giving input & character is are moving
        if (moveAction.ReadValue<Vector2>().sqrMagnitude > 0.05F && direction.sqrMagnitude > 0.1F)
        {// if true then change the direction the character is looking
         // this.capsuleRB.rotation = Quaternion.LookRotation(direction, Vector3.up);

            var rotation = Quaternion.LookRotation(direction, Vector3.up);
            this.capsuleRB.rotation = Quaternion.Slerp(this.capsuleRB.rotation, rotation, Time.deltaTime * rotationDamping);

        }
        else
            capsuleRB.angularVelocity = Vector3.zero;
    }

    // GetCamera Functions
    // The reason we use these functions instead of just
    // using transform.forward or transform.right
    // is because the camera likely not placed exactly in the horizontal plane
    // which we want to move our character on
    private Vector3 GetCameraForward(Camera playerCamera)
    {
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }
    private Vector3 GetCameraRight(Camera playerCamera)
    {
        Vector3 right = playerCamera.transform.right;
        right.y = 0;
        return right.normalized;
    }

    private void handleGravity()
    {
        // create gravity accerlation 
        if (capsuleRB.velocity.y < 0f)
            capsuleRB.velocity -= Physics.gravity.y * Time.fixedDeltaTime * Vector3.down;

        // create a cap for the players speed but on the horizontal plane (so gravity still increases)
        Vector3 horizontalVelocity = capsuleRB.velocity;
        horizontalVelocity.y = 0;
        // check if we have exceeded our maxspeed variable
        if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
            //if true, we set the horizontal speed to be the max and keep the current velocity in the y axis
            capsuleRB.velocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * capsuleRB.velocity.y;
    }

    #endregion

    #region - DoActions - 
    private void DoJump(InputAction.CallbackContext obj)
    {

        // Jump Button Pressed Logs
        Debug.Log("doJump!");
        if (hasJumped && !canDoubleJump)
        {
            Debug.Log("In Air - Can't Double Jump!");
        }

        // Jump Function 
        if (groundAngle < maxGroundAngle)
        {
            if (grounded)
            {
                if (follower.marioAutoRunStyle)
                {
                    if (follower.currentPath != null)
                    {
                        follower.previousPath = follower.currentPath;
                        follower.currentPath = null;
                        jumpedOnSpline = true;
                    }
                }

                else if (follower.currentPath != null)
                {
                    follower.previousPath = follower.currentPath;
                    follower.currentPath = null;
                    jumpedOnSpline = true;
                    follower.onSpline = false;
                }

                forceDirection += Vector3.up * jumpForce;
                hasJumped = true;
                landedJump = false;
                jumpCooldown = true;
                StartCoroutine(jumpCooldownTimer(jumpCooldownTime));
            }
        }
        else if (hasJumped && canDoubleJump)
        {
            Debug.Log("In Air - Performing Double Jump!");
            forceDirection += Vector3.up * jumpForce;
            hasDoubleJumped = true; // Unused bool currently, but will be used for animation
            canDoubleJump = false;
            landedJump = false;
        }
    }

    private void DoAttack(InputAction.CallbackContext obj)
    {
        if (!isAttacking)
        {
            StartMeleeAttacking();
            animator.SetTrigger("MeleeSwipe");
        }
    }



    private void DoTrigger(InputAction.CallbackContext obj)
    {
        triggerHeld = obj.ReadValueAsButton();
        Debug.Log("Trigger Pressed");
    }



    #endregion

    #region - Utility -

    private bool IsGrounded()
    {
        // define a new ray at with -
        // origin = slightly above the characters feet to ensure we cast above whatever surface the player is on
        // direction = down (4Head)
        groundedRay = new Ray(this.transform.position + Vector3.up * 0.25F, Vector3.down);

        Debug.DrawLine(groundedRay.origin, groundedRay.origin + Vector3.down * 0.5f, Color.red, 2f);

        if (Physics.Raycast(groundedRay, out groundHitInfo, 1f))
        {
            floorCollider = groundHitInfo.collider;
            Debug.Log("IS GROUNDED!");
            grounded = true;
            return true;
        }
        else
        {
            Debug.Log("NOT GROUNDED!");
            grounded = false;
            return false;
        }
    }

    public void CalculateGroundAngle()
    {
        if (!grounded)
        {
            groundAngle = 90;
            return;
        }

        groundAngle = Vector3.Angle(groundHitInfo.normal, transform.forward);

        if (groundAngle > 100 || groundAngle < 80)
        {
            sliding = true;
        }
        else
        {
            sliding = false;
        }
    }

    public void CalculateCharacterForward()
    {
        if (!grounded)
        {
            characterForward = transform.forward;
            return;
        }

        characterForward = Vector3.Cross(transform.right, groundHitInfo.normal);

    }

    IEnumerator jumpCooldownTimer(float time)
    {
        yield return new WaitForSeconds(time);
        Debug.Log("Jump Cooldown, Now check if Grounded!");
        jumpCooldown = false;
    }


    public void increaseMaxSpeed(float newSpeed, float time)
    {
        Debug.Log("increasing max speed!");
        maxSpeed += newSpeed;
        StartCoroutine(SpeedBuffTime(newSpeed, time));

    }
    IEnumerator SpeedBuffTime(float newSpeed, float time)
    {
        yield return new WaitForSeconds(time);
        Debug.Log("Resetting Max Speed!");
        maxSpeed -= newSpeed;
    }

    #endregion

    #region - Stance -

    private void ChangeStance(InputAction.CallbackContext obj)
    {

        if (obj.performed)
        {
            switch (_stance)
            {
                case CharacterStance.Standing:
                    if (obj.duration >= 1)
                    {
                        // standing + long press = prone
                        RequestStanceChange(CharacterStance.Prone);
                    }
                    else
                    {
                        // standing + fast press = crouched
                        RequestStanceChange(CharacterStance.Crouched);
                    }
                    break;
                case CharacterStance.Crouched:
                    if (obj.duration >= 1)
                    {
                        // crouched + long = prone
                        RequestStanceChange(CharacterStance.Prone);
                    }
                    else
                    {
                        // crouched + fast = standing
                        RequestStanceChange(CharacterStance.Standing);
                    }
                    break;
                case CharacterStance.Prone:
                    if (obj.duration >= 1)
                    {
                        // prone + long = standing
                        RequestStanceChange(CharacterStance.Standing);
                    }
                    else
                    {
                        // prone + fast = crouched
                        RequestStanceChange(CharacterStance.Crouched);
                    }
                    break;
            }
        }
    }




    public bool RequestStanceChange(CharacterStance newStance)
    {
        if (_stance == newStance)
        {
            Debug.Log("Stance = " + _stance);
            return true;
        }
        switch (_stance)
        {
            case CharacterStance.Standing:
                if (newStance == CharacterStance.Crouched)
                {
                    if (!CharacterOverlap(_crouchedCapsule))
                    {
                        walkForce = _crouchedSpeed.x;
                        runForce = _crouchedSpeed.y;
                        maxSpeed = _crouchedSpeed.z;
                        _stance = newStance;
                        animator.CrossFadeInFixedTime(_standToCrouch, 0.25F);
                        SetCapsuleDimensions(_crouchedCapsule);
                        sphereCollider.enabled = false;
                        Debug.Log("Stance changed = " + _stance);
                        return true;
                    }
                }
                else if (newStance == CharacterStance.Prone)
                {
                    if (!CharacterOverlap(_proneCapsule))
                    {
                        walkForce = _proneSpeed.x;
                        runForce = _proneSpeed.y;
                        maxSpeed = _proneSpeed.z;
                        _stance = newStance;
                        animator.CrossFadeInFixedTime(_standToProne, 0.5F);
                        SetCapsuleDimensions(_proneCapsule);
                        sphereCollider.enabled = false;
                        Debug.Log("Stance changed = " + _stance);
                        return true;
                    }
                }
                break;
            case CharacterStance.Crouched:
                if (newStance == CharacterStance.Standing)
                {
                    if (!CharacterOverlap(_standingCapsule))
                    {
                        walkForce = _standingSpeed.x;
                        runForce = _standingSpeed.y;
                        maxSpeed = _standingSpeed.z;
                        _stance = newStance;
                        animator.CrossFadeInFixedTime(_crouchToStand, 0.25F);
                        SetCapsuleDimensions(_standingCapsule);
                        sphereCollider.enabled = true;
                        Debug.Log("Stance changed = " + _stance);
                        return true;
                    }
                }
                else if (newStance == CharacterStance.Prone)
                {
                    if (!CharacterOverlap(_proneCapsule))
                    {
                        walkForce = _proneSpeed.x;
                        runForce = _proneSpeed.y;
                        maxSpeed = _proneSpeed.z;
                        _stance = newStance;
                        animator.CrossFadeInFixedTime(_crouchToProne, 0.25F);
                        SetCapsuleDimensions(_proneCapsule);
                        Debug.Log("Stance changed = " + _stance);
                        return true;
                    }
                }
                break;
            case CharacterStance.Prone:
                if (newStance == CharacterStance.Standing)
                {
                    if (!CharacterOverlap(_standingCapsule))
                    {
                        walkForce = _standingSpeed.x;
                        runForce = _standingSpeed.y;
                        maxSpeed = _standingSpeed.z;
                        _stance = newStance;
                        animator.CrossFadeInFixedTime(_proneToStand, 0.5F);
                        SetCapsuleDimensions(_standingCapsule);
                        sphereCollider.enabled = true;
                        Debug.Log("Stance changed = " + _stance);
                        return true;
                    }
                }
                else if (newStance == CharacterStance.Crouched)
                {
                    if (!CharacterOverlap(_crouchedCapsule))
                    {
                        walkForce = _crouchedSpeed.x;
                        runForce = _crouchedSpeed.y;
                        maxSpeed = _crouchedSpeed.z;
                        _stance = newStance;
                        animator.CrossFadeInFixedTime(_proneToCrouch, 0.25F);
                        SetCapsuleDimensions(_crouchedCapsule);
                        return true;
                    }
                }
                break;
        }
        return false;
    }

    // This function checks if there is enough space when you want to change the capsule colliders size,
    // it takes a Vector 3 that represents a Capsules desired Dimensions and checks if it overlaps with any flagged masks
    private bool CharacterOverlap(Vector3 dimensions)
    {

        //Check to see if grounded to ensure ground collider is referenced
        IsGrounded();
        Debug.Log("Checking CharacterOverlap");
        float _radius = dimensions.x;
        float _height = dimensions.y;
        // the center is the centre of the current capsule minus the offset of the capsule being passed in
        Vector3 _centre = new Vector3(_collider.center.x, dimensions.z, _collider.center.z);

        Vector3 _point0;
        Vector3 _point1;

        if (_height < _radius * 2)
        {
            _point0 = transform.position + _centre;
            _point1 = transform.position - _centre;

        }
        else
        {
            _point0 = transform.position + _centre + (transform.up * (_height * 0.5f - _radius));
            _point1 = transform.position + _centre - (transform.up * (_height * 0.5f - _radius));

        }

        // Use this Physics Overlap Non-allocating function to determine the amount of overlaps found in the capsule
        // It takes, point 0 & 1 (representing the two ends of the capsule), the radius, a buffer to store the detected overlaps and a layer mask 
        int _numOverlaps = Physics.OverlapCapsuleNonAlloc(_point0, _point1, _radius, _obstructions, _layerMask);

        Debug.Log("Begin Loop - Number of CharacterOverlaps = " + _numOverlaps);
        for (int i = 0; i < _numOverlaps + 1; i++) // +1 to numOverlaps to ensure when we subtract 1 we stil check the last overlap and remove if needed
        {
            Debug.Log("Checking Overlap = " + _obstructions[i].name + ", Number of CharacterOverlaps = " + _numOverlaps);
            if (_obstructions[i] == _collider || _obstructions[i] == sphereCollider || _obstructions[i] == capsuleRB || _obstructions[i] == floorCollider)
            {
                Debug.Log("Removing Overlap = " + _obstructions[i].name + ", Number of CharacterOverlaps = " + _numOverlaps);
                Debug.Log("Name = " + _obstructions[i].name + " | Type = " + _obstructions[i].GetType() + " | Layer = " + _obstructions[i].gameObject.layer);
                _numOverlaps--;
                i = 0;
                Debug.Log("Removed Overlap = " + _obstructions[i].name + ", Number of CharacterOverlaps = " + _numOverlaps);

            }
        }

        // Handle moving platform colliders
        if (onMovingPlatform)
        {
            Debug.Log("Getting Parent Box Colliders and removing from _numOverlaps");
            foreach (BoxCollider collider in this.GetComponentsInParent<BoxCollider>())
            {
                for (int j = 0; j < _numOverlaps; j++)
                {
                    _numOverlaps--;
                    Debug.Log("Removed Overlap = " + collider.name + ", Number of CharacterOverlaps = " + _numOverlaps);
                }
            }
        }

        Debug.Log("End Number of CharacterOverlaps = " + _numOverlaps);


        // Optional Debugging 
        if (_numOverlaps != 0)
        {
            for (int i = 0; i < _numOverlaps; i++)
            {
                Debug.Log(_obstructions[i]);
                Debug.Log("Name = " + _obstructions[i].name);
                Debug.Log("Type = " + _obstructions[i].GetType());
                Debug.Log("Layer = " + _obstructions[i].gameObject.layer);
            }
        }

        Debug.Log("End of CharacterOverlap, Number of CharacterOverlaps = " + _numOverlaps);
        return _numOverlaps > 0;

    }

    private void SetCapsuleDimensions(Vector3 newDimensions)
    {
        Debug.Log("Setting Capsule Dimensions");
        _collider.center = new Vector3(_collider.center.x, newDimensions.z, _collider.center.z);
        _collider.radius = newDimensions.x;
        _collider.height = newDimensions.y;
        Debug.Log("Capsule Dimensions Set!");
    }

    #endregion


    #region - Debug - 

    void DrawDebugLines()
    {

        Debug.DrawLine(transform.position + Vector3.up * 0.25F, this.transform.position + Vector3.up * 0.25F + Vector3.down * 0.5f, Color.red, 2f);
        Debug.DrawLine(this.transform.position + Vector3.up * 1, transform.position + Vector3.up * 1 + characterForward * 0.5f * 2, Color.blue);
    }

    #endregion
}
