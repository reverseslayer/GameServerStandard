# Server Standard

This is a central server and client model based server. The basis of the networking is on msgpack. works on IPv4 only at the moment. In a future release I might add IPV6 but not on any roadmap. Fully threaded and asynchrous to scale easily.

## Library

This is a library with a CLI attached to it to show usage. You can send and receive any data type except the standard 'Object' type

## Usage

Follow the example listed in Program.cs inside the CLI
Note: All datatypes that are sent to the server, The server needs to be aware of when in Athouritative mode.

Create the server
```c#
IMistoxServer serverobj = mServer.newServer( int Port, ServerMode mode )
```

Create the client
```c#
IMistoxServer serverobj = mServer.newClient( string HostNameOrIP, int Port )
```

Sending data
```c#
serverobj.Send( dynamic Object, SendType Speed )
```

Receiving data
```c#

serverobj.onSlowReceive += SlowReceivedData;
serverobj.onFastReceive += FastReceivedData;
void SlowReceivedData( object Data, EventArgs e ){
  if (Data is Datatype dt){ // Only works in Athoritative mode on server | will always work on client
    
  }
}
void FastReceivedData( object Data, EventArgs e ){
  if (Data is Datatype dt){ // Only works in Athoritative mode on server | will always work on client
    
  }
}
```

SendType  *Enumeration

  Where SlowUpdate is sent over TCP
  
  And FastUpate is sent over UDP
```c#
public enum SendType {
    SlowUpdate,
    FastUpdate
}
```

ServerMode *Enumeration
  The Servermode tells the server if it should Deserialize examine then Reserialize the data passing through.
  This can be useful to make sure data is

    1 actually belonging to the server

    2 the ability to keep track of objects within the server [ prevent cheating ]

  This requires alot of overhead on the server though.

  Passive doesnt use a serdes and instead just passes the data in and out
     In this mode the server doesnt need to know data types

  Athoritative will Deserialize and Serialize the data
     Refer to the CLI/Program.CS for reference
     In this mode the server needs to know the data types being sent
     Therefore you need to put all your data classes into a library and and the library to both the application and the server
```c#
public enum ServerMode {
    Passive,
    Authoritative
}
```

## Setup

Place all your networked data types into a library
Copy the library to the application and to the server
reference the library for the networked data types

## Contributing

All Credits to Derek Holloway

## License

[MIT](https://choosealicense.com/licenses/mit/)