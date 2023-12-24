using System;
using System.CodeDom;
using System.IO;
using System.Text;
using MsgPack.Serialization;
using Newtonsoft.Json;

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
        public static byte[] PacketSerialize<T>( T Packet ) {
            MessagePackSerializer serializer = MessagePackSerializer.Get<T>();
            using( MemoryStream stream = new MemoryStream() ) {
                serializer.Pack( stream, Packet );
                byte[] typename = Encoding.UTF8.GetBytes(typeof(T).FullName + "," + typeof(T).Assembly.FullName);
                byte[] typelength = BitConverter.GetBytes(typename.Length);
                byte[] packetdata = stream.ToArray();
                byte[] paketlength = BitConverter.GetBytes(packetdata.Length);
                return typelength.Join( typename ).Join( paketlength ).Join( packetdata );
            }
        }

        public static object PacketDeserialize( string typeData, byte[] Data ) {
            Type type = Type.GetType( typeData );
            MessagePackSerializer Serilizer = MessagePackSerializer.Get(type);

            using( MemoryStream ms = new MemoryStream( Data ) ) {
                return (object)Serilizer.Unpack( ms );
            }
        }

        static byte[] TBufferedData = new byte[0];
        public static dynamic tReceive( byte [] BytesRead ) {
            TBufferedData = TBufferedData.Join( BytesRead );
            if( TBufferedData.Length > 4 ) {
                int typeLength = BitConverter.ToInt32( TBufferedData.Sub(0, 4) );
                if( TBufferedData.Length > (8 + typeLength) ) {
                    int dataLength = BitConverter.ToInt32( TBufferedData.Sub( typeLength + 4, 4 ) );
                    int TotalLength = 8 + typeLength + dataLength;
                    if ( TBufferedData.Length >= TotalLength ) {
                        string typeData = Encoding.UTF8.GetString( TBufferedData.Sub(4, typeLength) );
                        if ( typeData.Substring(0, 13) != "System.Object" ) {
                            byte[] dataBytes = TBufferedData.Sub( (typeLength + 8), dataLength );
                            dynamic data = mSerialize.PacketDeserialize( typeData, dataBytes );
                            TBufferedData = TBufferedData.Sub( TotalLength, TBufferedData.Length - TotalLength );
                            Console.WriteLine( "Received : " + JsonConvert.SerializeObject( data, Formatting.Indented ) );
                            return data;
                        } else {
                            TBufferedData = TBufferedData.Sub( TotalLength, TBufferedData.Length - TotalLength );
                        }
                    }
                }
            }
            return null;
        }

        static byte[] UBufferedData = new byte[0];
        public static dynamic uReceive( byte [] BytesRead ) {
            UBufferedData = UBufferedData.Join( BytesRead );
            if( UBufferedData.Length > 4 ) {
                int typeLength = BitConverter.ToInt32( UBufferedData.Sub(0, 4) );
                if( UBufferedData.Length > (8 + typeLength) ) {
                    int dataLength = BitConverter.ToInt32( UBufferedData.Sub( typeLength + 4, 4 ) );
                    int TotalLength = 8 + typeLength + dataLength;
                    if( UBufferedData.Length >= TotalLength ) {
                        string typeData = Encoding.UTF8.GetString( UBufferedData.Sub(4, typeLength) );
                        byte[] dataBytes = UBufferedData.Sub( (typeLength + 8), dataLength );
                        dynamic data = mSerialize.PacketDeserialize( typeData, dataBytes );
                        UBufferedData = UBufferedData.Sub( TotalLength, UBufferedData.Length - TotalLength );
                        Console.WriteLine( "Received : " + JsonConvert.SerializeObject( data, Formatting.Indented ) );
                        return data;
                    }
                }
            }
            return null;
        }
    }
}
