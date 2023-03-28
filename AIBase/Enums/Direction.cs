using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YGOSharp.OCGWrapper.Enums;

namespace AIBase.Enums
{
    public enum Direction
    {
        U, D, L, R,
        UL, UR,
        DL, DR
    }

    public static class DirectionExtentions
    {
        public static Direction ToDirection(this CardLinkMarker m)
        {
            switch(m)
            {
                case CardLinkMarker.Top: return Direction.U;
                case CardLinkMarker.Bottom: return Direction.D;
                case CardLinkMarker.Left: return Direction.L;
                case CardLinkMarker.Right: return Direction.R;
                case CardLinkMarker.TopLeft: return Direction.UL;
                case CardLinkMarker.TopRight: return Direction.UR;
                case CardLinkMarker.BottomLeft: return Direction.DL;
                case CardLinkMarker.BottomRight: return Direction.DR;
                default: return Direction.DR; //dummy
            }
        }
    }
}
