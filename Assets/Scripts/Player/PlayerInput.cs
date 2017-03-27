using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    enum ControlMode
    {
        keyboard,
        gamepad
    }
    ControlMode controlMode = ControlMode.keyboard;

    // Attributes
    public const float JUMP_FORCE = 10f;
    public const float RUN_ACCEL = 10f;
    public const float RUN_MAX_VEL = 6f;
    const float RUN_MIN_VEL = 0.5f;
    const float JUMP_ROLL_MARGIN = -5f;
    const float ATTACK_DISPLACEMENT = 0.2f;


    // SCRIPTS
    [SerializeField]
    Movement2D mov2D;
    [SerializeField]
    Weapon weapon;

    // Control
    [SerializeField]
    Vector3 aim = Vector3.up;
    //Vector3 lastCursor = Vector3.zero;

    float xAxis = 0;
    float yAxis = 0;

    bool jump = false;
    bool doubleJump = false;

    [SerializeField]
    float rollTimer = 0f;
    float ROLL_TIMER = 0.33f;

    bool attackButtonRelease = true;

    //UI
    [SerializeField]
    Transform jumpTarget;

	// Use this for initialization
	void Start () {
        Cursor.lockState = CursorLockMode.Confined;
        if (!mov2D)
            mov2D = GetComponent<Movement2D>();
        if (weapon == null)
            weapon = GetComponentInChildren<Weapon>();
        if (!jumpTarget)
            jumpTarget = GameObject.Find("JumpTarget").transform;
	}
	
	// Update is called once per frame
	void Update () {
        PlayerInputs();
        UpdateJumpTarget();
        UpdateRolling();
    }

    void PlayerInputs()
    {
        // AIMING
        AssignAim();
        Debug.DrawRay(transform.position, aim, Color.green);

        // MOVEMENT
        AssignMovementAxis();
        Running();

        // JUMPING
        Jumping();

        // ATTACKING
        Attacking();
    }

    void AssignAim()
    {
        Vector3 cursor = Input.mousePosition;
        cursor.z = Camera.main.transform.position.z;
        cursor = Camera.main.ScreenToWorldPoint(cursor);
        cursor.z = transform.position.z;
        Vector3 newAim = cursor - transform.position;
        newAim.Normalize();

        /*Vector3 newAim = new Vector3(xAxis, yAxis, 0f).normalized;
        if (newAim != Vector3.zero)
            aim = newAim;*/

        aim = newAim;
    }

    void AssignMovementAxis()
    {
        if (controlMode == ControlMode.keyboard)
        {
            xAxis = 0;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                xAxis++;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                xAxis--;

            yAxis = 0;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                yAxis++;

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                yAxis--;
        }
        else if (controlMode == ControlMode.gamepad)
        {
            xAxis = Input.GetAxis("X");
            yAxis = Input.GetAxis("Y");
        }
    }

    void Running()
    {
        if (xAxis != 0 && mov2D.Grounded && !mov2D.GetRolling())
        {
            //mov2D.SetVelX(RUN_MAX_VEL * xAxis);
            if (Mathf.Abs(mov2D.GetVelocity().x) < RUN_MIN_VEL)
                mov2D.SetVelX(RUN_MIN_VEL * Mathf.Sign(xAxis));
            //else
            //mov2D.SetVelX(Mathf.Clamp(mov2D.GetVelocity().x * RUN_ACCEL, -RUN_MAX_VEL, RUN_MAX_VEL));
            mov2D.SetAccel(Vector3.right * RUN_ACCEL * Mathf.Sign(xAxis));
            mov2D.SetVelX(Mathf.Clamp(mov2D.GetVelocity().x, -RUN_MAX_VEL, RUN_MAX_VEL));
        }
    }

    void Jumping()
    {
        if (mov2D.Grounded)
        {
            jump = false;
            doubleJump = false;
        }

        if (!doubleJump && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.X)))
        {
            Vector3 jumpDir = GetJumpDir();

            CheckRollIntent(jumpDir.y);

            Vector3 residualVelocity = GetResidualVelocity(jumpDir);

            if (mov2D.Grounded)
            {
                Jump(jumpDir, residualVelocity);
            }
            else if (!doubleJump)
            {
                DoubleJump(jumpDir, residualVelocity);
            }
            else if (mov2D.WallLean != 0)
            {
                doubleJump = true;
                //mov2D.SetVel(jumpDir * JUMP_FORCE);
                if (!mov2D.GetRollIntent())
                    mov2D.SetVel((jumpDir + Vector3.up + Vector3.right * mov2D.WallLean).normalized * JUMP_FORCE);
                else
                    mov2D.SetVel((jumpDir + Vector3.right * mov2D.WallLean).normalized * JUMP_FORCE);
            }
        }
    }

    Vector3 GetJumpDir()
    {
        Vector3 jumpDir = new Vector3(xAxis, yAxis, 0f);
        jumpDir.Normalize();
        if (jumpDir == Vector3.zero)
            jumpDir = Vector3.up;
        return jumpDir;
    }

    void CheckRollIntent(float jumpDirY)
    {
        float angle = Mathf.Asin(jumpDirY) * Mathf.Rad2Deg;
        if (angle < JUMP_ROLL_MARGIN)
            mov2D.SetRollIntent(true);
        //if (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(1) || Input.GetKeyUp(KeyCode.Z))
        //    mov2D.SetRollIntent(false);
    }

    Vector3 GetResidualVelocity(Vector3 jumpDir)
    {
        Vector3 residualVelocity = mov2D.GetVelocity() / 3f;
        if (jumpDir.y > 0f)
            residualVelocity.y = 0f;
        if (jumpDir.x != 0 && Mathf.Sign(jumpDir.x) == Mathf.Sign(residualVelocity.x))
            residualVelocity.x = 0f;
        return residualVelocity;
    }

    void Jump(Vector3 jumpDir, Vector3 residualVelocity)
    {
        jump = true;
        if (!mov2D.GetRollIntent())
            mov2D.SetVel(residualVelocity + (jumpDir + Vector3.up).normalized * JUMP_FORCE);
        else
            mov2D.SetVel(residualVelocity + jumpDir * JUMP_FORCE);
        /*if (xAxis != 0 && Mathf.Sign(jumpDir.x) == Mathf.Sign(mov2D.GetVelocity().x))
            mov2D.SetVelY(JUMP_FORCE);
        else
            mov2D.SetVel(jumpDir * JUMP_FORCE);*/
    }

    void DoubleJump(Vector3 jumpDir, Vector3 residualVelocity)
    {
        doubleJump = true;

        //mov2D.SetVel(jumpDir * JUMP_FORCE);
        if (!mov2D.GetRollIntent())
            mov2D.SetVel(residualVelocity + (jumpDir + Vector3.up).normalized * JUMP_FORCE);
        else
            mov2D.SetVel(residualVelocity + jumpDir * JUMP_FORCE);
    }

    void UpdateJumpTarget()
    {
        jumpTarget.localPosition = aim * 0.33f;
        //jumpTarget.eulerAngles = new Vector3(0f, 0f, (Mathf.Acos(jumpDir.x) - Mathf.Asin(jumpDir.y)) * Mathf.Rad2Deg);
        //jumpTarget.Rotate(0f, 0f, 90f);
    }

    void UpdateRolling()
    {
        if (mov2D.GetRollIntent() && mov2D.Grounded) {
            mov2D.SetRollIntent(false);
            mov2D.SetRolling(true);
            rollTimer = 0f;
        }

        if (mov2D.GetRolling())
        {
            if (rollTimer < ROLL_TIMER)
            {
                rollTimer += Time.deltaTime;
                transform.eulerAngles += new Vector3(0f, 0f, -1090f * Time.deltaTime * Mathf.Sign(mov2D.GetVelocity().x));
            }
            else
            {
                rollTimer = 0f;
                mov2D.SetRolling(false);
                transform.eulerAngles = Vector3.zero;
            }
        }
    }

    void Attacking()
    {
        if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.C))
        {
            if (attackButtonRelease && weapon.AttackDown(aim) && weapon.IsMelee && weapon is WeaponStraight)
            {
                attackButtonRelease = false;
                mov2D.AddInstantTraslation(aim * ATTACK_DISPLACEMENT);
                //transform.position += aim * 0.25f;
            }
        }
        if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.C))
        {
            attackButtonRelease = true;
            if (weapon.AttackUp(aim) && weapon.IsMelee && weapon is WeaponCharged)
            {
                mov2D.AddInstantTraslation(aim * ATTACK_DISPLACEMENT);
            }
        }
    }
}
