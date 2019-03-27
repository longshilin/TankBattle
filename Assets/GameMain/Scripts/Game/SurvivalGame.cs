//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using GameFramework.DataTable;
using UnityEngine;

namespace TankBattle {

    public class SurvivalGame : GameBase {

        public override GameMode GameMode {
            get {
                return GameMode.Survival;
            }
        }
    }
}