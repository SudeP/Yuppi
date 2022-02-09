using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Yuppi.Serializer.Interface;

namespace Yuppi.Serializer.Inheritance
{
    public class BinarySocketSerializer<T> : ISocketSerializer<T> where T : struct
    {
        public BinarySocketSerializer()
        {
            serializer = new BinaryFormatter();
        }

        private readonly BinaryFormatter serializer;

        public byte[] Serialize(T t)
        {
            byte[] byteArray;

            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, t);
                byteArray = stream.ToArray();
            }

            return byteArray;
        }

        public T Deserialize(byte[] byteArray)
        {
            T t = default;

            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                t = (T)serializer.Deserialize(stream);
            }

            return t;
        }
    }
}
