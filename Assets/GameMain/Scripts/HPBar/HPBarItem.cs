using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace TankBattle {

    public class HPBarItem : MonoBehaviour {

        [SerializeField]
        public Slider m_Slider;                             // The slider to represent how much health the tank currently has.

        [SerializeField]
        public Image m_FillImage;                           // The image component of the slider.

        [SerializeField]
        public Color m_FullHealthColor = Color.green;       // The color the health bar will be when on full health.

        [SerializeField]
        public Color m_ZeroHealthColor = Color.red;         // The color the health bar will be when on no health.

        private float m_StartingHealth = 100;              // The amount of health each tank starts with.
        private float m_CurrentHealth;                      // How much health the tank currently has.
        private bool m_Dead;                                // Has the tank been reduced beyond zero health yet?

        private Canvas m_ParentCanvas = null;
        private RectTransform m_CachedTransform = null;
        private Entity m_Owner = null;
        private int m_OwnerId = 0;

        public Entity Owner {
            get {
                return m_Owner;
            }
        }

        private void Awake() {
            m_CachedTransform = GetComponent<RectTransform>();
            if (m_CachedTransform == null) {
                Log.Error("RectTransform is invalid.");
                return;
            }
            //Debug.LogFormat(Constant.Logger.loggerFormat4, GetType(), System.Reflection.MethodBase.GetCurrentMethod().Name, name, m_CachedTransform);
        }

        // Init the HP Bar Item
        public void Init(Entity owner, Canvas parentCanvas, float fromHPRatio, float toHPRatio) {
            //Debug.Log(GetType() + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            if (owner == null) {
                Log.Error("Owner is invalid.");
                return;
            }

            m_ParentCanvas = parentCanvas;
            gameObject.SetActive(true);

            if (m_Owner != owner || m_OwnerId != owner.Id) {
                m_Owner = owner;
                m_OwnerId = owner.Id;
            }

            // When the tank is enabled, reset the tank's health and whether or not it's dead.
            m_Dead = false;

            // Update the health slider's value and color.
            SetHealthUI(toHPRatio);
            Refresh();

            //Debug.LogFormat(Constant.Logger.loggerFormat3, GetType(), System.Reflection.MethodBase.GetCurrentMethod().Name, name);
        }

        // 将血条UI组件的坐标，与绑定实体的坐标进行同步，达到绑定的效果。
        public bool Refresh() {
            if (m_Dead) {
                return false;
            }
            if (m_Owner != null && Owner.Available && Owner.Id == m_OwnerId) {
                m_CachedTransform.position = m_Owner.CachedTransform.position;
            }
            return true;
        }

        public void Reset() {
            m_Slider.value = m_StartingHealth;
            m_Owner = null;
            gameObject.SetActive(false);
            m_Dead = false;
            //Debug.LogFormat(Constant.Logger.loggerFormat3, GetType(), System.Reflection.MethodBase.GetCurrentMethod().Name, name);
        }

        private void SetHealthUI(float toHPRatio) {
            if (toHPRatio <= 0) {
                OnDeath();
                return;
            }

            //Debug.Log("toHPRatio = " + toHPRatio);

            // Set the slider's value appropriately.
            m_Slider.value = m_StartingHealth * toHPRatio;

            // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
            m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, toHPRatio);
        }

        private void OnDeath() {
            // Set the flag so that this function is only called once.
            m_Dead = true;

            // Turn the tank off.
            gameObject.SetActive(false);
        }
    }
}