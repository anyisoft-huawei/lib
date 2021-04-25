using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace lib.icv
{

	/// <summary>
	/// Image帮助类
	/// </summary>
	public static class ImageHelper
	{

        /// <summary>
		/// 用指定长宽积获取新尺寸
		/// </summary>
		/// <param name="w">宽度</param>
		/// <param name="h">高度</param>
		/// <param name="product">最大长宽积</param>
		/// <returns></returns>
		public static Size GetSizeForProduct(int w, int h, int product)
        {
            if(w == h)
            {
                var wh = (int)Math.Sqrt(product);
                return new Size(wh, wh);
            }
            var r = (double)w / h;//获得宽高比
            var nw = Math.Sqrt(product * r); //获得宽度
            var nh = nw / r; //获得高度
            return new Size((int)nw, (int)nh);
        }

        /// <summary>
        /// 获取字节流
        /// </summary>
        /// <param name="img"></param>
        /// <param name="ici">编码信息</param>
        /// <param name="quality">图像质量</param>
        /// <returns></returns>
        public static byte[] ToBytes(this Image img, ImageCodecInfo ici, long quality = 100L)
        {
            var ms = new MemoryStream();
            var _eps = new EncoderParameters(1);
            try
            {
                _eps.Param[0] = new EncoderParameter(Encoder.Quality, quality);//创建质量参数
                img.Save(ms, ici, _eps);
                var bs = ms.ToArray();
                _eps.Dispose();
                return bs;
            }
            finally
            {
                ms.Close();
            }
        }

        /// <summary>
        /// 获取字节流
        /// </summary>
        /// <param name="img"></param>
        /// <param name="format">图像格式</param>
        /// <returns></returns>
        public static byte[] ToBytes(this Image img, ImageFormat format)
        {
            var ms = new MemoryStream();
            try
            {
                img.Save(ms, format);
                var bs = ms.ToArray();
                return bs;
            }
            finally
            {
                ms.Close();
            }
        }
        
        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="img"></param>
        /// <param name="_file">文件路径</param>
        /// <param name="ici">编码信息</param>
        /// <param name="quality">图像质量</param>
        public static void ToFile(this Image img, string _file, ImageCodecInfo ici, long quality = 100L)
		{
			EncoderParameters parameters = new EncoderParameters(1);
			parameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);//创建质量参数
            img.Save(_file, ici, parameters);
			parameters.Dispose();
		}

        /// <summary>
        /// 获取图像编码信息
        /// </summary>
        /// <param name="mimeType">包含编码解码器的多用途网际邮件扩充协议 (MIME) 类型的字符串</param>
        /// <returns>编码信息</returns>
        public static ImageCodecInfo GetImageCodecInfo(string mimeType)
		{
			foreach(ImageCodecInfo ici in ImageCodecInfo.GetImageEncoders())
			{
				if(ici.MimeType == mimeType) return ici;
			}
			return null;
		}

        /// <summary>
        /// 获取图像编码解码器的所有相关信息
        /// </summary>
        /// <param name="mimeType">图像格式</param>
        /// <returns>返回图像编码解码器的所有相关信息</returns>
        public static ImageCodecInfo GetImageCodecInfo(ImageFormat format)
        {
            foreach (ImageCodecInfo i in ImageCodecInfo.GetImageEncoders())
            {
                if (i.FormatID == format.Guid) return i;
            }
            return null;
        }

        /// <summary>
        /// 得到图片格式
        /// </summary>
        /// <param name="name">文件名称</param>
        /// <returns></returns>
        public static ImageFormat GetFormat(string name)
        {
            int p = name.LastIndexOf(".");
            string ext = p < 0 ? name : name.Substring(p + 1);
            switch (ext.ToLower())
            {
                case "bmp":
                    return ImageFormat.Bmp;
                case "png":
                    return ImageFormat.Png;
                case "gif":
                    return ImageFormat.Gif;
                default:
                    return ImageFormat.Jpeg;
            }
        }

        /// <summary>
        /// 制作缩略图
        /// </summary>
        /// <param name="_img"></param>
        /// <param name="product">长宽积</param>
        /// <returns></returns>
        public static Image ToToThumbnailImage(this Image _img, int product)
        {
            var kg = GetSizeForProduct(_img.Width, _img.Height, product);
            return new Bitmap(_img, kg.Width, kg.Height);
        }

        /// <summary>
        /// 制作小正方形
		/// </summary>
		/// <param name="_img">图片</param>
		/// <param name="product">最大积</param>
        /// <returns></returns>
        public static Image ToSquareImage(this Image _img, int product)
        {
            return _img.ToThumbnailImageCut(product, _img.Width > _img.Height ?
                new Rectangle((_img.Width - _img.Height) / 2, 0, _img.Height, _img.Height) :
                new Rectangle(0, (_img.Height - _img.Width) / 2, _img.Width, _img.Width));
        }

        /// <summary>
        /// 从指定裁剪区域制作缩略图
        /// </summary>
        /// <param name="_img">图形</param>
        /// <param name="product">最大积</param>
        /// <param name="cut">裁剪区域</param>
        /// <returns></returns>
        public static Image ToThumbnailImageCut(this Image _img, int product, Rectangle cut)
        {
            var kg = GetSizeForProduct(cut.Width, cut.Height, product);
            Bitmap bmp = new Bitmap(kg.Width, kg.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;//设置高质量插值法               
                g.SmoothingMode = SmoothingMode.AntiAlias;//设置高质量,低速度呈现平滑程度
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;               
                g.Clear(Color.Transparent);//清空画布并以透明背景色填充
                //在指定位置并且按指定大小绘制原图片的指定部分
                g.DrawImage(_img, new Rectangle(0, 0, kg.Width, kg.Height), cut, GraphicsUnit.Pixel);
                //Image re = new Bitmap(bmp, kg.Width, kg.Height);
                return bmp;
            }
        }

        /// <summary>
        /// 从指定裁剪区域制作缩略图
        /// </summary>
        /// <param name="_file">文件名</param>
        /// <param name="product">最大积</param>
        /// <param name="cut">裁剪区域</param>
        /// <returns></returns>
        public static Image ToThumbnailImageCut(string _file, int product, Rectangle cut)
        {
            Image img = Image.FromStream(new MemoryStream(File.ReadAllBytes(_file)));
            return img.ToThumbnailImageCut(product, cut);
        }

        /// <summary>
        /// 从指定裁剪区域制作缩略图
        /// </summary>
        /// <param name="ms">流</param>
        /// <param name="product">最大积</param>
        /// <param name="cut">裁剪区域</param>
        /// <returns></returns>
        public static Image ToThumbnailImageCut(Stream ms, int product, Rectangle cut)
        {
            Image img = Image.FromStream(ms);
            return img.ToThumbnailImageCut(product, cut);
        }

        /// <summary>
        /// 获取图片流
        /// </summary>
        /// <param name="url">图片URL</param>
        /// <returns></returns>
        private static Stream GetRemoteImage(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentLength = 0;
            request.Timeout = 20000;
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                return response.GetResponseStream();
            }
            catch
            {
                return null;
            }
        }

    }
}
