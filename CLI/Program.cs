using System;
using MistoxServer;

namespace MistoxHolePunch {
    class Program {

        static IMistoxServer serverObj;
        static bool running = true;

        void slowReceive( object obj, EventArgs e ) {
            if ( serverObj is ServerInterface ) {
                // If ServerMode is passive obj is byte[] and Send takes in byte[]
                // If Servermode is Athoritiative obj is the Class type sent and Send takes in any Class type except generic object

                // Check to make sure data is correct before relaying
                // Also perform server specific checks in here
                // IE player didnt teleport or is shooting from 20 feet from his body
                serverObj.Send( obj, SendType.SlowUpdate );
            }
            Console.Write( "Received TCP : " );
            Console.WriteLine( obj );
        }

        void fastReceive( object obj, EventArgs e ) {
            if( serverObj is ServerInterface ) {
                // If ServerMode is passive obj is byte[] and Send takes in byte[]
                // If Servermode is Athoritiative obj is the Class type sent and Send takes in any Class type except generic object

                // Check to make sure data is correct before relaying
                // Also perform server specific checks in here
                // IE player didnt teleport or is shooting from 20 feet from his body
                serverObj.Send( obj, SendType.FastUpdate );
            }
            Console.Write( "Received UDP : " );
            Console.WriteLine( obj );
        }

        static string host = "example.com";
        static int port = 7300;
        static ServerMode mode = ServerMode.Passive;

        void RunServer() {
            Console.Clear();
            serverObj = mServer.newServer( Convert.ToInt32( port ), mode );
            serverObj.onSlowReceive += slowReceive;
            serverObj.onFastReceive += fastReceive;
            while ( running ) {
                string x = Console.ReadLine();
                if( x.Length >= 4 ) {
                    string result = x.Substring(0, 4);
                    if( result == "slow" ) {
                        serverObj.Send( x.Substring( 4 ), SendType.SlowUpdate );
                    } else {
                        serverObj.Send( x, SendType.FastUpdate );
                    }
                } else {
                    serverObj.Send( x, SendType.FastUpdate );
                }
            }
        }

        void RunClient() {
            Console.Clear();
            serverObj = mServer.newClient( host, Convert.ToInt32( port ) );
            serverObj.onSlowReceive += slowReceive;
            serverObj.onFastReceive += fastReceive;
            while( running ) {
                string x = Console.ReadLine();
                if( x.Length >= 4 ) {
                    string result = x.Substring(0, 4);
                    if( result == "slow" ) {
                        serverObj.Send( x.Substring( 4 ), SendType.SlowUpdate );
                    } else {
                        serverObj.Send( x, SendType.FastUpdate );
                    }
                } else {
                    serverObj.Send( x, SendType.FastUpdate );
                }
            }
        }

        static void Main(string[] args) {
            string Task = args.Length > 0 ? args[0].ToLower() : null;

            for( int i = 0; i < args.Length; i++ ) {
                string cur = args[i].ToLower();
                if( cur == "/h" || cur == "-h" ) {
                    host = args [i + 1];
                } else if( cur == "/p" || cur == "-p" ) {
                    port = Convert.ToInt32( args [i + 1] );
                } else if( cur == "/a" || cur == "-a" ) {
                    mode = ServerMode.Authoritative;
                }
            }

            if (Task == "/?" || Task == "--help") {
                Console.WriteLine(HelpDocumentation.HelpText);
            } else if (Task == "/s" || Task == "-s") {
                Program prog = new Program();
                prog.RunServer();
            } else if (Task == "/c" || Task == "-c") {
                Program prog = new Program();
                prog.RunClient();
            } else {
                host = "mistox.net";
                port = 6500;
                Program prog = new Program();
                //prog.RunClient();
                prog.RunServer();
            }
        }

    }

}