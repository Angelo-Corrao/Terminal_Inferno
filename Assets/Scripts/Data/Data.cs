using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    // Simple class container for datas
    public class Data
    {
        [Serializable]
        public enum RouteColor
        {
            NULL,
            RED,
            ORANGE,
            YELLOW,
            SKYBLUE,
            BLUE,
            VIOLET
        }
        public enum PointType
        {
            START,
            STANDARD,
            END
        }

        public static Dictionary<RouteColor, Color> routeColorAssociation = new()
        {
            { RouteColor.NULL, Color.gray },
            { RouteColor.BLUE, Color.blue },
            { RouteColor.RED, Color.red },
            { RouteColor.YELLOW, Color.yellow },
            { RouteColor.VIOLET, Color.magenta },
            { RouteColor.ORANGE, new Color(1f,146f/255f,0f) },
            { RouteColor.SKYBLUE, new Color(0,1,1) },
        };

        [Serializable]
        public struct RouteToEnable
        {
            public RouteColor route;
            public bool enabled;
            [Range(1, 6)]
            public int group;
        }

        public static Dictionary<string, Dictionary<RouteColor, int>> airshipsDestination = new()
        {
            {"001", new() 
                {
                    {RouteColor.RED, 2},
                    {RouteColor.ORANGE, 5},
                    {RouteColor.YELLOW, 1},
                    {RouteColor.SKYBLUE, 3},
                    {RouteColor.BLUE, 4},
                    {RouteColor.VIOLET, 6}
                }
            },
            {"002", new()
                {
                    {RouteColor.RED, 1},
                    {RouteColor.ORANGE, 3},
                    {RouteColor.YELLOW, 4},
                    {RouteColor.SKYBLUE, 2},
                    {RouteColor.BLUE, 6},
                    {RouteColor.VIOLET, 5}
                }
            },
            {"003", new()
                {
                    {RouteColor.RED, 5},
                    {RouteColor.ORANGE, 2},
                    {RouteColor.YELLOW, 4},
                    {RouteColor.SKYBLUE, 6},
                    {RouteColor.BLUE, 1},
                    {RouteColor.VIOLET, 3}
                }
            },
            {"004", new()
                {
                    {RouteColor.RED, 4},
                    {RouteColor.ORANGE, 1},
                    {RouteColor.YELLOW, 6},
                    {RouteColor.SKYBLUE, 5},
                    {RouteColor.BLUE, 3},
                    {RouteColor.VIOLET, 2}
                }
            }

        };

        public static Dictionary<int, (int,int)> planeWaves = new()
        {
            {1, (2,0)},
            {2, (2,1)},
            {3, (2,1)},
            {4, (3,1)},
            {5, (3,1)},
            {6, (3,2)},
            {7, (3,2)},
            {8, (3,2)},
            {9, (4,2)},
            {10, (4,2)},
            {11, (4,2)},
            {12, (4,3)},
            {13, (4,3)},
            {14, (5,2)},
            {15, (5,3)},
            {16, (5,3)},
            {17, (6,2)},
            {18, (6,3)},
            {19, (6,3)},
            {20, (6,3)}
        };

        public static Color standardPointColor = new Color(56f/255f,118f/255f,29f/255f);

        public enum KevinState
        {
            Neutral,
            Happy,
            Angry
        }

        public static float ReMap(float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

    }
}
