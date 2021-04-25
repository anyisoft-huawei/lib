using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace lib.icv
{

	/// <summary>
	/// Image������
	/// </summary>
	public static class ImageHelper
	{

        /// <summary>
		/// ��ָ���������ȡ�³ߴ�
		/// </summary>
		/// <param name="w">���</param>
		/// <param name="h">�߶�</param>
		/// <param name="product">��󳤿��</param>
		/// <returns></returns>
		public static Size GetSizeForProduct(int w, int h, int product)
        {
            if(w == h)
            {
                var wh = (int)Math.Sqrt(product);
                return new Size(wh, wh);
            }
            var r = (double)w / h;//��ÿ�߱�
            var nw = Math.Sqrt(product * r); //��ÿ��
            var nh = nw / r; //��ø߶�
            return new Size((int)nw, (int)nh);
        }

        /// <summary>
        /// ��ȡ�ֽ���
        /// </summary>
        /// <param name="img"></param>
        /// <param name="ici">������Ϣ</param>
        /// <param name="quality">ͼ������</param>
        /// <returns></returns>
        public static byte[] ToBytes(this Image img, ImageCodecInfo ici, long quality = 100L)
        {
            var ms = new MemoryStream();
            var _eps = new EncoderParameters(1);
            try
            {
                _eps.Param[0] = new EncoderParameter(Encoder.Quality, quality);//������������
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
        /// ��ȡ�ֽ���
        /// </summary>
        /// <param name="img"></param>
        /// <param name="format">ͼ���ʽ</param>
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
        /// ����ͼƬ
        /// </summary>
        /// <param name="img"></param>
        /// <param name="_file">�ļ�·��</param>
        /// <param name="ici">������Ϣ</param>
        /// <param name="quality">ͼ������</param>
        public static void ToFile(this Image img, string _file, ImageCodecInfo ici, long quality = 100L)
		{
			EncoderParameters parameters = new EncoderParameters(1);
			parameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);//������������
            img.Save(_file, ici, parameters);
			parameters.Dispose();
		}

        /// <summary>
        /// ��ȡͼ�������Ϣ
        /// </summary>
        /// <param name="mimeType">��������������Ķ���;�����ʼ�����Э�� (MIME) ���͵��ַ���</param>
        /// <returns>������Ϣ</returns>
        public static ImageCodecInfo GetImageCodecInfo(string mimeType)
		{
			foreach(ImageCodecInfo ici in ImageCodecInfo.GetImageEncoders())
			{
				if(ici.MimeType == mimeType) return ici;
			}
			return null;
		}

        /// <summary>
        /// ��ȡͼ���������������������Ϣ
        /// </summary>
        /// <param name="mimeType">ͼ���ʽ</param>
        /// <returns>����ͼ���������������������Ϣ</returns>
        public static ImageCodecInfo GetImageCodecInfo(ImageFormat format)
        {
            foreach (ImageCodecInfo i in ImageCodecInfo.GetImageEncoders())
            {
                if (i.FormatID == format.Guid) return i;
            }
            return null;
        }

        /// <summary>
        /// �õ�ͼƬ��ʽ
        /// </summary>
        /// <param name="name">�ļ�����</param>
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
        /// ��������ͼ
        /// </summary>
        /// <param name="_img"></param>
        /// <param name="product">�����</param>
        /// <returns></returns>
        public static Image ToToThumbnailImage(this Image _img, int product)
        {
            var kg = GetSizeForProduct(_img.Width, _img.Height, product);
            return new Bitmap(_img, kg.Width, kg.Height);
        }

        /// <summary>
        /// ����С������
		/// </summary>
		/// <param name="_img">ͼƬ</param>
		/// <param name="product">����</param>
        /// <returns></returns>
        public static Image ToSquareImage(this Image _img, int product)
        {
            return _img.ToThumbnailImageCut(product, _img.Width > _img.Height ?
                new Rectangle((_img.Width - _img.Height) / 2, 0, _img.Height, _img.Height) :
                new Rectangle(0, (_img.Height - _img.Width) / 2, _img.Width, _img.Width));
        }

        /// <summary>
        /// ��ָ���ü�������������ͼ
        /// </summary>
        /// <param name="_img">ͼ��</param>
        /// <param name="product">����</param>
        /// <param name="cut">�ü�����</param>
        /// <returns></returns>
        public static Image ToThumbnailImageCut(this Image _img, int product, Rectangle cut)
        {
            var kg = GetSizeForProduct(cut.Width, cut.Height, product);
            Bitmap bmp = new Bitmap(kg.Width, kg.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;//���ø�������ֵ��               
                g.SmoothingMode = SmoothingMode.AntiAlias;//���ø�����,���ٶȳ���ƽ���̶�
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;               
                g.Clear(Color.Transparent);//��ջ�������͸������ɫ���
                //��ָ��λ�ò��Ұ�ָ����С����ԭͼƬ��ָ������
                g.DrawImage(_img, new Rectangle(0, 0, kg.Width, kg.Height), cut, GraphicsUnit.Pixel);
                //Image re = new Bitmap(bmp, kg.Width, kg.Height);
                return bmp;
            }
        }

        /// <summary>
        /// ��ָ���ü�������������ͼ
        /// </summary>
        /// <param name="_file">�ļ���</param>
        /// <param name="product">����</param>
        /// <param name="cut">�ü�����</param>
        /// <returns></returns>
        public static Image ToThumbnailImageCut(string _file, int product, Rectangle cut)
        {
            Image img = Image.FromStream(new MemoryStream(File.ReadAllBytes(_file)));
            return img.ToThumbnailImageCut(product, cut);
        }

        /// <summary>
        /// ��ָ���ü�������������ͼ
        /// </summary>
        /// <param name="ms">��</param>
        /// <param name="product">����</param>
        /// <param name="cut">�ü�����</param>
        /// <returns></returns>
        public static Image ToThumbnailImageCut(Stream ms, int product, Rectangle cut)
        {
            Image img = Image.FromStream(ms);
            return img.ToThumbnailImageCut(product, cut);
        }

        /// <summary>
        /// ��ȡͼƬ��
        /// </summary>
        /// <param name="url">ͼƬURL</param>
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
