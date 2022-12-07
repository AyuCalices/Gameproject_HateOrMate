namespace Features.Experimental
{
    public interface ISerializable
    {
        public abstract object Deserialize(byte[] data);
        public abstract byte[] Serialize(object customType);
    }
}
