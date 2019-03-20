using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace TankBattle {

    public class ProcedureLogin : ProcedureBase {
        private LoginForm m_LoginForm = null;
        private bool enterHome = false;

        public override bool UseNativeDialog {
            get {
                return false;
            }
        }

        public void goHome() {
            enterHome = true;
        }

        protected override void OnEnter(ProcedureOwner procedureOwner) {
            base.OnEnter(procedureOwner);

            GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);

            GameEntry.UI.OpenUIForm(UIFormId.LoginForm, this);
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown) {
            base.OnLeave(procedureOwner, isShutdown);

            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);

            if (m_LoginForm != null) {
                m_LoginForm.Close(isShutdown);
                m_LoginForm = null;
            }
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds) {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            //ChangeState<ProcedureChangeScene>(procedureOwner);

            if (enterHome) {
                procedureOwner.SetData<VarInt>(Constant.ProcedureData.NextSceneId, GameEntry.Config.GetInt("Scene.Home"));
                procedureOwner.SetData<VarInt>(Constant.ProcedureData.GameMode, (int)GameMode.Survival);
                ChangeState<ProcedureChangeScene>(procedureOwner);
            }
        }

        private void OnOpenUIFormSuccess(object sender, GameEventArgs e) {
            OpenUIFormSuccessEventArgs ne = (OpenUIFormSuccessEventArgs)e;
            if (ne.UserData != this) {
                return;
            }

            m_LoginForm = (LoginForm)ne.UIForm.Logic;
        }
    }
}