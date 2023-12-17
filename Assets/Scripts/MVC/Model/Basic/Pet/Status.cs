using System;
using System.Linq;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status
{
    public static Status zero { get => new Status(0); }
    public static Status one { get => new Status(1); }
    public static string[] typeNames => new string[6] { "atk", "mat", "def", "mdf", "spd", "hp" };
    public static string[] typeNamesChinese => new string[6] { "物攻", "特攻", "物防", "特防", "速度", "体力" };

    protected float[] status = new float[6];
    [XmlElement("atk")] public float atk { get => status[0]; set => status[0] = value; }
    [XmlElement("mat")] public float mat { get => status[1]; set => status[1] = value; }
    [XmlElement("def")] public float def { get => status[2]; set => status[2] = value; }
    [XmlElement("mdf")] public float mdf { get => status[3]; set => status[3] = value; }
    [XmlElement("spd")] public float spd { get => status[4]; set => status[4] = value; }
    [XmlElement("hp")] public float hp { get => status[5]; set => status[5] = value; }
    public float this[int n] {
        get => Get(n);
        set => Set(n, value);
    }

    public float this[string type] {
        get => Get(type);
        set => Set(type, value);
    }

    public float sum => status.Sum();
    public Status sign => Status.Sign(this);
    public Status abs => Status.Abs(this);
    public Status pos => this.Select(x => Mathf.Max(x, 0));
    public Status neg => this.Select(x => Mathf.Min(x, 0));


    #region constructors and utility

    public Status() {}

    public Status(Status copy) {
        for(int i = 0; i < status.Length; i++) {
            status[i] = copy.status[i];
        }
    }
    public Status(float ability) {
        for(int i = 0; i < status.Length; i++) {
            status[i] = ability;
        }  
    }
    public Status(IEnumerable<float> array) {
        status = array.Where((x, i) => i < 6).ToArray();
    }
    public Status(float atk, float mat, float def, float mdf, float spd, float hp) {
        this.atk = atk;     
        this.mat = mat;
        this.def = def;
        this.mdf = mdf;
        this.spd = spd;
        this.hp = hp;
    }

    public override string ToString() {
        Status floorStatus = Status.FloorToInt(this);
        string repr = string.Empty;

        for (int i = 0; i < 6; i++) {
            repr += typeNames[i] + ": " + floorStatus[i].ToString() + " ";
        }
        return repr;
    }

    public virtual float[] ToArray() {
        return status.ToArray();
    }

    public virtual float Get(int type) {
        if (!type.IsWithin(0, 5))
            return 0;

        return status[type];
    }

    public virtual void Set(int type, float value) {
        if (!type.IsWithin(0, 5))
            return;

        status[type] = value;
    }

    public virtual float Get(string type) {
        int idx = typeNames.IndexOf(type);
        return (idx == -1) ? 0 : status[typeNames.IndexOf(type)];
    }

    public virtual void Set(string type, float value) {
        int idx = typeNames.IndexOf(type);
        if (idx == -1)
            return;

        status[typeNames.IndexOf(type)] = value;
    }

    public Status Select(Func<float, float> selector) {
        return new Status(status.Select(selector).ToArray());
    }

    public Status Select(Func<float, int, float> selector) {
        return new Status(status.Select(selector).ToArray());
    }

    public int Count(Func<float, bool> pred) {
        return status.Count(pred);
    }

    #endregion

    #region math functions

    public static Status FloorToInt(Status value, Func<float, int> floorFunc = null) { 
        floorFunc = floorFunc ?? Mathf.FloorToInt;
        return value.Select(x => floorFunc(x));
    }
    public static Status Sign(Status value) {
        return value.Select((x, i) => Mathf.Sign(value[i]));
    }
    public static Status Abs(Status value) {
        return value.Select((x, i) => Mathf.Abs(value[i]));
    }
    public static Status Min(Status lhs, Status rhs) {
        return lhs.Select((x, i) => Mathf.Min(lhs[i], rhs[i]));
    }
    public static Status Max(Status lhs, Status rhs) {
        return lhs.Select((x, i) => Mathf.Max(lhs[i], rhs[i]));
    }
    public static Status Clamp(Status value, Status min, Status max) {
        return value.Select((x, i) => Mathf.Clamp(value[i], min[i], max[i]));
    }
    public static Status Pow(Status value, float power) {
        return value.Select((x, i) => Mathf.Pow(value[i], power));
    }
    public static Status Pow(Status value, Status power) {
        return value.Select((x, i) => Mathf.Pow(value[i], power[i]));
    }

    #endregion

    #region operator
    public static Status operator +(float lhs, Status rhs) {
        return rhs + lhs;
    }
    public static Status operator +(Status lhs, float rhs) {
        return lhs + new Status(rhs);
    }
    public static Status operator +(Status lhs, Status rhs) {
        return lhs.Select((x, i) => lhs[i] + rhs[i]);
    }

    public static Status operator -(Status lhs, float rhs) {
        return lhs - new Status(rhs);
    }
    public static Status operator -(Status lhs, Status rhs) {
        return lhs + rhs * (-1);
    }

    public static Status operator *(float lhs, Status rhs) {
        return rhs * lhs;
    }
    public static Status operator *(Status lhs, float rhs) {
        return lhs * new Status(rhs);
    }    
    public static Status operator *(Status lhs, Status rhs) {
        return lhs.Select((x, i) => lhs[i] * rhs[i]);
    }

    public static Status operator /(Status lhs, float rhs) {
        return lhs * (1 / rhs);
    }

    public static Status operator ^(Status lhs, float rhs) {
        return Status.Pow(lhs, rhs);
    }
    public static Status operator ^(Status lhs, Status rhs) {
        return Status.Pow(lhs, rhs);
    }

    #endregion

    public static Status GetPersonalityBuff(Personality p) {
        int id = (int)p;
        int up = id / 5;
        int down = id % 5;
        Status buff = Status.one;
        buff.status[up] += 0.1f;
        buff.status[down] -= 0.1f;
        return buff;
    }

    public static string GetPersonalityBuffDescription(Personality p) {
        int id = (int)p;
        int up = id / 5;
        int down = id % 5;
        return (up == down) ? "平衡发展" : (typeNamesChinese[up] + "+10%\n" + typeNamesChinese[down] + "-10%");
    }

    public static Status GetPowerUpBuff(Status powerup) {
        Status sign = powerup.sign;
        Status abs = Status.Abs(powerup);
        return (1 + 0.5f * abs) ^ (powerup.sign);
    }

    public static Status GetPetNormalStatus(int level, Status baseStatus, int iv, Status ev, Personality p) {
        Status normalStatus = (((((baseStatus * 2) + iv + (ev / 4)) * level) / 100) + 5) * Status.GetPersonalityBuff(p);
        Status hpStatus = ((((baseStatus * 2) + iv + (ev / 4)) * level) / 100) + 10 + level;
        normalStatus.hp = hpStatus.hp;
        return Status.FloorToInt(normalStatus);
    }
}

public class BattleStatus : Status {
    public static string[] battleTypeNames => typeNames.Concat(hiddenTypeNames).ToArray();
    public static string[] battleTypeNamesChinese => typeNamesChinese.Concat(hiddenTypeNamesChinese).ToArray();

    public static string[] hiddenTypeNames => new string[6] { "hit", "cri", "eva", "cdf", "rec", "angrec" };
    public static string[] hiddenTypeNamesChinese => new string[6] { "命中", "暴击", "闪避", "暴抗", "回复率", "怒气回复率" };

    protected float[] hiddenStatus = new float[6];
    [XmlElement("hit")] public float hit { get => hiddenStatus[0]; set => hiddenStatus[0] = value; }
    [XmlElement("cri")] public float cri { get => hiddenStatus[1]; set => hiddenStatus[1] = value; }
    [XmlElement("eva")] public float eva { get => hiddenStatus[2]; set => hiddenStatus[2] = value; }
    [XmlElement("cdf")] public float cdf { get => hiddenStatus[3]; set => hiddenStatus[3] = value; }
    [XmlElement("rec")] public float rec { get => hiddenStatus[4]; set => hiddenStatus[4] = value; }
    [XmlElement("angrec")] public float angrec { get => hiddenStatus[5]; set => hiddenStatus[5] = value; }
    public new float this[int n] {
        get => (n <= 5) ? status[n] : hiddenStatus[n-6];
        set => SetHiddenStatus(n, value);
    }

    public override string ToString() {
        Status floorStatus = Status.FloorToInt(new Status(hiddenStatus));
        string repr = string.Empty;
        for (int i = 0; i < hiddenStatus.Length; i++) {
            repr += (hiddenTypeNames[i] + ": " + floorStatus[i] + " ");
        }
        return base.ToString() + "\n" + repr;
    }

    public override float Get(int type)
    {
        if (!type.IsWithin(0, 11))
            return 0;

        return (type <= 5) ? status[type] : hiddenStatus[type - 6];
    }

    public override void Set(int type, float value)
    {
        if (!type.IsWithin(0, 11))
            return;

        if (type <= 5) {
            base.Set(type, value);
            return;
        }
        hiddenStatus[type - 6] = value;
    }

    public override float Get(string type) {
        int idx = battleTypeNames.IndexOf(type);
        if (idx == -1)
            return 0;

        return idx <= 5 ? status[idx] : hiddenStatus[idx - 6];
    }

    public override void Set(string type, float value) {
        int idx = battleTypeNames.IndexOf(type);
        if (idx == -1)
            return;

        if (idx <= 5) {
            status[idx] = value;
            return;
        }

        hiddenStatus[idx - 6] = value;
    }

    public Status GetBasicStatus() {
        return new Status(status);
    }
    public Status GetHiddenStatus() {
        return new Status(hiddenStatus);
    }

    public BattleStatus() {
        rec = 100;
        angrec = 100;
    }

    public BattleStatus(Status normalStatus, Status extraStatus) {
        for(int i = 0; i < status.Length; i++) {
            status[i] = normalStatus[i];
        } 
        for(int i = 0; i < hiddenStatus.Length; i++) {
            hiddenStatus[i] = extraStatus[i];
        }
    }

    public static BattleStatus FloorToInt(BattleStatus value, Func<float, int> floorFunc = null) { 
        Status normal = new Status(value.status);
        Status hidden = new Status(value.hiddenStatus);
        return new BattleStatus(Status.FloorToInt(normal, floorFunc), Status.FloorToInt(hidden, floorFunc));
    }

    public void SetHiddenStatus(int type, float value) {
        if (type <= 5)
            status[type] = value;
        else 
            hiddenStatus[type - 6] = value;
    }
}

public enum StatusType {
    Atk = 0,
    Mat = 1,
    Def = 2,
    Mdf = 3,
    Spd = 4,
    Hp = 5,
    Hit = 6,
    Cri = 7,
    Eva = 8,
    Cdf = 9,
    Rec = 10,
    AngRec = 11,
}

public enum HiddenStatusType {
    Hit = 0,
    Cri = 1,
    Eva = 2,
    Cdf = 3,
    Rec = 4,
    AngRec = 5,
}