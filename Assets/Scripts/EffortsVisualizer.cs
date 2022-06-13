using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EI_MiniProject
{
    public class EffortsVisualizer : MonoBehaviour
    {
        #region Data Structures

        private class LineDataPoint
        {
            public readonly Vector3 point;
            public readonly float width;
            public readonly Color color;
            public readonly GameObject flowSphereGameObject;

            public LineDataPoint(Vector3 point, float width, Color color, GameObject flowSphereGameObject)
            {
                this.point = point;
                this.width = width;
                this.color = color;
                this.flowSphereGameObject = flowSphereGameObject;
            }
        }

        private enum JointType
        {
            Head,
            RightHand,
            LeftHand
        }

        #endregion Data Structures

        #region Fields

        [Header("References")]
        [SerializeField]
        private Body body;

        [SerializeField]
        private LineRenderer lineRenderer;

        [SerializeField]
        private GameObject flowSpherePrefab;

        [Header("Settings")]
        [SerializeField]
        private JointType associatedJoint;

        private readonly float lineWidthMultiplier = 0.1f;
        private readonly int maxPoints = 100;
        private readonly float weightMin = -1, weightMax = 10;
        private readonly float timeMin = 5, timeMax = 40;
        private readonly float flowMin = 100, flowMax = 3000;

        private readonly List<LineDataPoint> lineDataPoints = new();
        private bool record;

        #endregion Fields

        #region Properties

        private Color Color => Color.Lerp(Color.green, Color.red, Mathf.InverseLerp(flowMin, flowMax, Joint.FlowEffort));
        //private float DistanceBetweenPoints => Mathf.Clamp((1f - Mathf.InverseLerp(timeMin, timeMax, Joint.TimeEffort)) * 0.05f, 0.01f, Mathf.Infinity);
        private float DistanceBetweenPoints => Mathf.InverseLerp(timeMax, timeMin, Joint.TimeEffort) * 0.1f;
        private float Width => Mathf.InverseLerp(weightMin, weightMax, Joint.WeightEffort);
        private Vector3 LastLinePoint => lineRenderer.GetPosition(lineRenderer.positionCount - 1);

        private Joint Joint
        {
            get
            {
                return associatedJoint switch
                {
                    JointType.Head => body.Head,
                    JointType.RightHand => body.RightHand,
                    JointType.LeftHand => body.LeftHand,
                    _ => null,
                };
            }
        }

        private AnimationCurve WidthCurve
        {
            get
            {
                AnimationCurve output = new();

                for (int i = 0; i < lineDataPoints.Count; i++)
                    output.AddKey(i / (float)lineDataPoints.Count, lineDataPoints[i].width);

                return output;
            }
        }

        #endregion Properties

        #region MonoBehaviour Methods

        private void Start()
        {
            lineRenderer.widthMultiplier = lineWidthMultiplier;
            AddPoint(Joint.Position, Width, Color);
        }

        private void Update()
        {
            if (!record)
                return;

            Debug.Log(DistanceBetweenPoints);

            if (Vector3.Distance(Joint.Position, LastLinePoint) >= DistanceBetweenPoints)
                AddPoint(Joint.Position, Width, Color);
        }

        #endregion MonoBehaviour Methods

        #region Private Methods

        private void AddPoint(Vector3 point, float width, Color color)
        {
            GameObject flowSphereGameObject = Instantiate(flowSpherePrefab, point, Quaternion.identity, null);
            flowSphereGameObject.GetComponent<MeshRenderer>().material.color = color;

            lineDataPoints.Add(new LineDataPoint(point, width, Color, flowSphereGameObject));

            if (lineDataPoints.Count > maxPoints)
            {
                Destroy(lineDataPoints[0].flowSphereGameObject);
                lineDataPoints.RemoveAt(0);
            }

            lineRenderer.positionCount = lineDataPoints.Count;
            lineRenderer.SetPositions(lineDataPoints.Select(p => p.point).ToArray());
            lineRenderer.widthCurve = WidthCurve;
        }

        #endregion Private Methods

        public void ToggleRecord()
        {
            record = !record;

            if (record && lineDataPoints.Count == 0)
                AddPoint(Joint.Position, Width, Color);
        }

        public void ResetVisualization()
        {
            for (int i = 0; i < lineDataPoints.Count; i++)
                Destroy(lineDataPoints[i].flowSphereGameObject);

            lineDataPoints.Clear();
            lineRenderer.positionCount = 0;

            AddPoint(Joint.Position, Width, Color);
        }
    }
}