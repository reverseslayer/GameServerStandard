using MistoxServer;
using System;

namespace MistoxHolePunch {
    class HelpDocumentation {

public static string HelpText = @"
-------------- Help Page for Mistox Game Server --------------

    Usage: MistServer ServerIP [Command] [Options]

        Command     Linux Style Command                         Meaning
        /c          -c                                          Start The Client
        /s          -s                                          Start The Server

        ServerOptions
            /s -s [options]
                /p          -p                                  The port that will be used for the server [Includes {port} + 1]
                /a          -a                                  Uses the Athoratative model for the server

        ClientOptions
            /c -c [options]
                /h          -h                                  The Hostname or IP of the server
                /p          -p                                  The port of the server

    Examples:
        MistServer.exe /c 127.0.0.1 /p 25550 /u Mistox         Start the client connecting to server 127.0.0.1 using port 25550
        MistServer.exe /c 127.0.0.1                            Start the client connecting to server 127.0.0.1 using port 25567
        MistServer.exe /s                                      Start the server
"
        ;
    }
}