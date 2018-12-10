# FileBackEnd

Compile the solution in your visual studio (2015 or 2017) in Debug or Release mode, then execute the following file:

\bin\Debug\BackEndSimulation.exe

or

\bin\Release\BackEndSimulation.exe

If done right, you should see the following text in Console:

[x] Awaiting RPC requests
 Press [enter] to exit.
 
 Files are created inside your bin folder, in a folder called "Files", for example:
\bin\Debug\Files\testfile.txt

or
\bin\Release\Files\testfile.txt

The response will be different according to the request.accion:

2 = It will return all existing files in the "Files" folder.
1 = It will create the received file.
3 = It will delete the received file.

Please take note that the MD5 of each file is its id, keep that in mind when sending requests.


