using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Rocket.Surgery.LaunchPad.Grpc.Validation
{
    internal static class BinarySerializer
    {
        public static byte[] ToBytes<T>(this T objectToSerialize)
        {
            var formatter = new BinaryFormatter();
            using var mStream = new MemoryStream();
            formatter.Serialize(mStream, objectToSerialize);
            return mStream.ToArray();
        }
    }
}
