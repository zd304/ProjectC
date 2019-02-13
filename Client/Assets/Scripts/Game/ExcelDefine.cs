﻿using System;
using UnityEngine;

public class excel_scn_list : ExcelBase<excel_scn_list>
{
    public string name;
    public int temp;
    public float[] min;
    public float[] max;
    public float viewDist;
}

public class excel_cha_class : ExcelBase<excel_cha_class>
{
    public string name;
    public int chaListID;
    public int[] skillIDs;
    public int chaAtbID;
}

public class excel_cha_list : ExcelBase<excel_cha_list>
{
    public string name;
    public string path;
    public float[] scale;
    public float radius;
    public int campID;
}

public class excel_anim_list : ExcelBase<excel_anim_list>
{
    public string name;
}

public class excel_skill_list : ExcelBase<excel_skill_list>
{
    public string name;
    public int[] stages;
    public int[] hits;
    public int trait;
    public float maxDistance;
    public int targetType;
    public int skillPreOpType;
    public int skillPreOpData1;
    public int skillPreOpData2;
    public int phyDamage;
    public int magDamage;
    public int phyPct;
    public int magPct;
}

public class excel_skill_stage : ExcelBase<excel_skill_stage>
{
    public string name;
    public int[] events;
    public int trait;
    public int time;
    public int nextStageID;
}

public class excel_skill_event : ExcelBase<excel_skill_event>
{
    public string name;
    public int triggerType;
    public int triggerParam1;
    public int triggerParam2;
    public int eventType;
    public int evnetParam1;
    public int evnetParam2;
    public int evnetParam3;
    public int evnetParam4;
    public int evnetParam5;
    public int evnetParam6;
    public int evnetParam7;
    public int evnetParam8;
    public string evnetParam9;
    public int trait;
}

public class excel_skill_hit : ExcelBase<excel_skill_hit>
{
    public string name;
    public int skillID;
    public int hitType;
    public int hitData1;
    public int hitData2;
    public int hitData3;
    public int targetType;
    public int maxHitCount;
    public int targetMaxHitCnt;
	public int hurtType;
}

public class excel_child_object : ExcelBase<excel_child_object>
{
    public string name;
    public string path;
    public int duration;
    public float size;
    public int initPos;
    public string initHinge;
    public float yOffset;
    public int initDir;
    public int moveType;
    public float speed;
    public int[] events;
    public int trait;
}

public class excel_cha_camp_list : ExcelBase<excel_cha_camp_list>
{
    public string name;
    public int[] enemyCamps;
    public int[] friendCamps;
    public int teamFriend;
}

public class excel_state_group : ExcelBase<excel_state_group>
{
    public string name;
    public int[] stateEffectIDs;
    public int duration;
    public int type;
    public string icon;
    public int mutexID;
    public int mutexScope;
    public int mutexPriority;
    public int mutexNextID;
}

public class excel_state_effect : ExcelBase<excel_state_effect>
{
    public string name;
    public int type;
    public int[] dataIntArray;
    public string dataString;
}

public class excel_can_not_flag : ExcelBase<excel_can_not_flag>
{
    public string remark;
    public int canNotMove;
    public int canNotControl;
    public int canNotSkill;
    public int canNotSelected;
}