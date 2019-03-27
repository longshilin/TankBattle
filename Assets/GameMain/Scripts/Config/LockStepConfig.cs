using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockStepConfig {
    public static int mRenderFrameCount = 2;    // 渲染帧计数
    public static float mRenderFrameUpdateTime = 0.02f;
    public static FixedPointF mRenderFrameRate = new FixedPointF(1, 20);
}