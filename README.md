# Server Standard

This is a central server and client model based on msgpack. works on IPv6 with IPv4 to IPv6 bindings

## Library

This is a library with a CLI attached to it to show usage. You can send and receive any data type except the standard 'Object' type

## Usage

Follow the example listed in Program.cs inside the CLI
Note: All datatypes that are sent to the server, The server needs to be aware of.

Create the server
```c#
IMistoxServer serverobj = mServer.newServer( int Port )
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

serverobj.onSlowReceive += ReceivedData;
serverobj.onSFastReceive += ReceivedData;
void ReceivedData( object Data, EventArgs e ){
  if (Data is Datatype){
    DataType x = (DataType)Data;
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

## Setup

Place all your networked data types into a library
Copy the library to the application and to the server
reference the library for the networked data types

## Contributing

All Credits to Derek Holloway

## License

[MIT](https://choosealicense.com/licenses/mit/)