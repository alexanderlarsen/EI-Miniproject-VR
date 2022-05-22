using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EI_MiniProject
{
    public class LabanCalculator : MonoBehaviour
    {
        #region Fields

        [Header("References"), SerializeField]
        private Body body;

        private readonly int frameWindow = 5;
        private readonly float weightMultiplier = 0.2f;
        private readonly float timeMultiplier = 0.001f;
        private readonly float flowMultiplier = 0.0001f;

        #endregion Fields

        private void Start()
        {
            StartCoroutine(CalculateTotalEfforts());
        }

        private IEnumerator CalculateTotalEfforts()
        {
            #region Session data variables

            List<float> weightList_left = new();
            List<float> weightList_right = new();
            List<float> weightList_head = new();

            List<float> timeList_left = new();
            List<float> timeList_right = new();
            List<float> timeList_head = new();

            List<float> flowList_left = new();
            List<float> flowList_right = new();
            List<float> flowList_head = new();

            float weightTarget_left, weightTarget_right, weightTarget_head;
            float timeTarget_left, timeTarget_right, timeTarget_head;
            float flowTarget_left, flowTarget_right, flowTarget_head;

            #endregion Session data variables

            while (true)
            {
                #region Clear existing list data before collecting new data

                weightList_left.Clear();
                weightList_right.Clear();
                weightList_head.Clear();

                timeList_left.Clear();
                timeList_right.Clear();
                timeList_head.Clear();

                flowList_left.Clear();
                flowList_right.Clear();
                flowList_head.Clear();

                #endregion Clear existing list data before collecting new data

                #region Collect data over {frameWindow} frames

                int currentFrameCount = 0;

                while (currentFrameCount < frameWindow)
                {
                    weightList_left.Add(body.LeftHand.Velocity.sqrMagnitude);
                    weightList_right.Add(body.RightHand.Velocity.sqrMagnitude);
                    weightList_head.Add(body.Head.Velocity.sqrMagnitude);

                    timeList_left.Add(body.LeftHand.Acceleration.sqrMagnitude);
                    timeList_right.Add(body.RightHand.Acceleration.sqrMagnitude);
                    timeList_head.Add(body.Head.Acceleration.sqrMagnitude);

                    flowList_left.Add(body.LeftHand.Jerk.sqrMagnitude);
                    flowList_right.Add(body.RightHand.Jerk.sqrMagnitude);
                    flowList_head.Add(body.Head.Jerk.sqrMagnitude);

                    yield return new WaitForFixedUpdate();
                    currentFrameCount++;
                }

                #endregion Collect data over {frameWindow} frames

                #region Calculate maximum WEIGHT efforts

                weightTarget_left = Mathf.Max(weightList_left.ToArray());
                weightTarget_right = Mathf.Max(weightList_right.ToArray());
                weightTarget_head = Mathf.Max(weightList_head.ToArray());

                #endregion Calculate maximum WEIGHT efforts

                #region Calculate average TIME efforts

                timeTarget_left = 0;
                timeTarget_right = 0;
                timeTarget_head = 0;

                for (int i = 0; i < timeList_left.Count; i++)
                    timeTarget_left += timeList_left[i];

                for (int i = 0; i < timeList_right.Count; i++)
                    timeTarget_right += timeList_right[i];

                for (int i = 0; i < timeList_head.Count; i++)
                    timeTarget_head += timeList_head[i];

                timeTarget_left /= timeList_left.Count;
                timeTarget_right /= timeList_right.Count;
                timeTarget_head /= timeList_head.Count;

                #endregion Calculate average TIME efforts

                #region Calculate average FLOW efforts

                flowTarget_left = 0;
                flowTarget_right = 0;
                flowTarget_head = 0;

                for (int i = 0; i < flowList_left.Count; i++)
                    flowTarget_left += flowList_left[i];

                for (int i = 0; i < flowList_right.Count; i++)
                    flowTarget_right += flowList_right[i];

                for (int i = 0; i < flowList_head.Count; i++)
                    flowTarget_head += flowList_head[i];

                flowTarget_left /= flowList_left.Count;
                flowTarget_right /= flowList_right.Count;
                flowTarget_head /= flowList_head.Count;

                #endregion Calculate average FLOW efforts

                #region Set final effort values in Joint objects

                body.LeftHand.WeightEffort = weightTarget_left * weightMultiplier;
                body.LeftHand.TimeEffort = timeTarget_left * timeMultiplier;
                body.LeftHand.FlowEffort = flowTarget_left * flowMultiplier;

                body.RightHand.WeightEffort = weightTarget_right * weightMultiplier;
                body.RightHand.TimeEffort = timeTarget_right * timeMultiplier;
                body.RightHand.FlowEffort = flowTarget_right * flowMultiplier;

                body.Head.WeightEffort = weightTarget_head * weightMultiplier;
                body.Head.TimeEffort = timeTarget_head * timeMultiplier;
                body.Head.FlowEffort = flowTarget_head * flowMultiplier;

                #endregion Set final effort values in Joint objects

                yield return new WaitForFixedUpdate();
            }
        }
    }
}