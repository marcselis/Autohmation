# Autohmation

## What is it?
Autohmation is a Simple Home Automation System that I'm building.
I'm building it myself because I found no other open source software that supports the wireless Niko Easywave switches I have at home.

## How does it work?
This system uses an [Eldat RX09 USB Stick](https://www.eldat.de/produkte/schnittstellen/rx09e_en.html) that is is able to receive and send Easywave telegrams.  The RX09 emulates a serial port that you can use to talk to it.

I configured my wireless devices to work independently of this system in order to be able to manually control the lights in my house when the system is down.  When it is running, the system replicates what is happening in the real world in the virtual model it has of my house by sniffing all Easwave telegrams that are sent.

Of course you can also control the physical world (i.e. automatically turn on/off the light) with this system system.  In order to be able to do that you need to pair every Easywave receiver in your house with a certain telegram that the RX09 can send.

## Roadmap
I'm not going to commit on any dates because I'm working on this project in my free time as some kind of hobby.
The plan that I have in my head is as follows:

---

WORK IN PROGESS: items striked-through are (more or less) done.

---

1. ~~Build the backend components that do the actual work, based on configuration in code.~~
2. Build a simple web GUI to be able control the lights from any device (PC, tablet, phone) connected to our wifi network, to make my wife happy ;-)
3. Add support for some automation tasks (turning lights on at dusk and off again at dawn) in code.
4. Load configuration from config file.
5. Extend web GUI to be able to alter the configuration and restart the backend.
6. ???

## How to build
I'm developing on Windows 10 in .NET Core 3.1 LTS using Visual Studio 2019 Enterprise edition.

If you have the [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1) installed you should also be able to use **free** tools like:
* **dotnet cli** together with any text editor you like.
* [Visual Studio Code](https://code.visualstudio.com/)
* [Visual Studio Community Edition](https://visualstudio.microsoft.com/vs/community/)