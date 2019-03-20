using UnityEngine;
using UnityGameFramework.Runtime;

namespace TankBattle
{
    public class MyTank : Tank
    {
        [SerializeField]
        private MyTankData m_MyTankData = null;

        private Vector3 m_TargetPosition = Vector3.zero;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);

            m_MyTankData = userData as MyTankData;
            if (m_MyTankData == null)
            {
                Log.Error("My aircraft data is invalid.");
                return;
            }


            // 坦克出生
        }


        // 坦克在每帧动画中，需要进行的操作：
        //      1. 坦克移动
        //      2. 坦克开火
        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            // 如果按下开火键，则进行开炮
            //if (Input.GetMouseButton(0))
            //{
            //    Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //    m_TargetPosition = new Vector3(point.x, 0f, point.z);
            //}

            //Vector3 direction = m_TargetPosition - CachedTransform.localPosition;
            //if (direction.sqrMagnitude <= Vector3.kEpsilon)
            //{
            //    return;
            //}

        }
    }
}
