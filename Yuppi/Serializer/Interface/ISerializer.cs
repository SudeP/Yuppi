namespace Yuppi.Serializer.Interface
{
    public interface ISocketSerializer<T> where T : struct
    {
        public byte[] Serialize(T t);
        public T Deserialize(byte[] byteArray);
    }
}