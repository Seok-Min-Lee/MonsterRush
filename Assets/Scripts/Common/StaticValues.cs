using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class StaticValues
{

    public static int playerCharacterNum = 0;
    public static bool isWait = false;
    public static bool isClear = false;
}

public static class PlayerPrefKeys
{
    public const string VOLUME_BGM = "volumeBGM";
    public const string VOLUME_SFX = "volumeSFX";
    public const string VISIBLE_TUTORIAL = "visibleTutorial";
    public const string IS_LEFT_HAND = "isLeftHand";
}
