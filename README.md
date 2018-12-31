# IRC7
IRC7 Project

*** Looking for contributors to the project! I ask most humbly please if you do find bugs fix them rather than breaking our live server ***

About:

Initially created by: Sky, SkyCrest, JD

This is a project that has been ongoing for more than a decade. The project has been contributed mainly by Sky, SkyCrest, and JD. The idea was to have a server that closely mimics what MSN Chat used to provide.

The project was initially created as a C# .NET Service but had undergone multiple iterations, it finally found its way to .NET Core which has gained a lot of traction in the industry and has multiple platform support so this can be targetted for Windows, Linux, MacOS etc.

There were originally two server projects one was Directory Server, another was Chat Server much like on MSN. However more recently we scrapped having two server projects and only use one project, along with merging some of the functionality of the two projects together. E.g. CREATE is now inside of the Chat server.

In terms of what we based the chat server on it is a mixture of the following RFC:

RFC 1459
draft-pfenning-irc-extensions-04
eXonytes Realm & other sources

Current project state:

- Apologies as commenting is VERY scarce, I have literally been rushing this code and it is by no means quality at the moment
- Bits will definitely need to be improved upon, re-written, etc
- Some fundamental architecture decisions may need to be made going forward
- Might be a good idea to separate the Directory Server & Chat Server out again
- Interserver communication logic was put in but is experimental and never used
- No test code!!! Should put NUnit in and do some testing
- Nobody has really tested this thing properly so I can imagine there are lots of quirks that need ironing out
