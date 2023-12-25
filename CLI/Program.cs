using System;
using MistoxServer;

namespace MistoxHolePunch {
    class Program {

        static IMistoxServer serverObj;
        static bool running = true;

        void RunServer( int Port ) {
            Console.Clear();
            serverObj = mServer.newServer( Convert.ToInt32( Port ) );
            while ( running ) {
                string x = Console.ReadLine();
                string result = x.Substring(0, 4);
                if( result == "slow" ) {
                    serverObj.Send( x.Substring(4), SendType.SlowUpdate );
                } else {
                    serverObj.Send( x, SendType.FastUpdate );
                }
            }
        }

        void RunClient( string Host, int Port ) {
            Console.Clear();
            serverObj = mServer.newClient( Host, Convert.ToInt32( Port ) );
            while( running ) {
                string x = Console.ReadLine();
                string result = x.Substring(0, 4);
                if( result == "slow" ) {
                    serverObj.Send( x.Substring( 4 ), SendType.SlowUpdate );
                } else {
                    serverObj.Send( x, SendType.FastUpdate );
                }
            }
        }

        static void Main(string[] args) {
            string Task = args.Length > 0 ? args[0].ToLower() : null;
            if (Task == "/?" || Task == "--help") {
                Console.WriteLine(HelpDocumentation.HelpText);
            } else if (Task == "/s" || Task == "-s") {
                Program prog = new Program();
                prog.RunServer( Convert.ToInt32( args [1] ) );
            } else if (Task == "/c" || Task == "-c") {
                Program prog = new Program();
                prog.RunClient( args [1], Convert.ToInt32( args [2] ) );
            } else {
                Program prog = new Program();
                prog.RunServer(6500);
            }
        }

    }

}