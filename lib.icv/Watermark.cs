using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib.icv
{
    public static class Watermark
    {

        /// <summary>
        /// 扩展好玩
        /// </summary>
        /// <param name="_map"></param>
        /// <param name="_old">旧颜色</param>
        /// <param name="_new">新颜色</param>
        /// <returns></returns>
        public static ColorMap Set(this ColorMap _map, Color _old, Color _new) 
        {
            _map.OldColor = _old;
            _map.NewColor = _new;
            return _map; 
        }

        /// <summary>
        /// 添加图片水印
        /// </summary>
        /// <param name="_img"></param>
        /// <param name="_watermark">水印图片</param>
        /// <param name="x">绘图位置</param>
        /// <param name="y">绘图位置</param>
        /// <param name="_transparency">透明度</param>
        /// <returns></returns>
        public static Image AddWatermark(this Image _img, Image _watermark, int x, int y, float _transparency = 0.5F, Matrix matrix = null)
        {
            //绘图参数
            using (Graphics g = Graphics.FromImage(_img))
            using (var ia = new ImageAttributes())
            {
                //定义颜色映射
                var maps = new ColorMap[] {
                    new ColorMap().Set(Color.FromArgb(255, 0, 255, 0), Color.FromArgb(0, 0, 0, 0))
                };
                ia.SetRemapTable(maps, ColorAdjustType.Bitmap);
                //定义颜色矩阵
                ColorMatrix cmatrix = new ColorMatrix(
                    new float[][] {
                        new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                        new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                        new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                        new float[] {0.0f,  0.0f,  0.0f, _transparency, 0.0f},
                        new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                    });//是否改为4*4?
                ia.SetColorMatrix(cmatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                //定义变换矩阵
                if (null != matrix) g.Transform = matrix;
                //绘图
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(_watermark, new Rectangle(x, y, _watermark.Width, _watermark.Height), 0, 0, _watermark.Width, _watermark.Height, GraphicsUnit.Pixel, ia);
                return _img;
            }
        }

        public static Image AddWatermark(this Image _img, string text, int x, int y, Font font, Brush brush, StringFormat format, Matrix matrix = null)
        {
            //绘图参数
            using (Graphics g = Graphics.FromImage(_img))
            {
                //定义变换矩阵
                if (null != matrix) g.Transform = matrix;
                //绘图
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawString(text, font, brush, x, y, format);
                return _img;
            }
        }

    }
}
