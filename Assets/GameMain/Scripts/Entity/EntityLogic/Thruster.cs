using GameFramework;
using GameFramework.Fsm;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace TankBattle {

    /// <summary>
    /// 坦克履带实体： 控制坦克的机动性
    /// </summary>
    public class Thruster : Entity {
        public AudioClip m_EngineIdling;            // Audio to play when the tank isn't moving.
        public AudioClip m_EngineDriving;           // Audio to play when the tank is moving.

        public bool m_ThrusterEnable = true;        // 表示Thruster是否可用，用于在状态机中判断是够对操纵杆进行取值；

        private ThrusterData m_TrusterData = null;
        private const string AttachPoint = "Thruster Point";    // Thruster实体的挂载点

        private string m_MovementAxisName;          // The name of the input axis for moving forward and back.
        private string m_TurnAxisName;              // The name of the input axis for turning.
        public Rigidbody m_Rigidbody;              // Reference used to move the tank.
        private float m_MovementInputValue;         // The current value of the movement input.
        private float m_TurnInputValue;             // The current value of the turn input.
        private float m_OriginalPitch;              // The pitch of the audio source at the start of the scene.

        //private int m_Angle;
        private float m_X;

        private float m_Y;

        private bool isMyTank = false;

        private GameFramework.Fsm.IFsm<Thruster> m_TankFsm;
        private FsmComponent Fsm = null;

        private ETCJoystick joy;
        private StartMoveReq startMoveReq;
        private ChangeDirReq changeDirReq;
        private EndMoveReq endMoveReq;

        private float m_PositionX = 0;
        private float m_PositionZ = 0;
        private float m_RotationY = 0;

        private TransformReq transformReq;

        public float PositionX { get => m_PositionX; set => m_PositionX = value; }
        public float PositionZ { get => m_PositionZ; set => m_PositionZ = value; }
        public float RotationY { get => m_RotationY; set => m_RotationY = value; }

        //public int Angle { get => m_Angle; set => m_Angle = value; }
        public float X { get => m_X; set => m_X = value; }

        public float Y { get => m_Y; set => m_Y = value; }
        public bool IsMyTank { get => isMyTank; set => isMyTank = value; }

        /// 第一部分被调用，Initialization模块
        private void Awake() {
            // 加载音频
            m_EngineIdling = Resources.Load<AudioClip>("Sounds/engine_idle");
            m_EngineDriving = Resources.Load<AudioClip>("Sounds/engine_driving");

            startMoveReq = new StartMoveReq();
            changeDirReq = new ChangeDirReq();
            endMoveReq = new EndMoveReq();

            transformReq = new TransformReq();
        }

        // 在tank移动上加入状态机
        protected override void OnInit(object userData) {
            base.OnInit(userData);

            m_MovementAxisName = "Vertical";
            m_TurnAxisName = "Horizontal";

            //Fsm = UnityGameFramework.Runtime.GameEntry.GetComponent<FsmComponent>();

            ///* Tank的所有状态 */
            //FsmState<Thruster>[] tankStates = new FsmState<Thruster>[] {
            //    new TankIdleState(),
            //    new TankMoveState(),
            //};

            //// 创建状态机
            //m_TankFsm = Fsm.CreateFsm<Thruster>(this, tankStates);

            //// 启动tank静止状态
            //m_TankFsm.Start<TankIdleState>();
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

            // 获取Thruster需要操作的Tank Rigibody
            m_Rigidbody = gameObject.GetComponentInParent<Rigidbody>();

            if (isMyTank) {
                // 加入操作杆的监听事件
                joy = GameObject.FindObjectOfType<ETCJoystick>();
                if (joy != null) {
                    joy.onMoveStart.AddListener(StartMoveCallBack);
                    joy.onMove.AddListener(MoveCallBack);
                    joy.onMoveEnd.AddListener(EndMoveCallBack);
                }
            }
        }

        private void StartMoveCallBack() {
            m_MovementInputValue = 0;
            m_TurnInputValue = 0;
            // 不做任何动作
            if (!GameEntry.NetData.mFightData.GameInitSuccess) {
                Debug.Log("需等待其他玩家准备就绪...");
                return;
            }

            startMoveReq.UserId = GameEntry.NetData.mUserData.UserId;
            startMoveReq.RoomId = GameEntry.NetData.mFightData.RoomId;
            //Debug.Log("发送StartMove帧命令!!!!!!!!!!!!");
            NetWorkChannel.send(startMoveReq);  // 发送帧命令
        }

        private void MoveCallBack(Vector2 tVec2) {
            if (!GameEntry.NetData.mFightData.GameInitSuccess) {
                Debug.Log("需等待其他玩家准备就绪...");
                return;
            }

            changeDirReq.UserId = GameEntry.NetData.mUserData.UserId;
            changeDirReq.RoomId = GameEntry.NetData.mFightData.RoomId;

            //发送遥杆角度
            //if (tVec2.x != 0) {
            changeDirReq.DirX = tVec2.x;
            changeDirReq.DirY = tVec2.y;

            //int angle = (int)(Mathf.Atan2(tVec2.y, tVec2.x) * 180 / Mathf.PI);
            //changeDirReq.Angle = angle;

            //Debug.Log("发送帧命令 - 遥杆角度 : " + angle + "°");
            //}
            //else {
            //    int angle = tVec2.y > 0 ? 90 : -90;
            //    changeDirReq.Angle = angle;
            //}
            //Debug.Log("发送帧命令!!!!!!!!!!!!");
            NetWorkChannel.send(changeDirReq);  // 发送帧命令
                                                //Debug.Log(tVec2.x + "  --  " + tVec2.y);
        }

        private void EndMoveCallBack() {
            m_MovementInputValue = 0;
            m_TurnInputValue = 0;

            // 不做任何动作
            if (!GameEntry.NetData.mFightData.GameInitSuccess) {
                Debug.Log("需等待其他玩家准备就绪...");
                return;
            }
            endMoveReq.UserId = GameEntry.NetData.mUserData.UserId;
            endMoveReq.RoomId = GameEntry.NetData.mFightData.RoomId;
            //Debug.Log("发送EenMove帧命令!!!!!!!!!!!!");
            NetWorkChannel.send(endMoveReq);  // 发送帧命令
        }

        /// 将引擎实体附加到坦克父实体上
        protected override void OnAttachTo(EntityLogic parentEntity, Transform parentTransform, object userData) {
            base.OnAttachTo(parentEntity, parentTransform, userData);

            Name = Utility.Text.Format("Thruster of {0}", parentEntity.Name);
            CachedTransform.localPosition = Vector3.zero;
            //Debug.LogFormat(Constant.Logger.loggerFormat4, GetType(), System.Reflection.MethodBase.GetCurrentMethod().Name, Name, "Attach a Thruster instance to " + parentEntity.name);
        }

        /// 第三步被调用，Initialization模块
        // 定义方向按键
        private void Start() {
            // Store the original pitch of the audio source.
            //m_OriginalPitch = m_MovementAudio.pitch;

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

                m_ThrusterEnable = true;
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
            // 不做任何动作
            if (!GameEntry.NetData.mFightData.GameInitSuccess) {
                Debug.Log("需等待其他玩家准备就绪...");
                return;
            }
            // 如果Tank是本客户端，发送该tank坐标
            //if (isMyTank) {
            //    transformReq.UserId = GameEntry.NetData.mUserData.UserId;
            //    transformReq.RoomId = GameEntry.NetData.mFightData.RoomId;
            //    // 发送tank坐标和朝向
            //    transformReq.PositionX = GetComponentInParent<MyTank>().transform.position.x;
            //    transformReq.PositionZ = GetComponentInParent<MyTank>().transform.position.z;
            //    transformReq.RotationY = GetComponentInParent<MyTank>().transform.rotation.y;

            //    NetWorkChannel.send(transformReq);
            //    Debug.Log("发送客户端本地玩家的位置给服务器！！！！！");
            //}

            m_MovementInputValue = m_Y;
            m_TurnInputValue = m_X;
            //Debug.Log("Tank m_Angle: " + m_Angle + " Mathf.Cos(m_Angle)" + Mathf.Cos(m_Angle) + " Mathf.Sin(m_Angle): " + Mathf.Sin(m_Angle));
            // 将angle角度转为x值和y值
            //m_MovementInputValue = Mathf.Sin(m_Angle);
            //m_TurnInputValue = Mathf.Cos(m_Angle);
            //Debug.Log("记录遥杆x : " + m_MovementInputValue);
            //Debug.Log("记录遥杆y : " + m_TurnInputValue);
            EngineAudio();
        }

        private void EngineAudio() {
            // 坦克移动时候的音效管理器
            if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f) {
                GameEntry.Sound.PlayDriving(m_TrusterData.IdleSound);
            }
            else {
                GameEntry.Sound.PlayDriving(m_TrusterData.DriveSound);
            }
        }

        /// 最后一步被调用，Disable模块
        private void OnDisable() {
            // When the tank is turned off, set it to kinematic so it stops moving.
            m_Rigidbody.isKinematic = true;

            m_ThrusterEnable = false;

            // 消耗joy监听事件
            if (joy != null) {
                joy.onMoveStart.RemoveListener(StartMoveCallBack);
                joy.onMove.RemoveListener(MoveCallBack);
                joy.onMoveEnd.RemoveListener(EndMoveCallBack);
            }

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

        // 状态机中也可以收集操作杆的数据进行Move
        public void Move(float movementInputValue) {
            // Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
            Vector3 movement = -transform.forward * movementInputValue * m_TrusterData.Speed * Time.deltaTime;

            // Apply this movement to the rigidbody's position.
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
        }

        public void Move() {
            // Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
            Vector3 movement = -transform.forward * m_MovementInputValue * m_TrusterData.Speed * Time.deltaTime;

            // Apply this movement to the rigidbody's position.
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
        }
    }
}