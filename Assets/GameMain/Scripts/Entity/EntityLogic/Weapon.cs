using GameFramework;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace TankBattle {

    /// <summary>
    /// 武器类。具有射击动作
    /// </summary>
    public class Weapon : Entity {
        public Slider m_AimSlider;                  // 坦克实体的子对象,用来显示蓄力进度条

        private const string FireAttachPoint = "Weapon Point";    // 坦克实体的子对象,用来表示炮弹产生的地方

        [SerializeField]
        private WeaponData m_WeaponData = null;

        private string m_FireButton;                // The input axis that is used for launching shells.
        private float m_CurrentLaunchForce;         // The force that will be given to the shell when the fire button is released.
        private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
        private bool m_Fired;                       // Whether or not the shell has been launched with this button press.

        /// 第一部分被调用，Initialization模块
        private void Awake() {
        }

        protected override void OnInit(object userData) {
            base.OnInit(userData);
            //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            // The fire axis is Fire button.
            m_FireButton = "Fire";
        }

        /// 附加子实体
        protected override void OnShow(object userData) {
            base.OnShow(userData);
            //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            m_WeaponData = userData as WeaponData;
            if (m_WeaponData == null) {
                Log.Error("Weapon data is invalid.");
                return;
            }

            // Attach weapon entity on fireAttachPoint
            GameEntry.Entity.AttachEntity(Entity, m_WeaponData.OwnerId, FireAttachPoint);

            // When the tank is turned on, reset the launch force and the UI
            m_CurrentLaunchForce = m_WeaponData.MinLaunchForce;

            //m_AimSlider = gameObject.transform.parent.parent.GetComponent<Slider>();
            //m_AimSlider.value = m_WeaponData.MinLaunchForce;

            // The rate that the launch force charges up is the range of possible forces by the max charge time.
            m_ChargeSpeed = (m_WeaponData.MaxLaunchForce - m_WeaponData.MinLaunchForce) / m_WeaponData.MaxChargeTime;

            //Debug.LogFormat(Constant.Logger.loggerFormat3, GetType(), System.Reflection.MethodBase.GetCurrentMethod().Name, Name);
        }

        /// 将武器实体附加到父实体上
        protected override void OnAttachTo(EntityLogic parentEntity, Transform parentTransform, object userData) {
            base.OnAttachTo(parentEntity, parentTransform, userData);
            //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            Name = Utility.Text.Format("Weapon of {0}", parentEntity.Name);
            CachedTransform.localPosition = Vector3.zero;

            //Debug.LogFormat(Constant.Logger.loggerFormat4, GetType(), System.Reflection.MethodBase.GetCurrentMethod().Name, Name, "Attach a Weapon instance to " + parentEntity.name);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds) {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (m_FireButton == null || GetComponentInParent<MyTank>() == null) {
                return;
            }

            // The slider should have a default value of the minimum launch force.
            //m_AimSlider.value = m_WeaponData.MinLaunchForce;

            // If the max force has been exceeded and the shell hasn't yet been launched...
            if (m_CurrentLaunchForce >= m_WeaponData.MaxLaunchForce && !m_Fired) {
                // ... use the max force and launch the shell.
                m_CurrentLaunchForce = m_WeaponData.MaxLaunchForce;
                Fire();
            }
            // Otherwise, if the fire button has just started being pressed...
            else if (ETCInput.GetButtonDown(m_FireButton)) {
                FireButtonDown();
            }
            // Otherwise, if the fire button is being held and the shell hasn't been launched yet...
            else if (ETCInput.GetButton(m_FireButton) && !m_Fired) {
                FireButton();
            }
            // Otherwise, if the fire button is released and the shell hasn't been launched yet...
            else if (ETCInput.GetButtonUp(m_FireButton) && !m_Fired) {
                // ... launch the shell.
                Fire();
            }
        }

        #region 将按键开火分模块来区分

        // 开火按键按下
        // if the fire button has just started being pressed...
        public void FireButtonDown() {
            // ... reset the fired flag and reset the launch force.
            m_Fired = false;
            m_CurrentLaunchForce = m_WeaponData.MinLaunchForce;

            // 音效播放
            GameEntry.Sound.PlaySFX(m_WeaponData.BulletChargingSoundId);

            //Debug.LogFormat(Constant.Logger.loggerFormat4, GetType(), System.Reflection.MethodBase.GetCurrentMethod().Name, Name, m_CurrentLaunchForce);
        }

        // 按下但还没松开，处于蓄力阶段
        // if the fire button is being held and the shell hasn't been launched yet...
        public void FireButton() {
            // Increment the launch force and update the slider.
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

            //m_AimSlider.value = m_CurrentLaunchForce;
            //Debug.LogFormat(Constant.Logger.loggerFormat4, GetType(), System.Reflection.MethodBase.GetCurrentMethod().Name, Name, m_CurrentLaunchForce);
        }

        // 发动攻击
        public void Fire() {
            // Set the fired flag so only Fire is only called once.
            m_Fired = true;

            // show炮弹并设置其属性
            GameEntry.Entity.ShowBullet(new BulletData(GameEntry.Entity.GenerateSerialId(), m_WeaponData.BulletId, m_WeaponData.OwnerId, m_WeaponData.OwnerCamp) {
                Position = CachedTransform.parent.position,
                Rotation = CachedTransform.parent.rotation,
                Velocity = m_CurrentLaunchForce * CachedTransform.parent.forward,
            });

            // 音效播放
            GameEntry.Sound.PlaySFX(m_WeaponData.BulletFiringSoundId);

            // Reset the launch force.  This is a precaution in case of missing button events.
            m_CurrentLaunchForce = m_WeaponData.MinLaunchForce;
        }

        #endregion 将按键开火分模块来区分
    }
}