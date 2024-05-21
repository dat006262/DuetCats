using BulletHell;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class IceCreamEmmiter : ProjectileEmitterBase
{
    protected const string hautoID = "BALL";
    ColorPulse StaticOutlinePulse;
    ColorPulse StaticPulse;

    [Foldout("Appearance", true)]
    [SerializeField] public bool UseColorPulse;
    [ConditionalField(nameof(UseColorPulse)), SerializeField] protected float PulseSpeed;
    [ConditionalField(nameof(UseColorPulse)), SerializeField] protected bool UseStaticPulse;


    [Foldout("Modifiers", true)]
    [SerializeField] public bool UseFollowTarget;
    [ConditionalField(nameof(UseFollowTarget))] public Transform Target;
    // [ConditionalField(nameof(UseFollowTarget))] public FollowTargetType FollowTargetType = FollowTargetType.Homing;
    [ConditionalField(nameof(UseFollowTarget)), Range(0, 5)] public float FollowIntensity;

    [Foldout("Outline", true)]
    [SerializeField] protected bool UseOutlineColorPulse;
    [ConditionalField(nameof(UseOutlineColorPulse)), SerializeField] protected float OutlinePulseSpeed;
    [ConditionalField(nameof(UseOutlineColorPulse)), SerializeField] protected bool UseOutlineStaticPulse;

    private EmitterGroup[] Groups;
    private int LastGroupCountPoll = -1;
    private bool PreviousMirrorPairRotation = false;
    private bool PreviousPairGroupDirection = false;
    [Foldout("Spokes", true)]
    [Range(0, 360), SerializeField] protected float DirecBase = 0;
    [Range(1, 10), SerializeField] protected int GroupCount = 1;
    [Range(0, 1), SerializeField] protected float GroupSpacing = 1;
    [Range(1, 10), SerializeField] protected int SpokeCount = 3;
    [Range(0, 100), SerializeField] protected float SpokeSpacing = 25;
    [SerializeField] protected bool MirrorPairRotation;
    [/*ConditionalField(nameof(MirrorPairRotation)),*/ SerializeField] protected bool PairGroupDirection;

    public new void Awake()
    {
        base.Awake();

        Groups = new EmitterGroup[10];
        RefreshGroups();
    }

    void Start()
    {
        // To allow for the enable / disable checkbox in Inspector
    }

    private void RefreshGroups()
    {
        if (GroupCount > 10)
        {
            Debug.Log("Max Group Count is set to 10.  You attempted to set it to " + GroupCount.ToString() + ".");
            return;
        }

        bool mirror = false;
        if (Groups == null || LastGroupCountPoll != GroupCount || PreviousMirrorPairRotation != MirrorPairRotation || PreviousPairGroupDirection != PairGroupDirection)
        {
            // Refresh the groups, they were changed
            float rotation = DirecBase;
            for (int n = 0; n < Groups.Length; n++)
            {
                if (n < GroupCount && Groups[n] == null)
                {
                    Groups[n] = new EmitterGroup(Rotate(Direction, rotation).normalized, SpokeCount, SpokeSpacing, mirror);
                }
                else if (n < GroupCount)
                {
                    Groups[n].Set(Rotate(Direction, rotation).normalized, SpokeCount, SpokeSpacing, mirror);
                }
                else
                {
                    //n is greater than GroupCount -- ensure we clear the rest of the buffer
                    Groups[n] = null;
                }

                // invert the mirror flag if needed
                if (MirrorPairRotation)
                    mirror = !mirror;

                // sets the starting direction of all the groups so we divide by 360 to evenly distribute their direction
                // Could reduce the scope of the directions here
                rotation = CalculateGroupRotation(n, rotation);
            }
            LastGroupCountPoll = GroupCount;
            PreviousMirrorPairRotation = MirrorPairRotation;
            PreviousPairGroupDirection = PairGroupDirection;
        }
        else if (RotationSpeed == 0)
        {
            float rotation = DirecBase;
            // If rotation speed is locked, then allow to update Direction of groups
            for (int n = 0; n < Groups.Length; n++)
            {
                if (Groups[n] != null)
                {
                    Groups[n].Direction = Rotate(Direction, rotation).normalized;
                }

                rotation = CalculateGroupRotation(n, rotation);
            }
        }
    }

    public override Pool<ProjectileData>.Node FireProjectile(Vector2 direction, float leakedTime)
    {
        Pool<ProjectileData>.Node node = new Pool<ProjectileData>.Node();
        Direction = direction;
        RefreshGroups();

        if (!AutoFire)
        {
            if (Interval > 0) return node;
            else Interval = CoolOffTime;
        }

        for (int g = 0; g < GroupCount; g++)
        {
            if (Projectiles.AvailableCount >= SpokeCount)
            {
                float rotation = 0;
                bool indexSplit2 = true;

                for (int n = 0; n < SpokeCount; n++)
                {
                    SpawnOnePrijectTile();

                }

                if (Groups[g].InvertRotation)
                    Groups[g].Direction = Rotate(Groups[g].Direction, -RotationSpeed);
                else
                    Groups[g].Direction = Rotate(Groups[g].Direction, RotationSpeed);
            }
        }

        return node;
    }
    protected override void UpdateProjectile(ref Pool<ProjectileData>.Node node, float tick)
    {

        if (node.Active)
        {

            node.Item.TimeToLive -= tick;

            // Projectile is active
            if (node.Item.TimeToLive > 0 || TimeToLive < -10)
            {
                UpdateProjectileNodePulse(tick, ref node.Item);



                // calculate where projectile will be at the end of this frame
                Vector2 deltaPosition = node.Item.Velocity * tick;
                float distance = deltaPosition.magnitude;

                // If flag set - return projectiles that are no longer in view 
                if (CullProjectilesOutsideCameraBounds)
                {
                    Bounds bounds = new Bounds(node.Item.Position, new Vector3(node.Item.Scale, node.Item.Scale, node.Item.Scale));
                    if (!GeometryUtility.TestPlanesAABB(Planes, bounds))
                    {
                        ReturnNode(node);
                        return;
                    }
                }
                float angle = Vector2.Angle(Vector2.up, node.Item.Velocity);

                float radius;
                radius = node.Item.Scale / 2f;
                if (node.Item.Position.y < -5.5f)
                {
                    GameManager.Instance.EndGame();
                    Debug.Log("Loss");
                }
                int result = -1;
                if (CollisionDetection == CollisionDetectionType.Raycast)
                {
                    result = Physics2D.Raycast(node.Item.Position, deltaPosition, ContactFilter, RaycastHitBuffer, distance);
                }
                else if (CollisionDetection == CollisionDetectionType.CircleCast)
                {
                    result = Physics2D.CircleCast(node.Item.Position, radius, deltaPosition, ContactFilter, RaycastHitBuffer, distance);
                }

                if (result > 0)
                {

                    // Put whatever hit code you want here such as damage events

                    // Collision was detected, should we bounce off or destroy the projectile?
                    if (BounceOffSurfaces)
                    {
                        //var element = RaycastHitBuffer[0];
                        //var enemy = element.transform.GetComponent<Enemy>();
                        //HitDameEnemy(enemy, 65f, node.NodeIndex, hautoID, 0.25f);


                        //Vector2 projectedNewPosition = node.Item.Position + (deltaPosition * RaycastHitBuffer[0].fraction);
                        //Vector2 directionOfHitFromCenter = RaycastHitBuffer[0].point - projectedNewPosition;
                        //float distanceToContact = (RaycastHitBuffer[0].point - projectedNewPosition).magnitude;
                        //float remainder = radius - distanceToContact;

                        //// reposition projectile to the point of impact 
                        //node.Item.Position = projectedNewPosition - (directionOfHitFromCenter.normalized * remainder);

                        //// reflect the velocity for a bounce effect -- will work well on static surfaces
                        //node.Item.Velocity = Vector2.Reflect(node.Item.Velocity, RaycastHitBuffer[0].normal);

                        //// calculate remaining distance after bounce
                        //deltaPosition = node.Item.Velocity * tick * (1 - RaycastHitBuffer[0].fraction);

                        //// When gravity is applied, the positional change here is actually parabolic
                        //node.Item.Position += deltaPosition;

                        //// Absorbs energy from bounce
                        //node.Item.Velocity = new Vector2(node.Item.Velocity.x * (1 - BounceAbsorbtionX), node.Item.Velocity.y * (1 - BounceAbsorbtionY));




                    }
                    else
                    {//IF DONT BOUNCY
                        //Debug.Log(Mathf.Abs(node.Item.Position.x - RaycastHitBuffer[0].transform.position.x));
                        Animator animator = RaycastHitBuffer[0].transform.GetChild(0).GetComponent<Animator>();
                        if (animator != null)
                        {
                            AnimatorStateInfo currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
                            if (currentStateInfo.normalizedTime < 1)
                            {
                            }
                            else
                            {
                                animator.Play("Base Layer.test", 0, 0);
                            }

                        }


                        float check = Mathf.Abs(node.Item.Position.x - RaycastHitBuffer[0].transform.position.x);
                        if (check > 1)
                        {
                            DamagePanel.Instance.SpawnDamageShow(node.Item.Position, "Cool");
                        }
                        else if (check > 0.3f)
                        {
                            DamagePanel.Instance.SpawnDamageShow(node.Item.Position, "Good");
                        }
                        else
                        {
                            DamagePanel.Instance.SpawnDamageShow(node.Item.Position, "Perfect");
                        }
                        ReturnNode(node);


                    }
                }
                else
                {
                    //No collision -move projectile
                    node.Item.Position += deltaPosition;
                    UpdateProjectileColor(ref node.Item);

                    // Update outline position
                    if (node.Item.Outline.Item != null)
                    {
                        node.Item.Outline.Item.Position = node.Item.Position;
                    }
                }
            }
            else
            {
                // End of life - return to pool
                ReturnNode(node);
            }
        }
    }
    public void SpawnOnePrijectTile()
    {
        Pool<ProjectileData>.Node node = new Pool<ProjectileData>.Node();
        node = Projectiles.Get();
        node.Item.Color = Color;
        node.Item.Position = this.transform.position;
        node.Item.Rotation = 0f;
        node.Item.NeedToReturnNode = false;
        node.Item.Speed = Speed;
        node.Item.Scale = Scale;
        node.Item.TimeToLive = TimeToLive;
        node.Item.Velocity = Speed * Vector3.down;
        //node.Item.Gravity = Gravity;

        //  node.Item.Color = Color.Evaluate(0);
        PreviousActiveProjectileIndexes[ActiveProjectileIndexesPosition] = node.NodeIndex;
        ActiveProjectileIndexesPosition++;
        if (ActiveProjectileIndexesPosition < ActiveProjectileIndexes.Length)
        {
            PreviousActiveProjectileIndexes[ActiveProjectileIndexesPosition] = -1;
        }
        else
        {
            Debug.Log("Error: Projectile was fired before list of active projectiles was refreshed.");
        }
        // UpdateProjectile(ref node, leakedTime);
    }
    public void ChangDirecBase(Vector2 direc)
    {
        float angle = Vector2.Angle(Vector2.up, direc);

        if (direc.x > 0)
        {
            DirecBase = 360 - angle;
        }
        else
        {
            DirecBase = angle;
        }

    }
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, Scale);

        Gizmos.color = UnityEngine.Color.yellow;

        float rotation = 0;

        for (int n = 0; n < GroupCount; n++)
        {
            rotation = CalculateGroupRotation(n, rotation);
            Vector2 direction = Rotate(Direction, rotation).normalized * (Scale + 0.2f);
            Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y) + direction);


        }

        Gizmos.color = UnityEngine.Color.red;
        rotation = 0;
        float spokeRotation = 0;
        bool left = true;
        for (int n = 0; n < GroupCount; n++)
        {
            rotation = CalculateGroupRotation(n, rotation);
            Vector2 groupDirection = Rotate(Direction, rotation).normalized;
            spokeRotation = 0;
            left = true;

            for (int m = 0; m < SpokeCount; m++)
            {
                Vector2 direction = Vector2.zero;
                if (left)
                {
                    direction = Rotate(groupDirection, spokeRotation).normalized * (Scale + 0.15f);
                    spokeRotation += SpokeSpacing;
                }
                else
                {
                    direction = Rotate(groupDirection, -spokeRotation).normalized * (Scale + 0.15f);
                }
                Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y) + direction);

                left = !left;
            }


        }
    }

    private float CalculateGroupRotation(int index, float currentRotation)
    {
        if (PairGroupDirection)
        {
            if (index % 2 == 1)
            {
                currentRotation += 360 * GroupSpacing * 2f / GroupCount;
                currentRotation += DirecBase;
            }

        }
        else
        {
            currentRotation += 360 * GroupSpacing / GroupCount;
            currentRotation += DirecBase;
        }
        return currentRotation;
    }

    protected override void UpdateProjectiles(float tick)
    {
        UpdateStaticPulses(tick);
        base.UpdateProjectiles(tick);
    }


    private void UpdateProjectileNodePulse(float tick, ref ProjectileData data)
    {

    }

    private void UpdateStaticPulses(float tick)
    {
        //projectile pulse
        if (UseColorPulse && UseStaticPulse)
        {
            StaticPulse.Update(tick, PulseSpeed);
        }

        //outline pulse
        if (UseOutlineColorPulse && UseOutlineStaticPulse)
        {
            StaticOutlinePulse.Update(tick, OutlinePulseSpeed);
        }
    }

    protected override void UpdateProjectileColor(ref ProjectileData data)
    {
        // foreground

        // data.Color = Color.Evaluate(1 - data.TimeToLive / TimeToLive);


        //outline

    }

}

