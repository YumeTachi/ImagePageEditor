using System.Xml;

namespace PageEditor
{
    static class DataControl
    {
        /// <summary>
        /// XML形式のファイルからデータを読み出す
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Type LoadXML<Type>(string fileName)
        {
            if (System.IO.File.Exists(fileName) == false)
                return default(Type);

            byte[] bytes = System.IO.File.ReadAllBytes(fileName);

            return ConvertFromXMLBytes<Type>(bytes);
        }

        /// <summary>
        /// バイト列（XML文字列型）からデータに変換する。
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        private static Type ConvertFromXMLBytes<Type>(byte[] data)
        {
            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(data, false))
            using (System.IO.StreamReader streamReader = new System.IO.StreamReader(memoryStream, new System.Text.UTF8Encoding(false)))
            {
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = true;
                doc.Load(streamReader);

                using (XmlNodeReader nodeReader = new XmlNodeReader(doc.DocumentElement))
                {
                    //XmlSerializerオブジェクトを作成
                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(Type));

                    //XMLファイルから読み込み、逆シリアル化する
                    Type obj = (Type)serializer.Deserialize(nodeReader);

                    return obj;
                }
            }
        }

        /// <summary>
        /// データをXML形式でファイルに保存する
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        public static void SaveXML<Type>(string fileName, Type data)
        {
            if (data == null)
                return;

            byte[] bytes = ConvertXMLBytes<Type>(data);

            System.IO.File.WriteAllBytes(fileName, bytes);
        }

        /// <summary>
        /// データをバイト列（XML文字列型）に変換する。
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        private static byte[] ConvertXMLBytes<Type>(Type data)
        {
            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
            using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(memoryStream, new System.Text.UTF8Encoding(false)))
            {
                //XmlSerializerオブジェクトを作成
                //オブジェクトの型を指定する
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(data.GetType());

                //シリアル化し、XMLファイルに保存する
                serializer.Serialize(streamWriter, data);

                byte[] retBytes = memoryStream.ToArray();

                //ファイルを閉じる
                streamWriter.Close();
                memoryStream.Close();

                return retBytes;
            }
        }
    }
}
