using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Checksum;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib.zip
{
    /// <summary>
    /// 压缩帮助库
    /// </summary>
    public static class ZipHelper
    {
        private const int _buffer = 8912;

        #region Zip

        /// <summary>
        /// 文件处理进度委托
        /// </summary>
        /// <param name="file">文件名</param>
        /// <param name="len">字节数</param>
        /// <param name="progressed">已处理的字节数</param>
        public delegate void FileCompressProgress(string file, long len, long progressed);

        /// <summary>
        /// 向压缩文件流中追加文件
        /// </summary>
        /// <param name="zip">zip流</param>
        /// <param name="file">文件名</param>
        /// <param name="path_len">根目录长度</param>
        /// <param name="fcp">进度事件</param>
        public static void AddFile(this ZipOutputStream zip, string file, int path_len, FileCompressProgress fcp = null)
        {
            if (File.Exists(file))
            {
                var entry = new ZipEntry(file.Substring(path_len));
                entry.DateTime = new FileInfo(file).LastWriteTimeUtc; ;//设置最后修改时间
                using (var fs = File.OpenRead(file))
                {
                    entry.Size = fs.Length;//设置文件大小
                    zip.PutNextEntry(entry);//开始处理
                    var crc = new Crc32();//创建CRC校验
                    crc.Reset();
                    var buffer = new byte[_buffer];//缓存
                    int i;
                    while ((i = fs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        if (i < _buffer)//处理剩余字节
                        {
                            var b = new byte[i];
                            Buffer.BlockCopy(buffer, 0, b, 0, i);
                            buffer = b;
                        }
                        crc.Update(buffer);//更新校验和
                        zip.Write(buffer, 0, buffer.Length);//写入流
                        fcp?.BeginInvoke(file, fs.Length, fs.Position, null, null);//通知进度
                    }
                    entry.Crc = crc.Value;//设置校验和
                    zip.CloseEntry();//结束当前文件
                }
            }
        }

        /// <summary>
        /// 向压缩文件流中追加目录
        /// </summary>
        /// <param name="zip">zip流</param>
        /// <param name="dir">目录名</param>
        /// <param name="path_len">根目录长度</param>
        /// <param name="fcp">进度事件</param>
        public static void AddDirectory(this ZipOutputStream zip, string dir, int path_len, FileCompressProgress fcp = null)
        {
            if (Directory.Exists(dir))
            {
                zip.PutNextEntry(new ZipEntry(dir.Substring(path_len) + (Path.DirectorySeparatorChar == dir.Last() ? Char.MinValue : Path.DirectorySeparatorChar)));
                zip.CloseEntry();
                foreach (var item in Directory.GetFiles(dir)) zip.AddFile(item, path_len, fcp);//添加文件
                foreach (var item in Directory.GetDirectories(dir)) zip.AddDirectory(item, path_len, fcp);//添加子目录
            }
        }

        /// <summary>
        /// 对文件和目录进行压缩
        /// </summary>
        /// <param name="names">待压缩文件集合及目录集合</param>
        /// <param name="outname">zip文件路径</param>
        /// <param name="level">压缩级别</param>
        /// <param name="fcp">压缩进度事件</param>
        public static void ZipCompress(List<string> names, string outname, int level = Deflater.DEFLATED, FileCompressProgress fcp = null)
        {
            outname = outname + ".zip";//拼接输出文件名
            using (var zip = new ZipOutputStream(File.Create(outname)))//创建输出流
            {
                zip.SetLevel(level);//设置压缩级别
                foreach (var item in names)//遍历
                {
                    var path = Path.GetDirectoryName(item);//获取文件/目录所在目录
                    var path_len = null == path ? 2 : path.Length;//设置添加路径起始字符，如果为根目录则为2
                    if (File.Exists(item))
                    {
                        zip.AddFile(item, path_len, fcp);//添加文件
                    }
                    else if(Directory.Exists(item))
                    {
                        zip.AddDirectory(item, path_len, fcp);//添加目录
                    }
                }
                zip.Flush();
            }
        }

        /// <summary>
        /// 对文件或目录进行压缩
        /// </summary>
        /// <param name="names">待压缩文件或目录</param>
        /// <param name="outname">zip文件路径</param>
        /// <param name="level">压缩级别</param>
        /// <param name="fcp">压缩进度事件</param>
        public static void ZipCompress(string name, string outname, int level = Deflater.DEFLATED, FileCompressProgress fcp = null)
        {
            using (var outfs = File.Create(outname))
            using (var zip = new ZipOutputStream(outfs))//创建输出流
            {
                zip.SetLevel(level);//设置压缩级别
                var path = Path.GetDirectoryName(name);//获取文件/目录所在目录
                var path_len = null == path ? 2 : path.Length;//设置添加路径起始字符，如果为根目录则为2
                if (File.Exists(name))
                {
                    zip.AddFile(name, path_len, fcp);//添加文件
                }
                else if (Directory.Exists(name))
                {
                    zip.AddDirectory(name, path_len, fcp);//添加目录
                }
                zip.Flush();
            }
        }

        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="file">zip文件名</param>
        /// <param name="path">要解压到的位置</param>
        /// <param name="fcp">进度事件</param>
        public static void ZipDeCompress(string file, string path, FileCompressProgress fcp = null)
        {
            if (File.Exists(file))
            {
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                using (var open = File.OpenRead(file))
                using (var zip = new ZipInputStream(open))
                {
                    ZipEntry entry;
                    while (null != (entry = zip.GetNextEntry()))
                    {
                        if(string.Empty != entry.Name)
                        {
                            if (entry.IsDirectory) Directory.CreateDirectory(Path.Combine(path, entry.Name));//目录
                            else
                            {
                                var name = Path.Combine(path, entry.Name);
                                using (var fs = File.OpenWrite(name))//文件
                                {
                                    var buffer = new byte[_buffer];
                                    int i;
                                    while ((i = zip.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        fs.Write(buffer, 0, i);
                                        fcp?.BeginInvoke(name, entry.Size, fs.Position, null, null);
                                    }
                                }//end open
                            }
                        }
                    }
                }//end zip
            }
            else throw new Exception("文件不存在！");
        }

        /// <summary>
        /// 获取zip文件列表
        /// </summary>
        /// <param name="file">zip文件</param>
        /// <param name="encod">编码</param>
        /// <returns></returns>
        public static List<string> GetZipFiles(string file)
        {
            var list = new List<string>();
            if (File.Exists(file))
            {
                using (var fs = File.OpenRead(file))
                using (var zip = new ZipInputStream(fs))
                {
                    ZipEntry entry;
                    while (null != (entry = zip.GetNextEntry()))
                    {
                        if (string.Empty != entry.Name)
                        {
                            list.Add(entry.Name);
                        }
                    }
                }
            }
            return list;
        }


        /// <summary>
        /// 压缩二进制数据
        /// </summary>
        /// <param name="bs">二进制数据</param>
        /// <param name="level">压缩级别</param>
        /// <returns></returns>
        public static byte[] ZipCompress(byte[] bs, int level = Deflater.BEST_COMPRESSION)
        {
            using (var ms = new MemoryStream())
            {
                using (var dos = new DeflaterOutputStream(ms, new Deflater(level), _buffer))
                {
                    dos.Write(bs, 0, bs.Length);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 解压二进制数据
        /// </summary>
        /// <param name="bs">压缩的数据</param>
        /// <returns></returns>
        public static byte[] ZipDeCompress(byte[] bs)
        {
            using (var ins = new MemoryStream(bs))
            using (var iis = new InflaterInputStream(ins))
            {
                var buffer = new byte[_buffer];
                using (var ms = new MemoryStream())
                {
                    int i;
                    while ((i = iis.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, i);
                    }
                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// 压缩字符串
        /// </summary>
        /// <param name="t">字符串</param>
        /// <returns></returns>
        public static string ZipCompress(string t)
        {
            return string.IsNullOrEmpty(t) ? "" : Convert.ToBase64String(ZipCompress(Encoding.UTF8.GetBytes(t)));
        }

        /// <summary>
        /// 解压字符串
        /// </summary>
        /// <param name="b64">压缩的Base64字符串</param>
        /// <returns></returns>
        public static string ZipDeCompress(string b64)
        {
            return string.IsNullOrEmpty(b64) ? "" : Encoding.UTF8.GetString(ZipDeCompress(Convert.FromBase64String(b64)));
        }

        #endregion

        #region GZip

        /// <summary>
        /// 压缩二进制数据
        /// </summary>
        /// <param name="_from">原始数据输入流</param>
        /// <param name="_to">压缩文件输出流</param>
        /// <returns></returns>
        public static void GZipCompress(Stream _from, Stream _to, FileCompressProgress fcp = null)
        {
            using (var gz = new GZipOutputStream(_to))
            {
                var buffer = new byte[_buffer];
                int i;
                while ((i = _from.Read(buffer, 0, buffer.Length)) > 0)
                {
                    gz.Write(buffer, 0, i);
                    fcp?.BeginInvoke("gz", _from.Length, _from.Position, null, null);
                }
                gz.Flush();
            }
        }

        /// <summary>
        /// 解压二进制数据
        /// </summary>
        /// <param name="_from">压缩数据输入流</param>
        /// <param name="_to">解压文件输出流</param>
        /// <returns></returns>
        public static void GZipDeCompress(Stream _from, Stream _to, FileCompressProgress fcp = null)
        {
            using (var gz = new GZipInputStream(_from))
            {
                var buffer = new byte[_buffer];
                int i;
                while ((i = gz.Read(buffer, 0, buffer.Length)) > 0)
                {
                    _to.Write(buffer, 0, i);
                    fcp?.BeginInvoke("gz", gz.Length, gz.Position, null, null);
                }
            }
        }

        /// <summary>
        /// 压缩字符串
        /// </summary>
        /// <param name="utf8">字符串</param>
        /// <returns></returns>
        public static byte[] GZipCompress(byte[] bs)
        {
            using (var _from = new MemoryStream(bs))
            using (var _to = new MemoryStream())
            {
                GZip.Compress(_from, _to, true, _buffer, Deflater.BEST_COMPRESSION);
                //GZipCompress(_from, _to);
                return _to.ToArray();
            }
        }

        /// <summary>
        /// 解压字符串
        /// </summary>
        /// <param name="base64">压缩的Base64字符串</param>
        /// <returns></returns>
        public static byte[] GZipDeCompress(byte[] bs)
        {
            using (var _from = new MemoryStream(bs))
            using (var _to = new MemoryStream())
            {
                GZip.Decompress(_from, _to, true);
                //GZipDeCompress(_from, _to);//解压数据
                return _to.ToArray();
            }
        }

        /// <summary>
        /// 压缩字符串
        /// </summary>
        /// <param name="utf8">字符串</param>
        /// <returns></returns>
        public static string GZipCompress(string utf8)
        {
            if (string.IsNullOrEmpty(utf8)) return "";
            return Convert.ToBase64String(GZipCompress(Encoding.UTF8.GetBytes(utf8)));
        }

        /// <summary>
        /// 解压字符串
        /// </summary>
        /// <param name="base64">压缩的Base64字符串</param>
        /// <returns></returns>
        public static string GZipDeCompress(string base64)
        {
            if (string.IsNullOrEmpty(base64)) return "";
            return Encoding.UTF8.GetString(GZipDeCompress(Convert.FromBase64String(base64)));
        }

        #endregion

        #region Tar

        /// <summary>
        /// 向包流中追加文件
        /// </summary>
        /// <param name="tar">zip流</param>
        /// <param name="file">文件名</param>
        /// <param name="path_len">根目录长度</param>
        /// <param name="fcp">进度事件</param>
        public static void AddFile(this TarOutputStream tar, string file, int path_len, FileCompressProgress fcp = null)
        {
            if (File.Exists(file))
            {
                var head = new TarHeader();
                head.Name = file.Substring(path_len);//设置文件名
                head.ModTime = new FileInfo(file).LastWriteTimeUtc;//设置添加时间
                using (var fs = File.OpenRead(file))
                {
                    head.Size = fs.Length;//设置文件大小
                    tar.PutNextEntry(new TarEntry(head));//开始处理
                    var buffer = new byte[_buffer];//缓存
                    int i;
                    while ((i = fs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        if (i < _buffer)//处理剩余字节
                        {
                            var b = new byte[i];
                            Buffer.BlockCopy(buffer, 0, b, 0, i);
                            buffer = b;
                        }
                        tar.Write(buffer, 0, buffer.Length);//写入流
                        fcp?.BeginInvoke(file, fs.Length, fs.Position, null, null);//通知进度
                    }
                    tar.CloseEntry();//结束当前文件
                }
            }
        }

        /// <summary>
        /// 向包流中追加目录
        /// </summary>
        /// <param name="tar">zip流</param>
        /// <param name="dir">目录名</param>
        /// <param name="path_len">根目录长度</param>
        /// <param name="fcp">进度事件</param>
        public static void AddDirectory(this TarOutputStream tar, string dir, int path_len, FileCompressProgress fcp = null)
        {
            if (Directory.Exists(dir))
            {
                var head = new TarHeader();
                head.Name = dir.Substring(path_len) + (Path.DirectorySeparatorChar == dir.Last() ? Char.MinValue : Path.DirectorySeparatorChar);
                head.ModTime = new DirectoryInfo(dir).LastWriteTimeUtc;//设置添加时间
                tar.PutNextEntry(new TarEntry(head));
                tar.CloseEntry();
                foreach (var item in Directory.GetFiles(dir)) tar.AddFile(item, path_len, fcp);//添加文件
                foreach (var item in Directory.GetDirectories(dir)) tar.AddDirectory(item, path_len, fcp);//添加子目录
            }
        }

        /// <summary>
        /// 打包Tar文件
        /// </summary>
        /// <param name="names">要打包的目录或文件</param>
        /// <param name="outname">输出文件名</param>
        /// <param name="encod"></param>
        /// <param name="fcp"></param>
        public static void TarCompress(List<string> names, string outname, Encoding encod, FileCompressProgress fcp = null) 
        {
            outname = outname + ".tar";//拼接输出文件名
            using (var zip = new TarOutputStream(File.Create(outname), encod))//创建输出流
            {
                foreach (var item in names)//遍历
                {
                    var path = Path.GetDirectoryName(item);//获取文件/目录所在目录
                    var path_len = null == path ? 2 : path.Length;//设置添加路径起始字符，如果为根目录则为2
                    if (File.Exists(item))
                    {
                        zip.AddFile(item, path_len, fcp);//添加文件
                    }
                    else if (Directory.Exists(item))
                    {
                        zip.AddDirectory(item, path_len, fcp);//添加目录
                    }
                }
                zip.Flush();
            }
        }

        /// <summary>
        /// 解包Tar文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <param name="fcp"></param>
        public static void TarDeCompress(string file, string path, Encoding encod, FileCompressProgress fcp = null)
        {
            if (File.Exists(file))
            {
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                using (var tar = new TarInputStream(File.OpenRead(file), encod))
                {
                    TarEntry entry;
                    while (null != (entry = tar.GetNextEntry()))
                    {
                        if (string.Empty != entry.Name)
                        {
                            if (entry.IsDirectory) Directory.CreateDirectory(Path.Combine(path, entry.Name));//目录
                            else
                            {
                                var name = Path.Combine(path, entry.Name);
                                using (var fs = File.OpenWrite(name))//文件
                                {
                                    var buffer = new byte[_buffer];
                                    int i;
                                    while ((i = tar.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        fs.Write(buffer, 0, i);
                                        fcp?.BeginInvoke(name, entry.Size, fs.Position, null, null);
                                    }
                                }//end open
                                new FileInfo(name).LastWriteTimeUtc = entry.ModTime;
                            }
                        }
                    }
                }//end zip
            }
            else throw new Exception("文件不存在！");
        }

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="outname">生成文件名</param>
        /// <param name="path">压缩路径</param>
        /// <param name="encod">编码</param>
        /// <param name="msg">进度</param>
        public static void TarGZipCompress(string outname, string path, Encoding encod, ProgressMessageHandler msg = null)
        {
            outname = outname + ".tar.gz";//拼接输出文件名
            using (var fs = File.OpenWrite(outname))
            using (var gz = new GZipOutputStream(fs))
            using (var tar = TarArchive.CreateOutputTarArchive(gz, encod))
            {
                tar.RootPath = path;
                tar.ProgressMessageEvent += msg;
                tar.ListContents();
            }
        }

        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="file">.tar.gz文件</param>
        /// <param name="path">要解压到的位置</param>
        /// <param name="encod">编码</param>
        /// <param name="msg">进度</param>
        public static void TarGZipDeCompress(string file, string path, Encoding encod, ProgressMessageHandler msg = null)
        {
            using (var fs = File.OpenRead(file))
            using (var gz = new GZipInputStream(fs))
            using (var tar = TarArchive.CreateInputTarArchive(gz, encod))
            {
                tar.ProgressMessageEvent += msg;
                tar.ExtractContents(path);
            }
        }

        /// <summary>
        /// 获取.tar.gz文件列表
        /// </summary>
        /// <param name="file">.tar.gz文件</param>
        /// <param name="encod">编码</param>
        /// <returns></returns>
        public static List<string> GetTarGZipFiles(string file, Encoding encod)
        {
            var list = new List<string>();
            if (File.Exists(file))
            {
                using (var fs = File.OpenRead(file))
                using (var gz = new GZipInputStream(fs))
                using (var tar = new TarInputStream(gz, encod))
                {
                    TarEntry entry;
                    while (null != (entry = tar.GetNextEntry()))
                    {
                        if (string.Empty != entry.Name)
                        {
                            list.Add(entry.Name);
                        }
                    }
                }
            }
            return list;
        }

        #endregion

        #region Bzip
        /// <summary>
        /// 压缩bizp字符串
        /// </summary>
        /// <param name="utf8"></param>
        /// <returns></returns>
        public static byte[] BZipCompress(byte[] bs)
        {
            using (var _from = new MemoryStream(bs))
            using (var _to = new MemoryStream())
            {
                BZip2.Compress(_from, _to, true, Deflater.BEST_COMPRESSION);
                return _to.ToArray();
            }
        }
        /// <summary>
        /// 解压bzip字符串
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static byte[] BZipDeCompress(byte[] bs)
        {
            using (var _from = new MemoryStream(bs))
            using (var _to = new MemoryStream())
            {
                BZip2.Decompress(_from, _to, true);
                return _to.ToArray();
            }
        }
        /// <summary>
        /// 压缩bizp字符串
        /// </summary>
        /// <param name="utf8"></param>
        /// <returns></returns>
        public static string BZipCompress(string utf8)
        {
                return Convert.ToBase64String(BZipCompress(Encoding.UTF8.GetBytes(utf8)));
        }
        /// <summary>
        /// 解压bzip字符串
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static string BZipDeCompress(string base64)
        {
            return Convert.ToBase64String(BZipDeCompress(Convert.FromBase64String(base64)));
        }
        #endregion

    }
}
