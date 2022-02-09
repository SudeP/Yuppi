using System.Text;
using System.Web.Script.Serialization;
using Yuppi.Serializer.Interface;

namespace Yuppi.Serializer.Inheritance
{
    public class JsonSocketSerializer<T> : ISocketSerializer<T> where T : struct
    {
        public JsonSocketSerializer(Encoding parserEncoding) : this(parserEncoding, null)
        {
        }

        public JsonSocketSerializer(Encoding parserEncoding, JavaScriptTypeResolver javaScriptTypeResolver)
        {
            encoding = parserEncoding;
            serializer = new JavaScriptSerializer(javaScriptTypeResolver);
        }

        private readonly Encoding encoding;
        private readonly JavaScriptSerializer serializer;

        public Encoding Encoding => encoding;
        public JavaScriptSerializer Serializer => serializer;

        public byte[] Serialize(T t)
        {
            string jsonString = serializer.Serialize(t);

            byte[] byteArray = encoding.GetBytes(jsonString);

            return byteArray;
        }

        public T Deserialize(byte[] byteArray)
        {
            string jsonString = encoding.GetString(byteArray);

            T t = serializer.Deserialize<T>(jsonString);

            return t;
        }
    }
}
