using System.Numerics;
using System.Runtime.Serialization;
namespace _SaveSystem.SerializationSurrogate
{
    public class SaveVector3 : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Vector3 vector3 = (Vector3) obj;
            info.AddValue("x", vector3.X);
            info.AddValue("y", vector3.Y);
            info.AddValue("z", vector3.Z);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Vector3 _vector3 = new Vector3();
            _vector3.X = (float) info.GetValue("x", typeof(float));
            _vector3.Y = (float) info.GetValue("y", typeof(float));
            _vector3.Z = (float) info.GetValue("z", typeof(float));
            obj = _vector3;
            return obj;
        }
    }
}