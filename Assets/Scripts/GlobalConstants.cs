using UnityEngine;
using System.Collections;

public static class GlobalConstants
{
    public const int UP = 1;
    public const int DOWN = -1;
    public const int LEFT = -1;
    public const int RIGHT = 1;

    public enum Direction_Indices { UP, DOWN, LEFT, RIGHT };

    private static Vector2 upVector = new Vector2(0, UP);
    private static Vector2 downVector = new Vector2(0, DOWN);
    private static Vector2 leftVector = new Vector2(LEFT, 0);
    private static Vector2 rightVector = new Vector2(RIGHT, 0);

    public static Vector2 UP_VECTOR
    { get { return upVector; } }

    public static Vector2 DOWN_VECTOR
    { get { return downVector; } }

    public static Vector2 LEFT_VECTOR
    { get { return leftVector; } }

    public static Vector2 RIGHT_VECTOR
    { get { return rightVector; } }
}
