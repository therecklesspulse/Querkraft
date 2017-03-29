using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement2D : MonoBehaviour {

    // Constants
    const float GRAVITY_VALUE = -19.81f;
    Vector3 GRAVITY = new Vector3(0f, GRAVITY_VALUE, 0f);
    const float FRICTION = 1f;
    const float WALL_LEAN_ANGLE = 22f;

    // Dynamics
    [SerializeField]
    Vector3 acceleration = Vector3.zero;
    [SerializeField]
    Vector3 velocity = Vector3.zero;
    Vector3 traslation = Vector3.zero;
    Vector3 instantTraslation = Vector3.zero;
    Vector3 overridingVelocity = Vector3.zero;

    // Shape
    [SerializeField]
    Vector3 size = new Vector3(1f,1f,0f);
    float SKIN_WIDTH = 0.01f;
    int _horRaysNum = 5;
    int _verRaysNum = 5;

    struct Bounds
    {
        public Vector3 topBound, botBound, rightBound, leftBound;
        public Vector3 topLeft, topRight, botLeft, botRight;
    }

    Bounds bounds;

    LayerMask collisionMask;

    // Control
    [SerializeField]
    float slopeAngle = 0f;
    public bool Grounded = false;
    public int WallLean = 0;
    bool vColl = false;
    bool hColl = false;
    [SerializeField]
    bool rollIntent = false;
    bool rolling = false;

    // Special Collisions
    [SerializeField]
    Character character;


	// Use this for initialization
	void Start ()
    {
        InitializeBounds();
        collisionMask = LayerMask.GetMask("Level", "Player", "Enemy", "Obstacle");
        if (!character)
            character = GetComponent<Character>();
	}


    void InitializeBounds()
    {
        bounds.topBound = new Vector3(0f, size.y / 2f - SKIN_WIDTH, 0f);
        bounds.botBound = new Vector3(0f, -size.y / 2f + SKIN_WIDTH, 0f);
        bounds.leftBound = new Vector3(-size.x / 2f + SKIN_WIDTH, 0f, 0f);
        bounds.rightBound = new Vector3(size.x / 2f - SKIN_WIDTH, 0f, 0f);

        bounds.topLeft = new Vector3(-size.x / 2f + SKIN_WIDTH, size.y / 2f - SKIN_WIDTH, 0f);
        bounds.topRight = new Vector3(size.x / 2f - SKIN_WIDTH, size.y / 2f - SKIN_WIDTH, 0f);
        bounds.botLeft = new Vector3(-size.x / 2f + SKIN_WIDTH, -size.y / 2f + SKIN_WIDTH, 0f);
        bounds.botRight = new Vector3(size.x / 2f - SKIN_WIDTH, -size.y / 2f + SKIN_WIDTH, 0f);
    }








	// Update is called once per frame
	void Update ()
    {
        CalculateDynamics();

        HorizontalCollisions();
        VerticalCollisions();

        Traslate();
        FixVelocity();
        if(Grounded && !rollIntent && !rolling && (Mathf.Abs(acceleration.x) < 0.01f || Mathf.Sign(acceleration.x) != Mathf.Sign(velocity.x)))
            ApplyFriction();
        acceleration = Vector3.zero;
        instantTraslation = Vector3.zero;
	}

    void CalculateDynamics()
    {
        acceleration += GRAVITY;
        velocity += acceleration * Time.deltaTime;
        traslation = velocity * Time.deltaTime;
        traslation += instantTraslation;
    }

    void ApplyFriction()
    {
        velocity.x /= 1+FRICTION;
    }

    void HorizontalCollisions()
    {
        float directionX = Mathf.Sign(traslation.x);
        //Vector3 rayOrigin = transform.position + (directionX > 0 ? bounds.botRight : bounds.botLeft);
        Vector3 rayDirection = Vector2.right * directionX;
        float rayLength = Mathf.Abs(traslation.x) + SKIN_WIDTH;

        if(Mathf.Abs(traslation.x) > 0.01f)
            WallLean = 0;
        hColl = false;

        for (int i = 0; i < _horRaysNum; i++)
        {
            Vector3 rayOrigin = transform.position + (directionX > 0 ? bounds.botRight : bounds.botLeft) + new Vector3(0f, size.y/(_horRaysNum-1)*i, 0f);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, collisionMask);
            if (hit)
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Level"))
                {
                    traslation.x = (hit.distance - SKIN_WIDTH) * directionX;
                    rayLength = hit.distance;

                    //if(Mathf.Abs(Vector3.Angle(hit.normal, Vector3.up)) < WALL_LEAN_ANGLE)
                    WallLean = Mathf.FloorToInt(Mathf.Sign(hit.normal.x));

                    hColl = true;
                }
                else
                {
                    character.SpecialCollisions(hit.collider.gameObject);
                    rayLength = hit.distance;
                }
            }
            Debug.DrawRay(rayOrigin, rayDirection, Color.red);
        }
        //ClimbSlope
    }

    void VerticalCollisions()
    {
        float directionY = Mathf.Sign(traslation.y);
        //Vector3 rayOrigin = transform.position + (directionY > 0 ? bounds.topBound : bounds.botBound);
        Vector3 rayDirection = Vector2.up * directionY;
        float rayLength = Mathf.Abs(traslation.y) + SKIN_WIDTH;

        Grounded = false;
        vColl = false;

        for (int i = 0; i < _verRaysNum; i++)
        {
            Vector3 rayOrigin = transform.position + (directionY > 0 ? bounds.topLeft : bounds.botLeft) + new Vector3(size.x/(_verRaysNum-1)*i,0f,0f);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, collisionMask);
            if (hit)
            {
                //if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Level"))
                //{
                    traslation.y = (hit.distance - SKIN_WIDTH) * directionY;
                    rayLength = hit.distance;

                    if (directionY <= 0)
                        Grounded = true;
                    vColl = true;
                    slopeAngle = Vector3.Angle(hit.normal, Vector3.up) * Mathf.Sign(hit.normal.x);
                //}
                //else
                if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Level"))
                {
                    character.SpecialCollisions(hit.collider.gameObject);
                    rayLength = hit.distance;
                }
            }

            Debug.DrawRay(rayOrigin, rayDirection, Color.red);
        }
        //DescendSlope
    }

    void Traslate()
    {
        transform.position += traslation;
    }

    void FixVelocity()
    {
        //traslation -= instantTraslation;
        if (!hColl)
            traslation.x -= instantTraslation.x;
        if (!vColl)
            traslation.y -= instantTraslation.y;
        if (overridingVelocity == Vector3.zero)
            velocity = traslation / Time.deltaTime;
        else
            velocity = overridingVelocity;
        overridingVelocity = Vector3.zero;
    }


    // INPUT
    public void SetVelX(float vx)
    {
        velocity.x = vx;
    }
    public void SetVelY(float vy)
    {
        velocity.y = vy;
    }
    public void SetVel(Vector3 vel)
    {
        velocity = vel;
    }
    public void AddVel(Vector3 vel)
    {
        velocity += vel;
    }
    public void AddAccelX(float ax)
    {
        acceleration.x += ax;
    }
    public void AddAccelY(float ay)
    {
        acceleration.y += ay;
    }
    public void AddAccel(Vector3 accel)
    {
        acceleration += accel;
    }
    public void SetAccel(Vector3 accel)
    {
        acceleration = accel;
    }
    public void SetRolling(bool value)
    {
        rolling = value;
    }
    public void SetRollIntent(bool value)
    {
        rollIntent = value;
    }
    public void AddInstantTraslation(Vector3 itras)
    {
        instantTraslation += itras;
    }
    public void SetOverridingVel(Vector3 ovVel)
    {
        overridingVelocity = ovVel;
    }
    public void AddOverridingVel(Vector3 ovVel)
    {
        overridingVelocity += ovVel;
    }

    // PhysicGetters
    public Vector3 GetVelocity()
    {
        return velocity;
    }
    public bool GetRolling()
    {
        return rolling;
    }
    public bool GetRollIntent()
    {
        return rollIntent;
    }
}
