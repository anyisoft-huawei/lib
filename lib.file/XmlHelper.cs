using System;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace lib.file
{

    /// <summary>
    /// xml帮助类
    /// </summary>
    public static class XmlHelper
    {

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="_file">文件路径</param>
        /// <returns></returns>
        public static T CreateFromXml<T>(string _file)
        {
            var xml = XmlReader.Create(_file);
            try
            {
                return (T)new XmlSerializer(typeof(T)).Deserialize(xml);
            }
            finally
            {
                xml.Close();
            }
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="_t">泛型</param>
        /// <param name="_file">文件路径</param>
        public static void SaveToXml<T>(this T _t, string _file)
        {
            var xml = XmlWriter.Create(_file);
            try
            {
                new XmlSerializer(_t.GetType()).Serialize(xml, _t);
            }
            finally
            {
                xml.Close();
            }
        }

    }
}
