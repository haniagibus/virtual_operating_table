using System.Collections.Generic;
using UnityEngine;

namespace OperatingTable
{
    [System.Serializable]
    public class ElementAnimation
    {
        public RotationPivot pivot;
        public MovementAxis axis;
        public string axisName;
        public AnimationClip clip;
        public float speed = 1f;
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

        private Animation animationComponent; // Animation zamiast Animator!

        void Awake()
        {
            if (string.IsNullOrEmpty(elementName))
            {
                elementName = gameObject.name.Replace("_", " ");
            }

            isFlipped = false;
            UpdateVisibility();

            // Użyj Animation component zamiast Animator
            animationComponent = GetComponent<Animation>();
            if (animationComponent == null && elementAnimations.Count > 0)
            {
                animationComponent = gameObject.AddComponent<Animation>();
                Debug.Log("[TableElement] Dodano Animation component");
            }

            // Dodaj wszystkie klipy do Animation component
            if (animationComponent != null)
            {
                foreach (var anim in elementAnimations)
                {
                    if (anim.clip != null)
                    {
                        animationComponent.AddClip(anim.clip, anim.clip.name);
                    }
                }
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
            if (animationComponent == null)
            {
                Debug.LogWarning("[TableElement] Animation component nie istnieje!");
                return;
            }

            var anim = elementAnimations.Find(x => x.pivot == pivot && x.axisName == axis);
            if (anim == null)
            {
                Debug.LogWarning("[TableElement] Nie znaleziono animacji dla pivot=" + pivot + " axis=" + axis);
                return;
            }

            AnimationState state = animationComponent[anim.clip.name];
            if (state == null)
            {
                Debug.LogWarning("[TableElement] AnimationState nie istnieje dla " + anim.clip.name);
                return;
            }

            // Ustaw prędkość i czas
            state.speed = forward ? Mathf.Abs(anim.speed) : -Mathf.Abs(anim.speed);
            state.time = forward ? 0f : anim.clip.length;
            state.enabled = true;
            state.weight = 1f;

            // Odtwórz w trybie addytywnym (wiele animacji jednocześnie!)
            animationComponent.Play(anim.clip.name, PlayMode.StopSameLayer);

            Debug.Log("[TableElement] Odtwarzam animację: " + anim.clip.name + " forward=" + forward);
        }

        public void PlayAnimationForAxis(MovementAxis axisObj, string axis, bool forward = true)
        {
            if (animationComponent == null)
            {
                Debug.LogWarning("[TableElement] Animation component nie istnieje!");
                return;
            }

            var anim = elementAnimations.Find(x => x.axis == axisObj && x.axisName == axis);
            if (anim == null)
            {
                Debug.LogWarning("[TableElement] Nie znaleziono animacji dla axis=" + axisObj + " axisName=" + axis);
                return;
            }

            AnimationState state = animationComponent[anim.clip.name];
            if (state == null)
            {
                Debug.LogWarning("[TableElement] AnimationState nie istnieje dla " + anim.clip.name);
                return;
            }

            // Ustaw prędkość i czas
            state.speed = forward ? Mathf.Abs(anim.speed) : -Mathf.Abs(anim.speed);
            state.time = forward ? 0f : anim.clip.length;
            state.enabled = true;
            state.weight = 1f;

            // Odtwórz w trybie addytywnym (wiele animacji jednocześnie!)
            animationComponent.Play(anim.clip.name, PlayMode.StopSameLayer);

            Debug.Log("[TableElement] Odtwarzam animację: " + anim.clip.name + " forward=" + forward);
        }

        public void StopAnimation(string clipName)
        {
            if (animationComponent != null && animationComponent.IsPlaying(clipName))
            {
                animationComponent.Stop(clipName);
            }
        }

        public void StopAllAnimations()
        {
            if (animationComponent != null)
            {
                animationComponent.Stop();
            }
        }

        public bool IsMountedOnOppositeSide()
        {
            if (currentMountPoint == null)
                return false;

            return currentMountPoint.side != defaultMountSide;
        }

        public RotationPivot GetPivotByName(string pivotName)
        {
            return rotationPivots.Find(p => p.pivotName == pivotName);
        }

        public MovementAxis GetAxisByName(string axisName)
        {
            return movementAxes.Find(a => a.axisName == axisName);
        }

        public bool HasRotationPivots()
        {
            return rotationPivots != null && rotationPivots.Count > 0;
        }

        public bool HasMovementAxes()
        {
            return movementAxes != null && movementAxes.Count > 0;
        }
    }
}