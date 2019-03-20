//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2019-03-20 21:28:05.806
//------------------------------------------------------------

using GameFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace TankBattle
{
    /// <summary>
    /// 武器表。
    /// </summary>
    public class DRWeapon : DataRowBase
    {
        private int m_Id = 0;

        /// <summary>
        /// 获取武器编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// 获取攻击间隔。
        /// </summary>
        public float AttackInterval
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取子弹编号。
        /// </summary>
        public int BulletId
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取蓄力声音编号。
        /// </summary>
        public int BulletChargingSoundId
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取炮弹开火声音编号。
        /// </summary>
        public int BulletFiringSoundId
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取最小蓄力。
        /// </summary>
        public float MinLaunchForce
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取最大蓄力。
        /// </summary>
        public float MaxLaunchForce
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取蓄力的最长时间。
        /// </summary>
        public float MaxChargeTime
        {
            get;
            private set;
        }

        public override bool ParseDataRow(GameFrameworkSegment<string> dataRowSegment)
        {
            // Star Force 示例代码，正式项目使用时请调整此处的生成代码，以处理 GCAlloc 问题！
            string[] columnTexts = dataRowSegment.Source.Substring(dataRowSegment.Offset, dataRowSegment.Length).Split(DataTableExtension.DataSplitSeparators);
            for (int i = 0; i < columnTexts.Length; i++)
            {
                columnTexts[i] = columnTexts[i].Trim(DataTableExtension.DataTrimSeparators);
            }

            int index = 0;
            index++;
            m_Id = int.Parse(columnTexts[index++]);
            index++;
            AttackInterval = float.Parse(columnTexts[index++]);
            BulletId = int.Parse(columnTexts[index++]);
            BulletChargingSoundId = int.Parse(columnTexts[index++]);
            BulletFiringSoundId = int.Parse(columnTexts[index++]);
            MinLaunchForce = float.Parse(columnTexts[index++]);
            MaxLaunchForce = float.Parse(columnTexts[index++]);
            MaxChargeTime = float.Parse(columnTexts[index++]);

            GeneratePropertyArray();
            return true;
        }

        public override bool ParseDataRow(GameFrameworkSegment<byte[]> dataRowSegment)
        {
            // Star Force 示例代码，正式项目使用时请调整此处的生成代码，以处理 GCAlloc 问题！
            using (MemoryStream memoryStream = new MemoryStream(dataRowSegment.Source, dataRowSegment.Offset, dataRowSegment.Length, false))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
                {
                    m_Id = binaryReader.ReadInt32();
                    AttackInterval = binaryReader.ReadSingle();
                    BulletId = binaryReader.ReadInt32();
                    BulletChargingSoundId = binaryReader.ReadInt32();
                    BulletFiringSoundId = binaryReader.ReadInt32();
                    MinLaunchForce = binaryReader.ReadSingle();
                    MaxLaunchForce = binaryReader.ReadSingle();
                    MaxChargeTime = binaryReader.ReadSingle();
                }
            }

            GeneratePropertyArray();
            return true;
        }

        public override bool ParseDataRow(GameFrameworkSegment<Stream> dataRowSegment)
        {
            Log.Warning("Not implemented ParseDataRow(GameFrameworkSegment<Stream>)");
            return false;
        }

        private void GeneratePropertyArray()
        {

        }
    }
}
