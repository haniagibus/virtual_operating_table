using System.Collections.Generic;
using UnityEngine;

namespace VirtualOperatingTable
{
    [System.Serializable]
    public class ElementAnimation
    {
        public RotationPivot pivot;
        public MovementAxis axis;
        // public string axisName;
        public AnimationClip clip;
        public float speed = 1f;
    }

    public class TableElement : MonoBehaviour
    {
        [Header("Element Info")]
        public string elementName;

        [Header("Element Type")]
        public ElementType type = ElementType.Component;

        [Header("Default Attach Animation")]
        public AnimationClip attachAnimationClip;

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

        private Animation animationComponent;

        void Awake()
        {
            if (string.IsNullOrEmpty(elementName))
            {
                elementName = gameObject.name.Replace("_", " ");
            }

            isFlipped = false;
            UpdateVisibility();

            animationComponent = GetComponent<Animation>();

            if (animationComponent == null)
            {
                animationComponent = GetComponentInChildren<Animation>(true);
            }

            if (animationComponent == null)
            {
                animationComponent = gameObject.AddComponent<Animation>();
                Debug.Log("[TableElement] Dodano Animation component na root");
            }
            else
            {
                Debug.Log("[TableElement] Znaleziono Animation na: " + animationComponent.gameObject.name);
            }

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

            if (attachAnimationClip != null &&
        !animationComponent.GetClip(attachAnimationClip.name))
            {
                animationComponent.AddClip(attachAnimationClip, attachAnimationClip.name);
                Debug.Log("[TableElement] Dodano attach clip: " + attachAnimationClip.name);
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

        public void PlayAnimationForPivot(RotationPivot pivot, bool forward = true)
        {
            if (animationComponent == null)
            {
                Debug.LogWarning("[TableElement] Animation component nie istnieje!");
                return;
            }

            var anim = elementAnimations.Find(x => x.pivot == pivot);
            if (anim == null || anim.clip == null)
            {
                Debug.LogWarning("[TableElement] Brak animacji dla pivotu: " + pivot.name);
                return;
            }

            AnimationState state = animationComponent[anim.clip.name];
            if (state == null)
                return;

            state.speed = forward ? Mathf.Abs(anim.speed) : -Mathf.Abs(anim.speed);
            state.time = forward ? 0f : anim.clip.length;
            state.weight = 1f;
            state.enabled = true;

            animationComponent.Play(anim.clip.name, PlayMode.StopSameLayer);
        }


        public void PlayAnimationForAxis(MovementAxis axisObj, bool forward = true)
        {
            if (animationComponent == null)
            {
                Debug.LogWarning("[TableElement] Animation component nie istnieje!");
                return;
            }

            var anim = elementAnimations.Find(x => x.axis == axisObj);
            if (anim == null || anim.clip == null)
            {
                Debug.LogWarning("[TableElement] Brak animacji dla osi: " + axisObj.name);
                return;
            }

            AnimationState state = animationComponent[anim.clip.name];
            if (state == null)
                return;

            state.speed = forward ? Mathf.Abs(anim.speed) : -Mathf.Abs(anim.speed);
            state.time = forward ? 0f : anim.clip.length;
            state.weight = 1f;
            state.enabled = true;

            animationComponent.Play(anim.clip.name, PlayMode.StopSameLayer);
        }


        public void PlayAttachAnimation()
        {
            if (animationComponent == null || attachAnimationClip == null)
                return;

            AnimationState state = animationComponent[attachAnimationClip.name];

            state.time = 0f;
            state.enabled = true;
            state.weight = 1f;

            animationComponent.Play(attachAnimationClip.name, PlayMode.StopSameLayer);
        }


        // public void StopAnimation(string clipName)
        // {
        //     if (animationComponent != null && animationComponent.IsPlaying(clipName))
        //     {
        //         animationComponent.Stop(clipName);
        //     }
        // }

        // public void StopAllAnimations()
        // {
        //     if (animationComponent != null)
        //     {
        //         animationComponent.Stop();
        //     }
        // }

        // public bool IsMountedOnOppositeSide()
        // {
        //     if (currentMountPoint == null)
        //         return false;

        //     return currentMountPoint.side != defaultMountSide;
        // }

        // public RotationPivot GetPivotByName(string pivotName)
        // {
        //     return rotationPivots.Find(p => p.pivotName == pivotName);
        // }

        // public MovementAxis GetAxisByName(string axisName)
        // {
        //     return movementAxes.Find(a => a.axisName == axisName);
        // }

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