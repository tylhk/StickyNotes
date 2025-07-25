using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace StickyNotes.Utils
{
    public static class ScreenHelper
    {
        // 缓存屏幕工作区域以减少重复计算
        private static List<Rectangle> _screenRectsCache;
        private static DateTime _lastCacheTime = DateTime.MinValue;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(5);

        private static List<Rectangle> GetScreenWorkingAreas()
        {
            if (_screenRectsCache == null || (DateTime.Now - _lastCacheTime) > CacheDuration)
            {
                _screenRectsCache = Screen.AllScreens
                    .Select(screen => screen.WorkingArea)
                    .ToList();
                _lastCacheTime = DateTime.Now;
            }

            return _screenRectsCache;
        }

        /// <summary>
        /// 判断指定的坐标点是否在任意屏幕的工作区域内
        /// </summary>
        public static bool IsPointOnAnyScreen(int x, int y)
        {
            var screenRects = GetScreenWorkingAreas();
            foreach (var rect in screenRects)
            {
                if (rect.Contains(x, y))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断指定的 Point 是否在任意屏幕的工作区域内
        /// </summary>
        public static bool IsPointOnAnyScreen(Point point)
        {
            return IsPointOnAnyScreen(point.X, point.Y);
        }

        /// <summary>
        /// 获取包含指定点的屏幕工作区域（如果没有匹配的屏幕则返回 null）
        /// </summary>
        public static Rectangle? GetScreenWorkAreaForPoint(int x, int y)
        {
            var screenRects = GetScreenWorkingAreas();
            foreach (var rect in screenRects)
            {
                if (rect.Contains(x, y))
                {
                    return rect;
                }
            }
            return null;
        }

        /// <summary>
        /// 将点约束到指定的矩形区域内（防止超出边界）
        /// </summary>
        public static Point ConstrainPointToRect(Point point, Rectangle rect)
        {
            int x = Math.Max(rect.Left, Math.Min(point.X, rect.Right));
            int y = Math.Max(rect.Top, Math.Min(point.Y, rect.Bottom));
            return new Point(x, y);
        }

        /// <summary>
        /// 判断两个矩形是否相交
        /// </summary>
        public static bool AreRectanglesIntersecting(Rectangle rect1, Rectangle rect2)
        {
            return rect1.IntersectsWith(rect2);
        }

        /// <summary>
        /// 获取主屏幕的工作区域
        /// </summary>
        public static Rectangle GetPrimaryScreenWorkArea()
        {
            return Screen.PrimaryScreen?.WorkingArea ?? Rectangle.Empty;
        }
    }
}