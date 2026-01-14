using System.Collections;
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

        [Header("Attach Animations")]
        public List<AnimationClip> attachAnimationClips = new List<AnimationClip>();

        [Header("Default Mount Side")]
        public MountSide defaultMountSide = MountSide.Right;

        [Header("Flip State")]
        [HideInInspector] public bool isFlipped;

        [Header("Attachment State")]
        public bool isAttached;

        [Header("Rotation System")]
        public List<RotationPivot> rotationPivots = new List<RotationPivot>();

        [Header("Movement System")]
        public List<MovementAxis> movementAxes = new List<MovementAxis>();

        [Header("Element Animations")]
        public List<ElementAnimation> elementAnimations = new List<ElementAnimation>();

        [HideInInspector]
        public MountPoint currentMountPoint;

        private readonly List<Animation> animationComponents = new List<Animation>();

        private void Awake()
        {
            if (string.IsNullOrEmpty(elementName))
                elementName = gameObject.name.Replace("_", " ");

            isFlipped = false;
            UpdateVisibility();

            InitAnimationComponents();
            RegisterClips();
        }

        private void InitAnimationComponents()
        {
            animationComponents.Clear();

            var rootAnim = GetComponent<Animation>();
            if (rootAnim != null)
                animationComponents.Add(rootAnim);

            var childAnims = GetComponentsInChildren<Animation>(true);
            foreach (var anim in childAnims)
            {
                if (!animationComponents.Contains(anim))
                    animationComponents.Add(anim);
            }

            Debug.Log("[TableElement] Animation components: " + animationComponents.Count);
        }

        private void RegisterClips()
        {
            foreach (var animComp in animationComponents)
            {
                foreach (var anim in elementAnimations)
                {
                    if (anim == null || anim.clips == null) continue;

                    foreach (var clip in anim.clips)
                    {
                        if (clip != null && animComp.GetClip(clip.name) == null)
                            animComp.AddClip(clip, clip.name);
                    }
                }

                foreach (var clip in attachAnimationClips)
                {
                    if (clip != null && animComp.GetClip(clip.name) == null)
                        animComp.AddClip(clip, clip.name);
                }
            }
        }

        public void SetAttached(bool attached)
        {
            isAttached = attached;
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            gameObject.SetActive(isAttached);
        }

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

        private void PlaySequence(List<AnimationClip> clips, float speed, bool forward)
        {
            StopAllCoroutines(); 
            StartCoroutine(PlaySequenceCoroutine(clips, speed, forward));
        }

        private IEnumerator PlaySequenceCoroutine(List<AnimationClip> clips, float speed, bool forward)
        {
            if (clips == null || clips.Count == 0) yield break;

            var sequence = forward ? clips : new List<AnimationClip>(clips);
            if (!forward) sequence.Reverse();

            foreach (var clip in sequence)
            {
                if (clip == null) continue;

                foreach (var anim in animationComponents)
                {
                    var state = anim[clip.name];
                    if (state == null)
                    {
                        anim.AddClip(clip, clip.name);
                        state = anim[clip.name];
                    }

                    state.speed = forward ? Mathf.Abs(speed) : -Mathf.Abs(speed);
                    state.time = forward ? 0f : clip.length;
                    state.weight = 1f;

                    anim.Play(clip.name, PlayMode.StopSameLayer);
                }

                float duration = clip.length / Mathf.Abs(speed);
                yield return new WaitForSeconds(duration);
            }
        }

        // =========================
        // HELPERS
        // =========================
        public bool HasRotationPivots()
        {
            return rotationPivots != null && rotationPivots.Count > 0;
        }

        public bool HasMovementAxes()
        {
            return movementAxes != null && movementAxes.Count > 0;
        }

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
