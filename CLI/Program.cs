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
                serverObj.Send( x, SendType.SlowUpdate );
            }
        }

        void RunClient( int Port ) {
            Console.Clear();
            serverObj = mServer.newClient( "DVR", Convert.ToInt32( Port ) );
            while( running ) {
                string x = Console.ReadLine();
                serverObj.Send( x, SendType.SlowUpdate );
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
                prog.RunClient( Convert.ToInt32( args [1] ) );
            } else {
                Program prog = new Program();
                prog.RunClient(6500);
            }
        }

    }

}