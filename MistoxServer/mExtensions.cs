using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MistoxServer {

    static class Extensions {
        public static T[] Join<T>( this T[] first, T[] second ) {
            T[] bytes = new T[first.Length + second.Length];
            Buffer.BlockCopy( first, 0, bytes, 0, first.Length );
            Buffer.BlockCopy( second, 0, bytes, first.Length, second.Length );
            return bytes;
        }

        public static T[] Sub<T>( this T[] data, int index, int length ) {
            T[] result = new T[length];
            Array.Copy( data, index, result, 0, length );
            return result;
        }

    }

    public class mSerialize {

        public static byte [] PacketSerialize<T>( T Packet ) {
            byte[] packetdata = Serialize(Packet);
            byte[] length = BitConverter.GetBytes(packetdata.Length);
            return length.Join( packetdata );
        }

        public static dynamic PacketDeserialize( byte[] Data ) {
            int packetLength = BitConverter.ToInt32( Data.Sub( 0, 4) );
            dynamic data = Deserialize( Data.Sub( 4, packetLength ));
            return data;
        }

        static byte[] Serialize<T>( T obj ) {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream() ) {
                bf.Serialize( ms, obj );
                return ms.ToArray();
            }
        }

        static dynamic Deserialize( byte [] obj ) {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream() ) {
                ms.Write( obj );
                return bf.Deserialize( ms );
            }
        }

    }
}