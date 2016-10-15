using System;
using UnityEngine;

public class ViewConstants
{

    public const float INITIAL_Y = 3f;
    public const float Y_INCREMENT = 0.475f;

    private static Color yellow = new Color(1F, 1F, 0F);
    private static Color blue = new Color(0F, 0F, 1F);
    private static Color black = new Color(0F, 0F, 0F);

    private ViewConstants()
    {

    }

    public static Color YELLOW
    {
        get
        {
            return yellow;
        }
    }

    public static Color BLUE
    {
        get
        {
            return blue;
        }
    }

    public static Color BLACK
    {
        get
        {
            return black;
        }
    }

    public static Color getColourForPlayerNumberAndSide(PlayerNumber playerNumber, Side player1side) {
        if (PlayerNumber.PLAYER1 == playerNumber) {
            if (Side.LEFT == player1side) {
                return yellow;
            }
            else {
                return blue;
            }
        }
        else if (PlayerNumber.PLAYER2 == playerNumber) {
            if (Side.LEFT == player1side) {
                return blue;
            }
            else {
                return yellow;
            }
        }
        else  {
            return black;
        }

    }
}

