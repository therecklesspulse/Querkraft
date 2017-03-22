using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInput : MonoBehaviour {

    // Attributes
    const float JUMP_FORCE = 10f;
    const float RUN_ACCEL = 10f;
    const float RUN_MAX_VEL = 6f;
    const float RUN_MIN_VEL = 0.5f;
    const float JUMP_ROLL_MARGIN = -5f;


    // SCRIPTS
    [SerializeField]
    Movement2D mov2D;
    [SerializeField]
    Weapon weapon;

    // Control
    float _cursorSensitivity = 1f;
    Vector3 cursor = Vector3.zero;
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
        GatherInput();
        UpdateJumpTarget();
        UpdateRolling();
    }

    void GatherInput()
    {
        // AIMING
        cursor = Input.mousePosition;
        cursor.z = Camera.main.transform.position.z;
        cursor = Camera.main.ScreenToWorldPoint(cursor);
        cursor.z = transform.position.z;
        aim = cursor - transform.position;
        aim.Normalize();

        Debug.DrawRay(transform.position, aim, Color.green);

        // MOVEMENT
        xAxis = 0;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            xAxis++;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.RightArrow))
            xAxis--;

        yAxis = 0;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            yAxis++;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            yAxis--;

        if(xAxis != 0 && mov2D.Grounded && !mov2D.GetRolling())
        {
            //mov2D.SetVelX(RUN_MAX_VEL * xAxis);
            if (Mathf.Abs(mov2D.GetVelocity().x) < RUN_MIN_VEL)
                mov2D.SetVelX(RUN_MIN_VEL * xAxis);
            //else
                //mov2D.SetVelX(Mathf.Clamp(mov2D.GetVelocity().x * RUN_ACCEL, -RUN_MAX_VEL, RUN_MAX_VEL));
            mov2D.SetAccel(Vector3.right * RUN_ACCEL * xAxis);
            mov2D.SetVelX(Mathf.Clamp(mov2D.GetVelocity().x, -RUN_MAX_VEL, RUN_MAX_VEL));
        }


        // JUMPING
        if (!doubleJump && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Z)))
        {
            float angle = Mathf.Asin(aim.y) * Mathf.Rad2Deg;
            if (angle < JUMP_ROLL_MARGIN)
                mov2D.SetRollIntent(true);

            if (mov2D.Grounded)
            {
                jump = true;
                if(!mov2D.GetRollIntent())
                    mov2D.SetVel((aim + Vector3.up).normalized * JUMP_FORCE);
                else
                    mov2D.SetVel(aim * JUMP_FORCE);
                /*if (xAxis != 0 && Mathf.Sign(jumpDir.x) == Mathf.Sign(mov2D.GetVelocity().x))
                    mov2D.SetVelY(JUMP_FORCE);
                else
                    mov2D.SetVel(jumpDir * JUMP_FORCE);*/
            }
            else if (!doubleJump)
            {
                doubleJump = true;
                //mov2D.SetVel(jumpDir * JUMP_FORCE);
                if (!mov2D.GetRollIntent())
                    mov2D.SetVel((aim + Vector3.up).normalized * JUMP_FORCE);
                else
                    mov2D.SetVel(aim * JUMP_FORCE);
            }
            else if(mov2D.WallLean != 0)
            {
                doubleJump = true;
                //mov2D.SetVel(jumpDir * JUMP_FORCE);
                if (!mov2D.GetRollIntent())
                    mov2D.SetVel((aim + Vector3.up + Vector3.right * mov2D.WallLean).normalized * JUMP_FORCE);
                else
                    mov2D.SetVel((aim + Vector3.right * mov2D.WallLean).normalized * JUMP_FORCE);
            }
        }
        //if (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(1) || Input.GetKeyUp(KeyCode.Z))
        //    mov2D.SetRollIntent(false);

        if (mov2D.Grounded)
        {
            jump = false;
            doubleJump = false;
        }

        // ATTACKING
        if (Input.GetMouseButton(0))
        {
            if (attackButtonRelease && weapon.AttackDown(aim) && weapon.IsMelee && weapon is WeaponStraight)
            {
                attackButtonRelease = false;
                mov2D.AddInstantTraslation(aim * 0.25f);
                //transform.position += aim * 0.25f;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            attackButtonRelease = true;
            if (weapon.AttackUp(aim) && weapon.IsMelee && weapon is WeaponCharged) {
                mov2D.AddInstantTraslation(aim * 0.25f);    
            }
        }
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
}
