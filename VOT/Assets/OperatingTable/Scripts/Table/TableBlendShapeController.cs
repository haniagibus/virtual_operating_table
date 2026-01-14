using UnityEngine;

namespace VirtualOperatingTable
{
    public class TableBlendShapeController : MonoBehaviour
    {
        public SkinnedMeshRenderer blendShapeRenderer;

        public string tiltLeft = "tilt_left";
        public string tiltRight = "tilt_right";
        public string tiltForward = "tilt_forward";
        public string tiltBackward = "tilt_backward";

        public float maxTiltAngle = 26f;
        public float maxTrendelenburgAngle = 36f;

        private int indexLeft, indexRight, indexForward, indexBackward;

        private void Start()
        {
            FindIndices();
            ResetLateral();
            ResetTrendelenburg();
        }

        private void FindIndices()
        {
            var mesh = blendShapeRenderer.sharedMesh;

            for (int i = 0; i < mesh.blendShapeCount; i++)
            {
                string name = mesh.GetBlendShapeName(i);
                if (name == tiltLeft) indexLeft = i;
                else if (name == tiltRight) indexRight = i;
                else if (name == tiltForward) indexForward = i;
                else if (name == tiltBackward) indexBackward = i;
            }
        }

        public void UpdateLateral(float angle)
        {
            if (indexLeft >= 0)
                Set(indexLeft, Mathf.Max(angle, 0), maxTiltAngle);
            if (indexRight >= 0)
                Set(indexRight, Mathf.Max(-angle, 0), maxTiltAngle);

            Debug.Log("[TableBlendShapeController] Lateral left=" + Mathf.Max(angle, 0) + " right=" + Mathf.Max(-angle, 0));
        }

        private void ResetLateral()
        {
            Set(indexLeft, 0);
            Set(indexRight, 0);
        }
        public void UpdateTrendelenburg(float angle)
        {
            if (indexForward >= 0)
                Set(indexForward, Mathf.Max(angle, 0), maxTrendelenburgAngle);
            if (indexBackward >= 0)
                Set(indexBackward, Mathf.Max(-angle, 0), maxTrendelenburgAngle);

            Debug.Log("TableBlendShapeController] Trend forward=" + Mathf.Max(angle, 0) + " backward=" + Mathf.Max(-angle, 0));
        }


        private void ResetTrendelenburg()
        {
            Set(indexForward, 0);
            Set(indexBackward, 0);
        }

        private void Set(int index, float angle, float max = 1f)
        {
            if (index < 0) return;

            float weight = Mathf.Clamp01(Mathf.Abs(angle) / max) * 100f;
            blendShapeRenderer.SetBlendShapeWeight(index, weight);
        }
    }
}