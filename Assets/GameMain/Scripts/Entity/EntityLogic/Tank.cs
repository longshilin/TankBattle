using GameFramework;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace TankBattle {

    /// <summary>
    /// 坦克类
    ///     装备各种物件： 推进器、武器、盔甲
    /// </summary>
    public class Tank : TargetableObject {

        [SerializeField]
        private TankData m_TankData = null;

        [SerializeField]
        protected Thruster m_Thruster = null;

        [SerializeField]
        protected Weapon m_Weapon = new Weapon();

        [SerializeField]
        protected Armor m_Armor = new Armor();

        /// <summary>
        /// 实体展示模块：负责加载Tank身上的各种附属实体。
        /// </summary>
        protected override void OnShow(object userData) {
            base.OnShow(userData);

            m_TankData = userData as TankData;
            if (m_TankData == null) {
                Log.Error("Tank data is invalid.");
                return;
            }

            Name = Utility.Text.Format("Tank ({0})", Id.ToString());

            GameEntry.Entity.ShowThruster(m_TankData.GetThrusterData());

            GameEntry.Entity.ShowWeapon(m_TankData.GetWeaponData());

            GameEntry.Entity.ShowArmor(m_TankData.GetArmorData());

            // 坦克初始血条展示
            GameEntry.HPBar.ShowHPBar(this, 1, 1);
        }

        protected override void OnHide(object userData) {
            base.OnHide(userData);
        }

        // 给坦克装备各种物件
        protected override void OnAttached(EntityLogic childEntity, Transform parentTransform, object userData) {
            base.OnAttached(childEntity, parentTransform, userData);

            if (childEntity is Thruster) {
                m_Thruster = (Thruster)childEntity;
                m_Thruster.m_Rigidbody = GetComponent<Rigidbody>();
                m_Thruster.m_MovementAudio = GetComponent<AudioSource>();
                return;
            }

            if (childEntity is Weapon) {
                m_Weapon = (Weapon)childEntity;
                //m_Weapon.m_AimSlider = GetComponentInChildren<Canvas>().GetComponent<Slider>();
                //Debug.Log("Slider " + GetComponentInChildren<Canvas>().GetComponent<Slider>() + " - " + GetComponent<Rigidbody>());
                return;
            }

            if (childEntity is Armor) {
                m_Armor = (Armor)childEntity;
                return;
            }
        }

        // 给坦克卸载各种物件
        protected override void OnDetached(EntityLogic childEntity, object userData) {
            base.OnDetached(childEntity, userData);

            if (childEntity is Thruster) {
                m_Thruster = null;
                return;
            }

            if (childEntity is Weapon) {
                m_Weapon = null;
                return;
            }

            if (childEntity is Armor) {
                m_Armor = null;
                return;
            }
        }

        // 触发坦克死亡
        protected override void OnDead(Entity attacker) {
            base.OnDead(attacker);

            // tank爆炸特效和爆炸音效
            GameEntry.Entity.ShowEffect(new EffectData(GameEntry.Entity.GenerateSerialId(), m_TankData.DeadEffectId) {
                Position = CachedTransform.localPosition,
            });
            GameEntry.Sound.PlaySFX(m_TankData.DeadSoundId);
        }

        public override ImpactData GetImpactData() {
            return new ImpactData(m_TankData.Camp, m_TankData.HP, 0, m_TankData.Defense);
        }

        // 在游戏阶段中，玩家不能控制他们的坦克
        public void DisableControl() {
            if (m_Thruster != null)
                m_Thruster.enabled = false;

            if (m_Weapon != null)
                m_Weapon.enabled = false;
        }

        // 在游戏阶段中，玩家可以控制他们的坦克
        public void EnableControl() {
            if (m_Thruster != null)
                m_Thruster.enabled = true;

            if (m_Weapon != null)
                m_Weapon.enabled = true;
        }

        // 重置坦克
        public void Reset() {
        }
    }
}