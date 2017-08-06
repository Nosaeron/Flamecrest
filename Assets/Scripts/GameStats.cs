using UnityEngine;

public static class GameStats
{
    private static int stocks;
    private static int p1 = -1;
    private static int p2 = -1;

    public static int Stocks
    {
        get
        {
            return stocks;
        }
        set
        {
            stocks = value;
        }
    }
    public static int P1
    {
        get
        {
            return p1;
        }
        set
        {
            p1 = value;
        }
    }
    public static int P2
    {
        get
        {
            return p2;
        }
        set
        {
            p2 = value;
        }
    }
    
    public static Color GetColor(int teamNumber)
    {
        switch(teamNumber)
        {
            case 1:
                return Color.red;
            case 2:
                return Color.blue;
        }
        return Color.white;
    }
}
