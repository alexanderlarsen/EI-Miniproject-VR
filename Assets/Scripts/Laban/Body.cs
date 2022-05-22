using MyBox;
using UnityEngine;

namespace EI_MiniProject
{
    public class Body : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Transform headTransform;

        [SerializeField]
        private Transform rightHandTransform;

        [SerializeField]
        private Transform leftHandTransform;

        [field: Header("Joint Data"), SerializeField, ReadOnly]
        public Joint Head { get; private set; } = new Joint();

        [field: SerializeField, ReadOnly]
        public Joint RightHand { get; private set; } = new Joint();

        [field: SerializeField, ReadOnly]
        public Joint LeftHand { get; private set; } = new Joint();

        private void Awake()
        {
            Head = new Joint();
            LeftHand = new Joint();
            RightHand = new Joint();
        }

        private void FixedUpdate()
        {
            Head.CalculatePositionDerivatives(headTransform.position);
            RightHand.CalculatePositionDerivatives(rightHandTransform.position);
            LeftHand.CalculatePositionDerivatives(leftHandTransform.position);
        }
    }
}