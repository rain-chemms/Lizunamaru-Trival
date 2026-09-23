using System;
using UnityEngine;
using System.Collections.Generic;

namespace MapSystem.DrawPaperSystem
{
    [Serializable]
    public class Brush
    {
        [SerializeField] private Color color = Color.black;
        public void SetColor(Color color) => this.color = color;
        public Color GetColor() => color;

        [SerializeField] private int size = 10;
        public void SetSize(int size) => this.size = size;
        public int GetSize() => size;

        //绘制函数
        public void Draw(Texture2D tex, Vector2 uv)
        {
            int centerX = (int)(uv.x * tex.width);
            int centerY = (int)(uv.y * tex.height);
            int radius = size / 2;
            //可以对笔刷进行扩展
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    int drawX = centerX + x;
                    int drawY = centerY + y;
                    // 圆形笔刷:只绘制圆内的像素
                    if (x * x + y * y <= radius * radius
                        && drawX >= 0 && drawX < tex.width
                        && drawY >= 0 && drawY < tex.height)
                    {
                        tex.SetPixel(drawX, drawY, color);
                    }
                }
            }
        }

        public void Draw_Pixel(Texture2D tex, Vector2Int pos)
        {
            int radius = size / 2;
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    int drawX = pos.x + x;
                    int drawY = pos.y + y;
                    // 圆形笔刷:只绘制圆内的像素
                    if (x * x + y * y <= radius * radius
                        && drawX >= 0 && drawX < tex.width
                        && drawY >= 0 && drawY < tex.height)
                    {
                        tex.SetPixel(drawX, drawY, color);
                    }
                }
            }
        }

        public void DrawLine_Pixel(Texture2D tex, Vector2Int start, Vector2Int end)
        {
            List<Vector2Int> drawPos = GetLinePoints(start,end);
            foreach (var point in drawPos)
            {
                Draw_Pixel(tex, point);
            }
        }

        /// <summary>
        /// 辅助方法：计算两点之间的直线像素路径
        /// </summary>
        private List<Vector2Int> GetLinePoints(Vector2Int start, Vector2Int end)
        {
            List<Vector2Int> points = new List<Vector2Int>();
            
            int x0 = start.x, y0 = start.y;
            int x1 = end.x, y1 = end.y;
            
            int dx = Mathf.Abs(x1 - x0);
            int dy = Mathf.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                points.Add(new Vector2Int(x0, y0));
                if (x0 == x1 && y0 == y1) break;
                int e2 = 2 * err;
                if (e2 > -dy) { err -= dy; x0 += sx; }
                if (e2 < dx) { err += dx; y0 += sy; }
            }
            return points;
        }
    }
}
