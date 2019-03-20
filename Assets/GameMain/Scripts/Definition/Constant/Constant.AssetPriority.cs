//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

namespace TankBattle
{
    public static partial class Constant
    {
        /// <summary>
        /// 资源优先级。
        /// 优先级越大，越先加载。整体思路：
        ///     最先加载配置、数据表、字典集；
        ///     其次加载实体；
        ///     接着加载UI、字体；
        ///     再接着加载音效、武器、装备；
        ///     然后加载背景音；
        ///     最后加载场景资源；
        /// </summary>
        public static class AssetPriority
        {
            public const int ConfigAsset = 100;
            public const int DataTableAsset = 100;
            public const int DictionaryAsset = 100;
            public const int FontAsset = 50;
            public const int MusicAsset = 20;
            public const int SceneAsset = 0;
            public const int SoundAsset = 30;
            public const int UIFormAsset = 50;
            public const int UISoundAsset = 30;

            public const int MyTankAsset = 90;
            public const int AircraftAsset = 80;
            public const int ThrusterAsset = 30;
            public const int WeaponAsset = 30;
            public const int ArmorAsset = 30;
            public const int BulletAsset = 80;
            public const int EffectAsset = 80;
        }
    }
}
