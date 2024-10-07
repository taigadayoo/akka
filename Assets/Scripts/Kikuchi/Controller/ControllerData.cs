/// <summary>
/// コントローラーのデータを保持するクラス
/// </summary>
public class ControllerData
{
    /// <summary>
    /// プレイヤーのタイプ（Player1やPlayer2など）
    /// </summary>
    public PlayerType PlayerType { get; private set; }

    /// <summary>
    /// アクションのタイプ（ボタン入力かスティック入力か）
    /// </summary>
    public ActionType ActionType { get; private set; }

    /// <summary>
    /// ボタンのタイプ（南、東、西、北など）、ボタン入力の場合に使用
    /// </summary>
    public ButtonType? ButtonType { get; private set; }

    /// <summary>
    /// スティックの方向（上、下、左、右など）、スティック入力の場合に使用
    /// </summary>
    public StickDirection? StickDirection { get; private set; }

    /// <summary>
    /// ボタン入力用のコンストラクタ
    /// </summary>
    /// <param name="playerType">プレイヤーのタイプ</param>
    /// <param name="buttonType">押されたボタンのタイプ</param>
    public ControllerData(PlayerType playerType, ButtonType buttonType)
    {
        PlayerType = playerType;
        ActionType = ActionType.Buttons;
        ButtonType = buttonType;
    }

    /// <summary>
    /// スティック入力用のコンストラクタ
    /// </summary>
    /// <param name="playerType">プレイヤーのタイプ</param>
    /// <param name="stickDirection">スティックの方向</param>
    public ControllerData(PlayerType playerType, StickDirection stickDirection)
    {
        PlayerType = playerType;
        ActionType = ActionType.Sticks;
        StickDirection = stickDirection;
    }

    /// <summary>
    /// オブジェクトの文字列表現を返します
    /// </summary>
    /// <returns>コントローラーの状態を表す文字列</returns>
    public override string ToString()
    {
        if (ButtonType.HasValue)
            return $"Player: {PlayerType}, Button: {ButtonType.Value}";
        if (StickDirection.HasValue)
            return $"Player: {PlayerType}, Stick Direction: {StickDirection.Value}";
        return $"Player: {PlayerType}, No Input";
    }
}

/// <summary>
/// プレイヤーのタイプを表す列挙型
/// </summary>
public enum PlayerType
{
    Player1,
    Player2,
}

/// <summary>
/// アクションのタイプを表す列挙型
/// </summary>
public enum ActionType
{
    Buttons,
    Sticks,
}

/// <summary>
/// ボタンのタイプを表す列挙型
/// </summary>
public enum ButtonType
{
    South,
    East,
    West,
    North,
    L1,
    R1,
    L2,
    R2,
    Up,
    Down,
    Left,
    Right,
    Option,
    Select,
}

/// <summary>
/// スティックの方向を表す列挙型
/// </summary>
public enum StickDirection
{
    Up,
    Down,
    Left,
    Right,
    None,
}
