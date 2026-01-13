public static class StaticValues
{
    public const int CHECKPOINT_LEVEL = 80;

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
public static class TagKeys 
{ 
    public const string PLAYER = "Player";
    public const string ENEMY = "Enemy";
    public const string ITEM_BOX = "ItemBox";
    public const string ITEM = "Item";
    public const string PLAYER_AREA = "PlayerArea";
}
public static class LayerKeys
{
    public const string ENEMY = "Enemy";
    public const string WEAPON = "Weapon";
    public const string PLAYER = "Player";
}
public static class SceneNames
{
    public const string TITLE = "01_Title";
    public const string GAME = "02_Game";
    public const string RESULT = "00_Init";
}
