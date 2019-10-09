using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using MC.Players.Constants;
using MC.Utils;
using UnityEngine.Events;
using Object = UnityEngine.Object;
using System.Linq;
using MC.Players;

public enum DirectionEnum
{
	Front,
	Right,
	Left,
	Rear
}

public enum DriftState
{
	NotDrifting,
	FacingLeft,
	FacingRight
}

namespace MC.Karts{

    /// <summary>
    /// ジェネリックを隠すために継承してしまう
    /// [System.Serializable]を書くのを忘れない
    /// </summary>
    [System.Serializable]
    public class RayOriginTable : Serialize.TableBase<DirectionEnum, Transform, RayOriginPair>
    {                                                                                                                                                       

    }

    /// <summary>
    /// ジェネリックを隠すために継承してしまう
    /// [System.Serializable]を書くのを忘れない
    /// </summary>
    [System.Serializable]
    public class RayOriginPair : Serialize.KeyAndValue<DirectionEnum, Transform>
    {

        public RayOriginPair(DirectionEnum key, Transform value) : base(key, value)
        {

        }
    }

    public class KartMover : MonoBehaviour {

        public RayOriginTable rayOriginTable;

        private Rigidbody rb;
        private CapsuleCollider cc;
        private PlayerCore core;
        private GroundInfo nextGroundInfo;
        private Quaternion driftOffset = Quaternion.identity;
        private GroundInfo currentGroundInfo;
        private Vector3 currentVelocity;
        private Vector3 currentPos;
        private Vector3 rigidbodyPos;
        private Quaternion rotationStream;
        private float deltaTime;
        private float rotationCorrectionSpeed = 180f;
        public float minDriftStartAngle = 15f;
        public float maxDriftStartAngle = 90f;
        private RaycastHit[] raycastHitBuffer = new RaycastHit[8];
        Collider[] colliderBuffer = new Collider[8];
        private DriftState driftState;
        private KartStats stats;

        const int MaxPenetrationSolves = 3;
        const float GroundToCapsuleOffsetDistance = 0.025f;
        const float DeadZone = 0.01f;
        const float VelocityNormalAirborneDot = 0.5f;

        public Dictionary<DirectionEnum, DirectionEnum[]> crossDirectionDic = new Dictionary<DirectionEnum, DirectionEnum[]>()
        {
            {DirectionEnum.Front, new DirectionEnum[]{ DirectionEnum.Rear, DirectionEnum.Right, DirectionEnum.Left, DirectionEnum.Right} },
            {DirectionEnum.Right, new DirectionEnum[]{ DirectionEnum.Rear, DirectionEnum.Front, DirectionEnum.Left, DirectionEnum.Front} },
            {DirectionEnum.Left, new DirectionEnum[]{ DirectionEnum.Right, DirectionEnum.Front, DirectionEnum.Rear, DirectionEnum.Front} },
            {DirectionEnum.Rear, new DirectionEnum[]{ DirectionEnum.Left, DirectionEnum.Right, DirectionEnum.Front, DirectionEnum.Right} }
        };

        private void Start()
        {
            var input = GetComponent<IPlayerInput>();
            var state = GetComponent<PlayerState>();
            cc = GetComponent<CapsuleCollider>();
            rb = GetComponent<Rigidbody>();
            core = GetComponent<PlayerCore>();

            //this.UpdateAsObservable()
            //    .Subscribe(_ =>
            //    {
            //        var rotationStream = rb.rotation;
            //        var deltaTime = Time.deltaTime;
            //        currentPos = rb.position;
            //        var currentGroundInfo = CheckForGround(deltaTime, rotationStream, Vector3.zero);
            //        //Hop(rotationStream, currentGroundInfo);
            //        GroundInfo nextGroundInfo = CheckForGround(deltaTime, rotationStream, currentVelocity * deltaTime);
            //        StartDrift(currentGroundInfo, nextGroundInfo, rotationStream);
            //        StopDrift(deltaTime);
            //        CalculateDrivingVelocity(deltaTime, currentGroundInfo, rotationStream);

            //        Vector3 penetrationOffset = SolvePenetration(rotationStream);
            //        penetrationOffset = ProcessVelocityCollisions(deltaTime, rotationStream, penetrationOffset);

            //        rotationStream = Quaternion.RotateTowards(rb.rotation, rotationStream, rotationCorrectionSpeed * deltaTime);

            //        AdjustVelocityByPenetrationOffset(deltaTime, ref penetrationOffset);

            //        rb.MoveRotation(rotationStream);
            //        rb.MovePosition(currentPos + currentPos);
            //    });

            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    rotationStream = rb.rotation;
                    deltaTime = Time.deltaTime;
                    currentPos = rb.position;
                    currentGroundInfo = CheckForGround(deltaTime, rotationStream, Vector3.zero);
                });

            input.OnJumpButtonObseravable
                .Where(x => x)
                .Subscribe(_ => Hop(rotationStream, currentGroundInfo));

            this.UpdateAsObservable()
                .Subscribe(_ => nextGroundInfo = CheckForGround(deltaTime, rotationStream, currentVelocity * deltaTime));


            input.OnDriftButtonsObservable
                .Subscribe(l =>
                {
                    if (l.Any())
                        StartDrift(currentGroundInfo, nextGroundInfo, rotationStream, l);
                    else
                        StopDrift(deltaTime);
                });

            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    CalculateDrivingVelocity(deltaTime, currentGroundInfo, rotationStream);

                    Vector3 penetrationOffset = SolvePenetration(rotationStream);
                    penetrationOffset = ProcessVelocityCollisions(deltaTime, rotationStream, penetrationOffset);

                    rotationStream = Quaternion.RotateTowards(rb.rotation, rotationStream, rotationCorrectionSpeed * deltaTime);

                    AdjustVelocityByPenetrationOffset(deltaTime, ref penetrationOffset);
                });

            this.FixedUpdateAsObservable()
                .Subscribe(_ =>
                {

                    rb.MoveRotation(rotationStream);
                    rb.MovePosition(currentPos + currentPos);
                });
        }


        void Hop(Quaternion stream, GroundInfo groundInfo)
        {
            //if (currentGroundInfo.isGrounded && m_Input.HopPressed && m_HasControl)
            //{
            //    currentVelocity += stream * Vector3.up * stats.hopHeight;
            //}

            if (currentGroundInfo.isGrounded && core.HasControl.Value)
                currentVelocity += stream * Vector3.up * stats.hopHeight;
        }

        private Vector3 GetCrossVec3(DirectionEnum[] directions, Dictionary<DirectionEnum, RaycastHit> hitdic)
        {
            return Vector3.Cross(hitdic[directions[0]].point - hitdic[directions[1]].point, hitdic[directions[2]].point - hitdic[directions[3]].point);
        }

        GroundInfo CheckForGround(float duringTime, Quaternion stream, Vector3 offset)
        {
            GroundInfo groundInfo = new GroundInfo();
            Vector3 defaultPosition = offset + currentVelocity * duringTime;
            Vector3 direction = stream * Vector3.down;

            float capsuleRadius = cc.radius;
            float capsuleTouchingDistance = capsuleRadius + Physics.defaultContactOffset;
            float groundedDistance = capsuleTouchingDistance + GroundToCapsuleOffsetDistance;
            float closeToGroundDistance = Mathf.Max(groundedDistance + capsuleRadius, currentVelocity.y);

            var didHitDic = new Dictionary<DirectionEnum, bool>();
            var hitDic = new Dictionary<DirectionEnum, RaycastHit>();

            foreach(var kvp in rayOriginTable.GetTable()){
                var ray = new Ray(defaultPosition + kvp.Value.position, direction);
                var didhit = GetNearestFromRaycast(ray, closeToGroundDistance, groundLayers, QueryTriggerInteraction.Ignore, out RaycastHit hit);
                didHitDic.Add(kvp.Key, didhit);
                hitDic.Add(kvp.Key, hit);
            }

            var hitCount = didHitDic.Values.Count(x => x == true);
            groundInfo.isCapsuleTouching = hitDic.Values.Count(hit => hit.distance <= capsuleTouchingDistance) > 0;
            groundInfo.isGrounded = hitDic.Values.Count(hit => hit.distance <= groundedDistance) > 0;
            groundInfo.isCloseToGround = hitCount > 0;

            switch (hitCount){
                case 0:
                    groundInfo.normal = Vector3.up;
                    break;
                case 1:
                    var directf = didHitDic.First(x => x.Value).Key;
                    groundInfo.normal = hitDic[directf].normal;
                    break;
                case 2:
                    groundInfo.normal = hitDic.Values.Aggregate(Vector3.zero, (v_zero, hit) => v_zero + hit.normal) * 0.5f;
                    break;
                case 3:
                    var directl = didHitDic.Last(x => !x.Value).Key;
                    groundInfo.normal = GetCrossVec3(crossDirectionDic[directl], hitDic);
                    break;
                case 4:
                    groundInfo.normal = crossDirectionDic.Values
                        .Select(x => GetCrossVec3(x, hitDic))
                        .Aggregate(Vector3.zero, (v_zero, v) => v_zero + v) * 0.25f;
                    break;
            }

            if (groundInfo.isGrounded)
            {
                var dot = Vector3.Dot(groundInfo.normal, currentVelocity.normalized);
                if (dot > VelocityNormalAirborneDot)
                {
                    groundInfo.isGrounded = false;
                }
            }

            return groundInfo;
        }

        bool GetNearestFromRaycast(Ray ray, float rayDistance, int layerMask, QueryTriggerInteraction query, out RaycastHit hit)
        {
            int hits = Physics.RaycastNonAlloc(ray, raycastHitBuffer, rayDistance, layerMask, query);

            hit = new RaycastHit();
            hit.distance = float.PositiveInfinity;

            bool hitSelf = false;
            for (int i = 0; i < hits; i++)
            {
                if (raycastHitBuffer[i].collider == cc)
                {
                    hitSelf = true;
                    continue;
                }

                if (raycastHitBuffer[i].distance < hit.distance)
                    hit = raycastHitBuffer[i];
            }

            if (hitSelf)
                hits--;

            return hits > 0;
        }

        void StartDrift(GroundInfo currentInfo, GroundInfo nextInfo, Quaternion stream, List<DriftState> driftEnum)
        {
            if (!currentInfo.isGrounded && nextInfo.isGrounded && core.HasControl.Value && driftState == DriftState.NotDrifting)
            {
                Vector3 kartForward = (stream * Vector3.forward).SetY(0f).normalized;
                //kartForward.y = 0f;
                //kartForward.Normalize();
                Vector3 flatVelocity = currentVelocity.SetY(0f).normalized;
                //flatVelocity.y = 0f;
                //flatVelocity.Normalize();

                float signedAngle = Vector3.SignedAngle(kartForward, flatVelocity, Vector3.up);

                if (signedAngle > minDriftStartAngle && signedAngle < maxDriftStartAngle && driftEnum == DriftState.FacingRight)
                {
                    driftOffset = Quaternion.Euler(0f, signedAngle, 0f);
                    driftState = DriftState.FacingLeft;

                    //OnDriftStarted.Invoke();
                }
                else if (signedAngle < -minDriftStartAngle && signedAngle > -maxDriftStartAngle && driftEnum == DriftState.FacingLeft)
                {
                    driftOffset = Quaternion.Euler(0f, signedAngle, 0f);
                    driftState = DriftState.FacingRight;

                    //OnDriftStarted.Invoke();
                }
            }
        }

        /// <summary>
        /// Stops a drift if the hop input is no longer held.
        /// </summary>
        void StopDrift(float duringTime)
        {
            if (core.HasControl.Value)
            {
                driftOffset = Quaternion.RotateTowards(driftOffset, Quaternion.identity, rotationCorrectionSpeed * duringTime);
                driftState = DriftState.NotDrifting;

                //OnDriftStopped.Invoke();
            }
        }

        void CalculateDrivingVelocity(float duringTime, GroundInfo groundInfo, Quaternion stream)
        {
            Vector3 localVelocity = Quaternion.Inverse(stream) * Quaternion.Inverse(driftOffset) * currentVelocity;
            if (groundInfo.isGrounded)
            {
                localVelocity.x = Mathf.MoveTowards(localVelocity.x, 0f, stats.grip * duringTime);

                float acceleration = core.HasControl.Value ? m_Input.Acceleration : localVelocity.z > 0.05f ? -1f : 0f;

                if (acceleration > -DeadZone && acceleration < DeadZone)    // No acceleration input.
                    localVelocity.z = Mathf.MoveTowards(localVelocity.z, 0f, stats.coastingDrag * duringTime);
                else if (acceleration > DeadZone)                            // Positive acceleration input.
                    localVelocity.z = Mathf.MoveTowards(localVelocity.z, stats.topSpeed, acceleration * stats.acceleration * duringTime);
                else if (localVelocity.z > DeadZone)                         // Negative acceleration input and going forwards.
                    localVelocity.z = Mathf.MoveTowards(localVelocity.z, 0f, -acceleration * stats.braking * duringTime);
                else                                                           // Negative acceleration input and not going forwards.
                    localVelocity.z = Mathf.MoveTowards(localVelocity.z, -stats.reverseSpeed, -acceleration * stats.reverseAcceleration * duringTime);
            }

            if (groundInfo.isCapsuleTouching)
                localVelocity.y = Mathf.Max(0f, localVelocity.y);

            currentVelocity = driftOffset * stream * localVelocity;

            if (!groundInfo.isCapsuleTouching)
                currentVelocity += Vector3.down * stats.gravity * duringTime;
        }

        Vector3 SolvePenetration(Quaternion stream)
        {
            Vector3 summedOffset = Vector3.zero;
            for (var solveIterations = 0; solveIterations < MaxPenetrationSolves; solveIterations++)
            {
                summedOffset = ComputePenetrationOffset(stream, summedOffset);
            }

            return summedOffset;
        }

        Vector3 ProcessVelocityCollisions(float duringTime, Quaternion stream, Vector3 penetrationOffset)
        {
            Vector3 rayDirection = currentVelocity * duringTime + penetrationOffset;
            float rayLength = rayDirection.magnitude + .2f;
            Ray sphereCastRay = new Ray(rigidbodyPos, rayDirection);
            int hits = Physics.SphereCastNonAlloc(sphereCastRay, 0.75f, raycastHitBuffer, rayLength, allCollidingLayers, QueryTriggerInteraction.Collide);

            for (int i = 0; i < hits; i++)
            {
                if (raycastHitBuffer[i].collider == cc)
                    continue;


                //IKartModifier kartModifier = raycastHitBuffer[i].collider.GetComponent<IKartModifier>();
                //if (kartModifier != null)
                //{
                //    m_CurrentModifiers.Add(kartModifier);
                //    m_TempModifiers.Add(kartModifier);
                //}

                IKartCollider kartCollider = raycastHitBuffer[i].collider.GetComponent<IKartCollider>();
                if (Mathf.Approximately(raycastHitBuffer[i].distance, 0f))
                    if (Physics.Raycast(rigidbodyPos, stream * Vector3.down, out RaycastHit hit, rayLength + 0.5f, allCollidingLayers, QueryTriggerInteraction.Collide))
                        raycastHitBuffer[i] = hit;

                if (kartCollider != null)
                {
                    currentVelocity = kartCollider.ModifyVelocity(this, raycastHitBuffer[i]);

                    if (Mathf.Abs(Vector3.Dot(raycastHitBuffer[i].normal, Vector3.up)) <= .2f)
                    {
                        //OnKartCollision.Invoke();
                    }
                }
                else
                {
                    penetrationOffset = ComputePenetrationOffset(stream, penetrationOffset);
                }
            }

            return penetrationOffset;
        }

        Vector3 ComputePenetrationOffset(Quaternion stream, Vector3 summedOffset)
        {
            var capsuleAxis = stream * Vector3.forward * cc.height * 0.5f;
            var point0 = rigidbodyPos + capsuleAxis + summedOffset;
            var point1 = rigidbodyPos - capsuleAxis + summedOffset;
            var kartCapsuleHitCount = Physics.OverlapCapsuleNonAlloc(point0, point1, cc.radius, colliderBuffer, allCollidingLayers, QueryTriggerInteraction.Ignore);

            for (int i = 0; i < kartCapsuleHitCount; i++)
            {
                var hitCollider = colliderBuffer[i];
                if (hitCollider == cc)
                    continue;

                var hitColliderTransform = hitCollider.transform;
                if (Physics.ComputePenetration(cc, rigidbodyPos + summedOffset, stream, hitCollider, hitColliderTransform.position, hitColliderTransform.rotation, out Vector3 separationDirection, out float separationDistance))
                {
                    Vector3 offset = separationDirection * (separationDistance + Physics.defaultContactOffset);
                    if (Mathf.Abs(offset.x) > Mathf.Abs(summedOffset.x))
                        summedOffset.x = offset.x;
                    if (Mathf.Abs(offset.y) > Mathf.Abs(summedOffset.y))
                        summedOffset.y = offset.y;
                    if (Mathf.Abs(offset.z) > Mathf.Abs(summedOffset.z))
                        summedOffset.z = offset.z;
                }
            }

            return summedOffset;
        }

        void AdjustVelocityByPenetrationOffset(float duringTime, ref Vector3 penetrationOffset)
        {
            // Find how much of the velocity is in the penetration offset's direction.
            Vector3 penetrationProjection = Vector3.Project(currentVelocity * duringTime, penetrationOffset);

            // If the projection and offset are in opposite directions (more than 90 degrees between the velocity and offset) ...
            if (Vector3.Dot(penetrationOffset, penetrationProjection) < 0f)
            {
                // ... and if the offset is larger than the projection...
                if (penetrationOffset.sqrMagnitude > penetrationProjection.sqrMagnitude)
                {
                    // ... then reduce the velocity by the equivalent velocity of the projection and the the offset by the projection.
                    currentVelocity -= penetrationProjection / duringTime;
                    penetrationOffset += penetrationProjection;
                }
                else // If the offset is smaller than the projection...
                {
                    // ... then reduce the velocity by the equivalent velocity of the offset and then there is the offset remaining.
                    currentVelocity += penetrationOffset / duringTime;
                    penetrationOffset = Vector3.zero;
                }
            }

            currentPos = currentVelocity * duringTime + penetrationOffset;
        }
    }
}
