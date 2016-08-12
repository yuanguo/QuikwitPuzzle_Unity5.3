using UnityEngine;
using System;
using System.Collections;

using System.Collections.Generic;

using Ucss;
using MiniJSON;

public enum RequestState
{
	none = 0,
	RequestLogin = 1,
	RequestRegister = 2,
	RequestUserInfo = 3,
    RequestGameInfo = 4,
    RequestWorldInfo = 5,
	RequestCheckDate,
	RequestUpdatePoints,
	RequestPrizeWin,
	RequestUpdatePowerUp,
	RequestGetPowerUp,
	RequestForgotenPassword,
}

