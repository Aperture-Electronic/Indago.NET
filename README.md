# Indago.NET (IndagoSharp, Indago#)

## Introduction
The Indago.NET (or *IndagoSharp*, *Indago#*) is a .NET implementation for Indago Scripting API,
which allow user to access, control and analysis the database of *Cadence(R) Indago(R) Interactive Verification Enviroment*.

The Indago.NET is rewritten by official Python scripting library provided by Cadence. The target of this project is provide a complete enviroment in C#, contains all functions in the Python API. We'd also like to provide new features that implement based on the library.

## Development Enviroment
Indago.NET is developing under latest .NET 8 and C# 12.0, we are happy to use the latest technologies to develop the software.

All library files are written by C#, and with some third-party library (Google.Protobuf, gRPC.NET, etc.), and the Microsoft .NET standard libraries. The base framework of the project is .NET 8.

## Reference Library
The development source is from the official of Cadence Indago (current version is 22.09).

## Implemented Functions
We have implemented these functions of the official Python API
- [x] Connect running local Indago Server with port
- [x] Startup a new Indago Server with specific database
- [x] Query the scopes in the database, including the scope's name, path, declaration info.
- [x] Query the signals in the database, including the signal's name, width, path, declaration info.
- [x] Get/set the current debug time
- [x] Get the value of any signal in any time
- [x] Register event handler for Current Debug Location Change (CDL_CHANGE)

We are still working on these functions:
- [ ] Multiple database support
- [ ] Get ports, supply nets, power awares and other info. of scopes
- [ ] Get drivers and loads of a signal
- [ ] Get expression informations
- [ ] Indago GUI functions
- [ ] Other event handler of Indago, like Key and Mouse action, selection, waveform change
- [ ] Cursor control

## Plan of Development
We are now processing the base library to implement all functions in official Indago Python API.

## Tools Using This Library
There some parctical tools using this library:
* [IndagoPatchViewer](https://github.com/Aperture-Electronic/IndagoPatchViewer): A tool can realtime display the value as a image patch of selected signal 

## NuGet Package
You can install the package to your project from NuGet by   
```dotnet add package Indago.NET --version 0.1.1```   
or, access the [NuGet repository](https://www.nuget.org/packages/Indago.NET)
