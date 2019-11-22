using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UniRx.Async;
using Zenject;
using MC.GameManager;
using UniRx.Triggers;
using MC.Utils;
using System.Linq;
using KartGame.KartSystems;

namespace MC.Players
{

    public class AIInput : MonoBehaviour, IPlayerInput
    {
        private ReactiveProperty<bool> _isJumping = new ReactiveProperty<bool>();
        private ReactiveProperty<bool> _hasUsingItme = new ReactiveProperty<bool>();
        private ReactiveProperty<float> _straightAccelerate = new ReactiveProperty<float>();
        private ReactiveProperty<float> _bendAccelerate = new ReactiveProperty<float>();
        private ReactiveProperty<bool> _isDefending = new ReactiveProperty<bool>();
        private ReactiveProperty<float> itemThrowDirection = new ReactiveProperty<float>();

        public IReadOnlyReactiveProperty<bool> IsJumping { get { return _isJumping.ToReadOnlyReactiveProperty(); } }
        public IReadOnlyReactiveProperty<bool> HasUsingItem { get { return _hasUsingItme.ToReadOnlyReactiveProperty(); } }
        public IReadOnlyReactiveProperty<float> StraightAccelerate { get { return _straightAccelerate.ToReadOnlyReactiveProperty(); } }
        public IReadOnlyReactiveProperty<float> BendAccelerate { get { return _bendAccelerate.ToReadOnlyReactiveProperty(); } }
        public IReadOnlyReactiveProperty<bool> IsDefending { get { return _isDefending.ToReadOnlyReactiveProperty(); } }
        public IReadOnlyReactiveProperty<float> ItemThrowDirection => itemThrowDirection.ToReadOnlyReactiveProperty();

        [Inject] RankManager rankManager;
        [SerializeField] private GameObject pointer;


        private float kartRadius;
        private KartMovement kartMovement;
        private PlayerId playerId;
        private Transform[] checkPoints = new Transform[3];
        private float[] Widths = new float[3];
        private float topSpeed;
        private float turnSpeed;
        private float acceleration;
        private Transform currentCheckPoint;

        private async void Start()
        {
            var core = GetComponent<PlayerCore>();
            kartMovement = GetComponent<KartMovement>();
            var itemGetter = GetComponent<ItemGetter>();
            var cc = GetComponent<CapsuleCollider>();
            kartRadius = cc.radius;

            await UniTask.WhenAll(core.OnInitialized.ToUniTask(), rankManager.Initialized.ToUniTask());

            topSpeed = kartMovement.CurrentStats.topSpeed;
            turnSpeed = kartMovement.CurrentStats.turnSpeed;
            acceleration = kartMovement.CurrentStats.acceleration;

            this.playerId = core.PlayerId;
            _straightAccelerate.Value = 1;

            var line1 = new GameObject("Line", typeof(LineRenderer)).GetComponent<LineRenderer>();
            var line2 = new GameObject("Line", typeof(LineRenderer)).GetComponent<LineRenderer>();
            var line3 = new GameObject("Line", typeof(LineRenderer)).GetComponent<LineRenderer>();
            var line4 = new GameObject("Line", typeof(LineRenderer)).GetComponent<LineRenderer>();

            line1.SetVertexCount(2);
            line1.SetWidth(0.2f, 0.2f);
            line2.SetVertexCount(2);
            line2.SetWidth(0.2f, 0.2f);
            line1.SetColors(Color.red, Color.red);
            line2.SetColors(Color.blue, Color.blue);
            line3.SetVertexCount(2);
            line3.SetWidth(0.2f, 0.2f);
            line3.SetColors(Color.blue, Color.blue);
            line4.SetVertexCount(2);
            line4.SetWidth(1f, 1f);
            line4.SetColors(Color.blue, Color.blue);

            itemGetter.UseItemObservable
                .Do(_ => _hasUsingItme.Value = true)
                .DelayFrame(1)
                .Subscribe(_ => _hasUsingItme.Value = false);

            rankManager.GetRankInfo(playerId)
                .ObserveEveryValueChanged(x => x.checkPointId)
                .Subscribe(x =>
                {
                    currentCheckPoint = rankManager.GetCheckPoint(x);
                    for (int i = 0; i < 2; i++)
                    {
                        var checkPoint = rankManager.GetCheckPoint(x + i + 1);
                        checkPoints[i] = checkPoint;
                        var mf = checkPoint.GetComponent<MeshFilter>();
                        var vertic = mf.mesh.vertices[0];
                        var checkPointScale = checkPoint.localScale;
                        var rootScale = checkPoint.root.localScale;
                        Widths[i] = Mathf.Abs(vertic.x) * Mathf.Abs(checkPointScale.x) * Mathf.Abs(rootScale.x);
                    }
                });

            core.HasControl
                .Where(x => !x)
                .Subscribe(_ => _isJumping.Value = false);

            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    var n = checkPoints[0].transform.forward;
                    var r = checkPoints[0].transform.right;
                    var u = checkPoints[0].transform.up;
                    var x = checkPoints[0].transform.position;
                    var x0 = transform.position;
                    var m = n;
                    var h = Vector3.Dot(n, x);
                    var intersectPoint = x0 + ((h - Vector3.Dot(n, x0)) / (Vector3.Dot(n, m))) * m;
                    var currentWidth = (intersectPoint - x).magnitude;
                    var checkPointWidth = Widths[0] - kartRadius;
                    line3.SetPosition(0, x + Vector3.up);
                    line3.SetPosition(1, intersectPoint + Vector3.up);
                    var direction = intersectPoint - x0;
                    var currentDistance = direction.magnitude;
                    var currentVec = kartMovement.Movement;
                    if (currentVec.magnitude < 0.1f)
                        currentVec = transform.forward;
                    var nearNormal = x + r * checkPointWidth * (Vector3.Cross(intersectPoint - x, n).y < 0f ? 1f : -1f);
                    var padding = 4f;
                    if (currentWidth - padding > checkPointWidth)
                    {
                        var intersection = Vector2.zero;
                        var startA = x0.ToXZVector2();
                        var offsetA = r.ToXZVector2();
                        var starB = nearNormal.ToXZVector2();
                        var offsetB = n.ToXZVector2();
                        if (Vector2Extensions.LineSegmentsIntersection(startA, startA + offsetA, starB, starB + offsetB, out intersection))
                            intersectPoint = new Vector3(intersection.x, x0.y,intersection.y);
                        else
                            intersectPoint = nearNormal;
                    }else if(currentWidth >= checkPointWidth && currentWidth < checkPointWidth + padding)
                    {
                        intersectPoint = nearNormal;
                    }
                    pointer.transform.position = intersectPoint;
                    //var height = (intersectPoint - PerpendicularFootPoint(x, x + r, intersectPoint)).magnitude;
                    line1.SetPosition(0, transform.position + Vector3.up);
                    line2.SetPosition(0, transform.position + Vector3.up);
                    line1.SetPosition(1, transform.position + transform.forward + Vector3.up);
                    line2.SetPosition(1, transform.position + (intersectPoint - x0 + Vector3.up));
                    //var tangentLine = checkPoints[1].position - checkPoints[0].position;
                    //line4.SetPosition(0, checkPoints[0].position);
                    //line4.SetPosition(1, checkPoints[0].position + tangentLine);
                    var nextangle = checkPoints[1].eulerAngles.y - checkPoints[0].eulerAngles.y;
                    nextangle = Mathf.Abs(nextangle) <= 180 ? nextangle : -nextangle / Mathf.Abs(nextangle) * (360f - Mathf.Abs(nextangle));
                    //Debug.Log(nextangle);
                    //var nextaxis = Vector3.Cross(n, tangentLine);
                    //var nextangle = Vector3.Angle(n, tangentLine) * (nextaxis.y < 0 ? -1 : 1f);
                    var dir = 0f;
                    if (Mathf.Abs(nextangle) >= 20f && !IsJumping.Value)
                    {
                        var currentVecMagnitude = currentVec.SetY(0f).magnitude;
                        var maximumSpeedReachingTime = (topSpeed - currentVecMagnitude) / acceleration;
                        var width = nextangle >= 0f ? checkPointWidth + currentWidth : checkPointWidth - currentWidth;
                        var time = 2.2f + 2.1f * width / (checkPointWidth * 2f);
                        var accelerationtime = Mathf.Clamp(maximumSpeedReachingTime, 0f, time);
                        //Debug.Log(accelerationtime);
                        var constantSpeedTime = Mathf.Max(time - accelerationtime, 0f);
                        var distance = currentVecMagnitude * accelerationtime + 0.5f * acceleration * accelerationtime * accelerationtime + topSpeed * constantSpeedTime;
                        if (currentDistance < distance)
                        {
                            _isJumping.Value = true;
                            dir = nextangle / Mathf.Abs(nextangle);
                            _bendAccelerate.Value = dir;
                        }
                    }
                    var currentAngle = checkPoints[0].position.y - currentCheckPoint.position.y;
                    currentAngle = Mathf.Abs(currentAngle) <= 180 ? currentAngle : -currentAngle / Mathf.Abs(currentAngle) * (360f - Mathf.Abs(currentAngle));
                    if (Mathf.Abs(currentAngle) < 20f && Mathf.Abs(nextangle) < 20f && IsJumping.Value && Vector3.Angle(n, currentVec) * (Vector3.Cross(n, currentVec).y < 0 ? -1 : 1f) <  -8.0f * dir)
                    {
                        _isJumping.Value = false;
                    }
                    if (!IsJumping.Value)
                    {
                        var axis = Vector3.Cross(currentVec, intersectPoint - x0);
                        var angle = Vector3.Angle(currentVec, intersectPoint - x0) * (axis.y < 0 ? -1f : 1f);
                        //angle = Mathf.Abs(angle) <= 180 ? angle : -angle / Mathf.Abs(angle) * (360f - Mathf.Abs(angle));
                        var angleInOneFrame = turnSpeed * Time.deltaTime;
                        var bend = Mathf.Abs(angle) >= angleInOneFrame ? 1f : Mathf.Abs(angle) / angleInOneFrame;
                        if (playerId == PlayerId.Player1)
                            Debug.Log(angle);
                        if (angle > 0.01f)
                            _bendAccelerate.Value = bend;
                        else if (angle < -0.01f)
                            _bendAccelerate.Value = -bend;
                        else
                            _bendAccelerate.Value = 0f;
                    }
                }).AddTo(this.gameObject);
        }

        Vector3 PerpendicularFootPoint(Vector3 a, Vector3 b, Vector3 p)
        {
            return a + Vector3.Project(p - a, b - a);
        }
    }
}
