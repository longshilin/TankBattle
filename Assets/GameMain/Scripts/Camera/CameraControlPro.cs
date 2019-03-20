using UnityEngine;

namespace TankBattle{

    public class CameraControlPro : MonoBehaviour {

        public Transform m_Target;


        public float distanceUp = 5f;
        public float distanceAway = 10f;
        public float smooth = 10000f;//位置平滑移动值
        public float camDepthSmooth = 10000f;
        // Use this for initialization
        void Start() {
        }

        // Update is called once per frame
        void Update() {
            // 鼠标轴控制相机的远近
            if ((Input.mouseScrollDelta.y < 0 && Camera.main.fieldOfView >= 3) || Input.mouseScrollDelta.y > 0 && Camera.main.fieldOfView <= 80) {
                Camera.main.fieldOfView += Input.mouseScrollDelta.y * camDepthSmooth * Time.deltaTime;
            }
        }

        void LateUpdate() {

            if (m_Target != null) {
                //相机的位置
                Vector3 disPos = m_Target.position + Vector3.up * distanceUp - m_Target.forward * distanceAway;
                transform.position = Vector3.Lerp(transform.position, disPos, Time.deltaTime * smooth);
                //相机的角度
                transform.LookAt(m_Target.position);
            }
        }
    }
}