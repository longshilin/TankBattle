using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace TankBattle {

    /// <summary>
    /// 坦克履带实体：
    ///     掌握坦克的机动性
    /// </summary>
    public class Thruster : Entity {
        public AudioSource m_MovementAudio;         // Reference to the audio source used to play engine sounds. NB: different to the shooting audio source.
        public AudioClip m_EngineIdling;            // Audio to play when the tank isn't moving.
        public AudioClip m_EngineDriving;           // Audio to play when the tank is moving.

        [SerializeField]
        private ThrusterData m_TrusterData = null;

        private const string AttachPoint = "Thruster Point";

        private string m_MovementAxisName;          // The name of the input axis for moving forward and back.
        private string m_TurnAxisName;              // The name of the input axis for turning.
        public Rigidbody m_Rigidbody;              // Reference used to move the tank.
        private float m_MovementInputValue;         // The current value of the movement input.
        private float m_TurnInputValue;             // The current value of the turn input.
        private float m_OriginalPitch;              // The pitch of the audio source at the start of the scene.

        /// 第一部分被调用，Initialization模块
        private void Awake() {
            // 加载音频
            m_EngineIdling = Resources.Load<AudioClip>("Sounds/engine_idle");
            m_EngineDriving = Resources.Load<AudioClip>("Sounds/engine_driving");
            m_MovementAudio = GetComponent<AudioSource>();

            Debug.Log("m_MovementAudio - " + m_MovementAudio);
        }

        /// 将引擎实体附加到坦克父实体上
        protected override void OnAttachTo(EntityLogic parentEntity, Transform parentTransform, object userData) {
            base.OnAttachTo(parentEntity, parentTransform, userData);

            Name = Utility.Text.Format("Thruster of {0}", parentEntity.Name);
            CachedTransform.localPosition = Vector3.zero;
            //Debug.LogFormat(Constant.Logger.loggerFormat4, GetType(), System.Reflection.MethodBase.GetCurrentMethod().Name, Name, "Attach a Thruster instance to " + parentEntity.name);
        }

        // 生成实体
        protected override void OnShow(object userData) {
            base.OnShow(userData);

            m_TrusterData = userData as ThrusterData;
            if (m_TrusterData == null) {
                Log.Error(this.GetType() + " : Thruster data is invalid.");
                return;
            }

            // Attach Thruster System to Tank Render
            GameEntry.Entity.AttachEntity(Entity, m_TrusterData.OwnerId, AttachPoint);
            //Debug.LogFormat(Constant.Logger.loggerFormat4, GetType(), System.Reflection.MethodBase.GetCurrentMethod().Name, Name, "Instantiation a Thruster prefab and ThrusterData ");
        }

        /// 第三步被调用，Initialization模块
        // 定义方向按键
        private void Start() {
            m_MovementAxisName = "Vertical";
            m_TurnAxisName = "Horizontal";

            // Store the original pitch of the audio source.
            m_OriginalPitch = m_MovementAudio.pitch;

            //Debug.LogFormat(Constant.Logger.loggerFormat4, GetType(), System.Reflection.MethodBase.GetCurrentMethod().Name, Name, "set the input axis.");
        }

        /// 第二部分被调用，Initialization模块
        private void OnEnable() {
            //Debug.Log("OnEnable : m_Rigidbody" + m_Rigidbody);
            if (m_Rigidbody != null) {
                // When the tank is turned on, make sure it's not kinematic.
                m_Rigidbody.isKinematic = false;

                // Also reset the input values.
                m_MovementInputValue = 0f;
                m_TurnInputValue = 0f;

                //Debug.LogFormat(Constant.Logger.loggerFormat3, GetType(), System.Reflection.MethodBase.GetCurrentMethod().Name, Name);
            }
        }

        /// 第四步被调用，Physics模块
        private void FixedUpdate() {
            // Adjust the rigidbodies position and orientation in FixedUpdate.
            Move();
            Turn();
        }

        /// 第五步被调用，GameLogic模块
        private void Update() {
            m_MovementInputValue = ETCInput.GetAxis(m_MovementAxisName);
            m_TurnInputValue = ETCInput.GetAxis(m_TurnAxisName);

            EngineAudio();
        }

        private void EngineAudio() {
            // 坦克移动时候的音效管理器
            // If there is no input (the tank is stationary)...
            if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f) {
                //... and if the audio source is currently playing the driving clip...
                if (m_MovementAudio.clip == m_EngineDriving) {
                    // ... change the clip to idling and play it.
                    m_MovementAudio.clip = m_EngineIdling;
                    m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_TrusterData.PitchRange, m_OriginalPitch + m_TrusterData.PitchRange);
                    m_MovementAudio.Play();
                }
                //GameEntry.Sound.PlayDriving(m_TrusterData.IdleSound);
            }
            else {
                // Otherwise if the tank is moving and if the idling clip is currently playing...
                if (m_MovementAudio.clip == m_EngineIdling) {
                    // ... change the clip to driving and play.
                    m_MovementAudio.clip = m_EngineDriving;
                    m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_TrusterData.PitchRange, m_OriginalPitch + m_TrusterData.PitchRange);
                    m_MovementAudio.Play();
                }
                //GameEntry.Sound.PlayDriving(m_TrusterData.DriveSound);
            }
        }

        /// 最后一步被调用，Disable模块
        private void OnDisable() {
            // When the tank is turned off, set it to kinematic so it stops moving.
            m_Rigidbody.isKinematic = true;

            //Debug.LogFormat(Constant.Logger.loggerFormat3, GetType(), System.Reflection.MethodBase.GetCurrentMethod().Name, Name);
        }

        private void Turn() {
            // Determine the number of degrees to be turned based on the input, speed and time between frames.
            float turn = m_TurnInputValue * m_TrusterData.TurnSpeed * Time.deltaTime;

            // Make this into a rotation in the y axis.
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

            // Apply this rotation to the rigidbody's rotation.
            m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
        }

        private void Move() {
            // Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
            Vector3 movement = -transform.forward * m_MovementInputValue * m_TrusterData.Speed * Time.deltaTime;

            // Apply this movement to the rigidbody's position.
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
        }
    }
}