﻿using System;

public enum CTS
{
    CTS_Default = 0,
    CTS_Login = 1,
    CTS_Regist = 2,

    CTS_EnterLobby = 1001,
    CTS_Match = 1002,
    CTS_MatchReady = 1003,

    CTS_EnterScn = 2001,
    CTS_LoadedScn = 2002,
    CTS_PlayerMove = 2003,
}

public enum STC
{
    STC_Default             = 0,
    STC_RoomInfo            = 1,
    STC_RoomAddPlayer       = 2,
    STC_MatchSuccess        = 3,
    STC_MatchFailed         = 4,
    STC_MatchReady          = 5,
    STC_SceneReady          = 6,

    STC_ScnLoad             = 2001,
    STC_StartClienGame      = 2002,
}

public enum STS
{
    STS_Default,
    STS_CreateScn,
}