using GameFramework;
using UnityGameFramework.Runtime;

namespace TankBattle {

    /// <summary>
    /// Config文件加载的扩展方法
    /// </summary>
    public static class ConfigExtension {

        /// <summary>
        /// 加载配置。
        /// </summary>
        public static void LoadConfig(this ConfigComponent configComponent, string configName, LoadType loadType, object userData = null) {
            if (string.IsNullOrEmpty(configName)) {
                Log.Warning("Config name is invalid.");
                return;
            }

            configComponent.LoadConfig(configName, AssetUtility.GetConfigAsset(configName, loadType), loadType, Constant.AssetPriority.ConfigAsset, userData);
        }
    }
}