using System.Numerics;
using System.Runtime.Serialization;
namespace _SaveSystem.SerializationSurrogate
{
    public class SaveVector3 : ISerializationSurrogate
    {
        public void GetObjectData(object _obj, SerializationInfo _info, StreamingContext _context)
        {
            Vector3 _vector3 = (Vector3) _obj;
            _info.AddValue("x", _vector3.X);
            _info.AddValue("y", _vector3.Y);
            _info.AddValue("z", _vector3.Z);
        }

        public object SetObjectData(object _obj, SerializationInfo _info, StreamingContext _context, ISurrogateSelector _selector)
        {
            Vector3 _vector3 = new Vector3();
            _vector3.X = (float) _info.GetValue("x", typeof(float));
            _vector3.Y = (float) _info.GetValue("y", typeof(float));
            _vector3.Z = (float) _info.GetValue("z", typeof(float));
            _obj = _vector3;
            return _obj;
        }
    }
}