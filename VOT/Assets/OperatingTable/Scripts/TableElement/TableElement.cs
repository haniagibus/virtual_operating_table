using System.Collections.Generic;
using UnityEngine;

namespace VirtualOperatingTable
{
    [System.Serializable]
    public class ElementAnimation
    {
        public RotationPivot pivot;
        public MovementAxis axis;

        public List<AnimationClip> clips = new List<AnimationClip>();

        public float speed = 1f;
    }

    public class TableElement : MonoBehaviour
    {
        [Header("Element Info")]
        public string elementName;

        [Header("Element Type")]
        public ElementType type = ElementType.Component;

        [Header("Default Attach Animation")]
        [Tooltip("Lista animacji montażu elementu")]
        public List<AnimationClip> attachAnimationClips = new List<AnimationClip>();

        [Header("Default Mount Side")]
        public MountSide defaultMountSide = MountSide.Right;

        [Header("Flip State")]
        [HideInInspector] public bool isFlipped = false;

        [Header("Attachment State")]
        public bool isAttached = false;

        [Header("Rotation System")]
        public List<RotationPivot> rotationPivots = new List<RotationPivot>();

        [Header("Movement System")]
        public List<MovementAxis> movementAxes = new List<MovementAxis>();

        [Header("Element Animations")]
        public List<ElementAnimation> elementAnimations = new List<ElementAnimation>();

        [HideInInspector]
        public MountPoint currentMountPoint;

        private Animation animationComponent;

        private void Awake()
        {
            if (string.IsNullOrEmpty(elementName))
                elementName = gameObject.name.Replace("_", " ");

            isFlipped = false;
            UpdateVisibility();

            InitAnimationComponent();
            RegisterClips();
        }

        // INIT
        private void InitAnimationComponent()
        {
            animationComponent = GetComponent<Animation>();

            if (animationComponent == null)
                animationComponent = GetComponentInChildren<Animation>(true);

            if (animationComponent == null)
            {
                animationComponent = gameObject.AddComponent<Animation>();
                Debug.Log("[TableElement] Dodano Animation component na root");
            }
            else
            {
                Debug.Log("[TableElement] Znaleziono Animation na: " + animationComponent.gameObject.name);
            }
        }

        private void RegisterClips()
        {
            if (animationComponent == null)
                return;

            foreach (var anim in elementAnimations)
            {
                if (anim != null && anim.clips != null)
                {
                    foreach (var clip in anim.clips)
                    {
                        if (clip != null && animationComponent.GetClip(clip.name) == null)
                        {
                            animationComponent.AddClip(clip, clip.name);
                        }
                    }
                }
            }

            if (attachAnimationClips != null)
            {
                foreach (var clip in attachAnimationClips)
                {
                    if (clip != null && animationComponent.GetClip(clip.name) == null)
                    {
                        animationComponent.AddClip(clip, clip.name);
                    }
                }
            }
        }

        // VISIBILITY / STATE
        public void SetAttached(bool attached)
        {
            isAttached = attached;
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            gameObject.SetActive(isAttached);
        }

        // ANIMATION API
        public void PlayAttachAnimation(bool forward = true)
        {
            if (attachAnimationClips == null || attachAnimationClips.Count == 0)
                return;

            PlaySequence(attachAnimationClips, 3f, forward);
        }

        public void Play(ElementAnimation anim, bool forward = true)
        {
            if (anim == null || anim.clips == null || anim.clips.Count == 0)
                return;

            PlaySequence(anim.clips, anim.speed, forward);
        }

        // ANIMATION INTERNAL
        private void PlaySequence(List<AnimationClip> clips, float speed, bool forward)
        {
            if (animationComponent == null || clips == null || clips.Count == 0)
                return;

            animationComponent.Stop();

            foreach (var clip in clips)
            {
                if (clip == null) continue;

                var state = animationComponent[clip.name];
                if (state == null)
                {
                    animationComponent.AddClip(clip, clip.name);
                    state = animationComponent[clip.name];
                }

                state.speed = forward ? Mathf.Abs(speed) : -Mathf.Abs(speed);
                state.time = forward ? 0f : clip.length;
                state.weight = 1f;

                animationComponent.PlayQueued(
                    clip.name,
                    QueueMode.CompleteOthers,
                    PlayMode.StopSameLayer
                );
            }
        }

        // HELPERS
        public bool HasRotationPivots()
        {
            return rotationPivots != null && rotationPivots.Count > 0;
        }

        public bool HasMovementAxes()
        {
            return movementAxes != null && movementAxes.Count > 0;
        }

        // GETTERS
        public ElementAnimation GetAnimationForPivot(RotationPivot pivot)
        {
            return elementAnimations.Find(a => a.pivot == pivot);
        }

        public ElementAnimation GetAnimationForAxis(MovementAxis axis)
        {
            return elementAnimations.Find(a => a.axis == axis);
        }
    }
}
