using UnityEngine;
using System.Collections;
using GameFramework.Fsm;
using UnityGameFramework.Runtime;

namespace TankBattle {

    public class TankMoveState : FsmState<Thruster> {
        private float movementInputValue = 0;

        /// <summary>
        /// 有限状态机状态初始化时调用
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        protected override void OnInit(IFsm<Thruster> fsm) {
        }

        /// <summary>
        /// 有限状态机状态进入时调用
        /// </summary>
        /// <param name="fsm"></param>
        protected override void OnEnter(IFsm<Thruster> fsm) {
            //Log.Info("进入坦克Drive状态");
        }

        /// <summary>
        /// 有限状态机状态轮询时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        protected override void OnUpdate(IFsm<Thruster> fsm, float elapseSeconds, float realElapseSeconds) {
            /* 移动操作杆 前进或后退进行移动*/
            if (fsm.Owner.m_ThrusterEnable) {
                movementInputValue = ETCInput.GetAxis("Vertical");
                if (movementInputValue != 0) {
                    /* 移动 */
                    fsm.Owner.Move(movementInputValue);
                }
                else {
                    /* 静止 */
                    ChangeState<TankIdleState>(fsm);
                }
            }
        }

        /// <summary>
        /// 有限状态机状态离开时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        /// <param name="isShutdown">是否是关闭有限状态机时触发。</param>
        protected override void OnLeave(IFsm<Thruster> fsm, bool isShutdown) {
        }

        /// <summary>
        /// 有限状态机状态销毁时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        protected override void OnDestroy(IFsm<Thruster> fsm) {
            base.OnDestroy(fsm);
        }
    }
}