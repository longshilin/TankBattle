using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankBattle {
    /*
public class GameManager : ScriptBase
{
    public static GameManager Instance;
    private Dictionary<uint, Actor> mActorDic = new Dictionary<uint, Actor>();

    private Transform mActorParent;

    private FrameData mFrameData;
    private LockStep mLockStep;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _PhysicalSystem.Open();
        _UnityUdpSocket.Open();
        mFrameData = new FrameData();
        mLockStep = gameObject.AddComponent<LockStep>();

        for (int i = 0; i < NetData.Instance.mFightData.PlayerHeroInfoList.Count; i++)
        {
            PlayerHeroInfo info = NetData.Instance.mFightData.PlayerHeroInfoList[i];
            uint userid = info.userid;
            uint heroid = info.heroid;

            GameObject actorobj = _AssetManager.GetGameObject("prefab/hero/yase/yase_prefab");
            Actor actor;
            if (userid == NetData.Instance.mUserData.Userid)
                actor = actorobj.AddComponent<PlayerActor>();
            else
                actor = actorobj.AddComponent<RoleActor>();
            if (mActorParent == null)
                mActorParent = GameObject.Find("ActorParent").transform;
            actor.transform.parent = mActorParent;
            actor.Position = new CustomVector3(0, 0, 0);
            actor.Speed = new FixedPointF(300,100);
            actor.Angle = 0;
            actor.Id = userid;
            AddActor(userid, actor);
        }

        _UnityUdpSocket.Connect();
    }

    public void AddActor(uint tUserid, Actor tActor)
    {
        mActorDic[tUserid] = tActor;
    }

    public void RemoveActor(uint tUserid)
    {
        Destroy(mActorDic[tUserid].gameObject);
        mActorDic.Remove(tUserid);
    }

    public Actor GetActor(uint tUserid)
    {
        Actor actor = null;
        mActorDic.TryGetValue(tUserid, out actor);
        return actor;
    }

    public void AddOneFrame(uint frameindex, List<GameMessage> list)
    {
        mFrameData.AddOneFrame(frameindex, list);
    }

    public bool LockFrameTurn(ref List<GameMessage> list)
    {
        return mFrameData.LockFrameTurn(ref list);
    }

    public void SetFaseForward(int tValue)
    {
        mLockStep.SetFaseForward(tValue);
    }

    public void UpdateEvent()
    {
        UpdateActor();
        UpdateCollider();
    }

    void UpdateActor()
    {
        var enumerator = mActorDic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            enumerator.Current.Value.UpdateEvent();
        }
        enumerator.Dispose();
    }

    void UpdateCollider()
    {
        _PhysicalSystem.UpdateCollider();
    }

    void OnDestroy()
    {
        Instance = null;
        _PhysicalSystem.Close();
        _UnityUdpSocket.Close();
    }
}
*/
}