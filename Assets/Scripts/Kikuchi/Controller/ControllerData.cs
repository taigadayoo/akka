public class ControllerData
{
    public PlayerType PlayerType { get; private set; }
    public ActionType ActionType { get; private set; }
    public ButtonType? ButtonType { get; private set; }
    public StickDirection? StickDirection { get; private set; }

    public ControllerData(PlayerType playerType, ButtonType buttonType)
    {
        PlayerType = playerType;
        ActionType = ActionType.Buttons;
        ButtonType = buttonType;
    }

    public ControllerData(PlayerType playerType, StickDirection stickDirection)
    {
        PlayerType = playerType;
        ActionType = ActionType.Sticks;
        StickDirection = stickDirection;
    }

    public override string ToString()
    {
        if (ButtonType.HasValue)
            return $"Player: {PlayerType}, Button: {ButtonType.Value}";
        if (StickDirection.HasValue)
            return $"Player: {PlayerType}, Stick Direction: {StickDirection.Value}";
        return $"Player: {PlayerType}, No Input";
    }
}


public enum PlayerType
{
    Player1,
    Player2,
}

public enum ActionType
{
    Buttons,
    Sticks,
}

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

public enum StickDirection
{
    Up,
    Down,
    Left,
    Right,
    None,
}