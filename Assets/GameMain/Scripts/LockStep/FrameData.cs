using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LitJson;
using System.IO;

using ProtoBuf;
using GameFramework.Network;

/// <summary>
/// 帧数据
/// </summary>
namespace TankBattle {

    public class FrameData {
        private int mPlayFrameIndex = 1;
        private Dictionary<int, List<Packet>> mFrameCatchDic; // 保存每个角色帧队列的字典 <UserId, 帧队列List>

        public FrameData() {
            mFrameCatchDic = new Dictionary<int, List<Packet>>();
            mPlayFrameIndex = 1;
        }

        /// <summary>
        /// 保存每个玩家的帧数据，使用lock锁机制
        /// </summary>
        public void AddOneFrame(int frameindex, List<Packet> list) {
            lock (mFrameCatchDic) {
                if (frameindex >= mPlayFrameIndex) {
                    mFrameCatchDic[frameindex] = list;

                    int speed = (int)(frameindex - mPlayFrameIndex);
                    if (speed == 0)
                        speed = 1;
                    GameEntry.LockManager.SetFaseForward(speed); // 快速播放遗留帧
                }
            }
        }

        /// <summary>
        /// 播放帧命令
        /// </summary>
        public bool LockFrameTurn(ref List<Packet> list) {
            lock (mFrameCatchDic) {
                if (mFrameCatchDic.TryGetValue(mPlayFrameIndex, out list)) {
                    //Debug.Log("执行帧id = " + mPlayFrameIndex);
                    mFrameCatchDic.Remove(mPlayFrameIndex);
                    mPlayFrameIndex++;
                    return true;
                }
                else
                    return false;
            }
        }
    }
}