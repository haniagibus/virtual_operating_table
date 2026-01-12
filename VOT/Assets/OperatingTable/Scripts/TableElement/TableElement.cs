using System.Collections.Generic;
using UnityEngine;

namespace OperatingTable
{
    [System.Serializable]
    public class ElementAnimation
    {
        public RotationPivot pivot;        // opcjonalnie
        public MovementAxis axis;          // opcjonalnie
        public string axisName;            // nazwa osi pivotu/axis dla której animacja ma działać
        public AnimationClip clip;         // animacja do odtwarzania
        public float speed = 1f;           // prędkość odtwarzania
    }


    public class TableElement : MonoBehaviour
    {
        [Header("Element Info")]
        public string elementName;

        [Header("Element Type")]
        public ElementType type = ElementType.Component;

        [Header("Default Mount Side")]
        [Tooltip("Domyślna strona montażu (lewa/prawa)")]
        public MountSide defaultMountSide = MountSide.Right;

        [Header("Flip State")]
        [HideInInspector]
        public bool isFlipped = false;

        [Header("Attachment State")]
        public bool isAttached = false;

        [Header("Rotation System")]
        public List<RotationPivot> rotationPivots = new List<RotationPivot>();

        [Header("Movement System")]
        public List<MovementAxis> movementAxes = new List<MovementAxis>();

        [HideInInspector]
        public MountPoint currentMountPoint;

        [Header("Element Animations")]
        public List<ElementAnimation> elementAnimations = new List<ElementAnimation>();

        private Animator animator; // Animator powiązany z tym elementem

        void Awake()
        {
            if (string.IsNullOrEmpty(elementName))
            {
                elementName = gameObject.name.Replace("_", " ");
            }

            // Ustaw początkowy stan obrotu na false (domyślnie element jest po prawej)
            isFlipped = false;

            UpdateVisibility();

            animator = GetComponent<Animator>();
            if (animator == null && elementAnimations.Count > 0)
            {
                Debug.LogWarning("[TableElement] Animator wymagany do obsługi elementAnimations!");
            }
        }

        public void UpdateVisibility()
        {
            gameObject.SetActive(isAttached);
        }

        public void SetAttached(bool attached)
        {
            isAttached = attached;
            UpdateVisibility();
        }


        // ====================== ANIMACJE PIVOTA ======================

        public void PlayAnimationForPivot(RotationPivot pivot, string axis, bool forward = true)
        {
            if (animator == null) return;

            var anim = elementAnimations.Find(x => x.pivot == pivot && x.axisName == axis);
            if (anim == null) return;

            animator.speed = forward ? Mathf.Abs(anim.speed) : -Mathf.Abs(anim.speed);
            animator.Play(anim.clip.name, 0, forward ? 0f : 1f);
        }

        public void PlayAnimationForAxis(MovementAxis axisObj, string axis, bool forward = true)
        {
            if (animator == null) return;

            var anim = elementAnimations.Find(x => x.axis == axisObj && x.axisName == axis);
            if (anim == null) return;

            animator.speed = forward ? Mathf.Abs(anim.speed) : -Mathf.Abs(anim.speed);
            animator.Play(anim.clip.name, 0, forward ? 0f : 1f);
        }




        /// <summary>
        /// Sprawdza czy element jest zamontowany po przeciwnej stronie niż domyślna
        /// </summary>
        public bool IsMountedOnOppositeSide()
        {
            if (currentMountPoint == null)
                return false;

            return currentMountPoint.side != defaultMountSide;
        }

        /// <summary>
        /// Zwraca pivot o podanej nazwie
        /// </summary>
        public RotationPivot GetPivotByName(string pivotName)
        {
            return rotationPivots.Find(p => p.pivotName == pivotName);
        }

        /// <summary>
        /// Zwraca oś ruchu o podanej nazwie
        /// </summary>
        public MovementAxis GetAxisByName(string axisName)
        {
            return movementAxes.Find(a => a.axisName == axisName);
        }

        /// <summary>
        /// Sprawdza czy element ma jakiekolwiek pivoty
        /// </summary>
        public bool HasRotationPivots()
        {
            return rotationPivots != null && rotationPivots.Count > 0;
        }

        /// <summary>
        /// Sprawdza czy element ma jakiekolwiek osie ruchu
        /// </summary>
        public bool HasMovementAxes()
        {
            return movementAxes != null && movementAxes.Count > 0;
        }
    }
}