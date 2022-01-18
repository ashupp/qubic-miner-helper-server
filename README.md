# qubic-miner-helper-server

### Helps to easy run and observe multiple qubic miners
#### Acts as a server for connected qubic-miner-helper instances

**Download latest server:** https://github.com/ashupp/qubic-miner-helper-server/releases/latest  
Qubic miner helper project: https://github.com/ashupp/qubic-miner-helper  

### New Version 1.0.2.0 (18.01.2022)
- Improved UI by making text boxes readonly instead of disabled. Looks better and you can copy the values.
- Connected helpers needs at least version 1.1.2.0 to show following new values.  
(Download: https://github.com/ashupp/qubic-miner-helper/releases/latest)
- Added display of temperatures and load (needs coretemp running on connected helpers to show values)
- Added display of last time error/s was found per machine.
- Added time of last error reduction to data sent to server
- Added overall restart times to data sent to server
- Added installer & icon

### Version 1.0.1.0
- (at least one connected helper needs to be on version 1.1.1.0 to display new values properly)
- server address is now being saved correctly
- display of current rank added
- display of current remaining pool errors added
- display of helper version of machine added
- current server version is now shown in window title

### Features
- displays summarized data of connected qubic-miner-helper  
- may allow additional control and monitoring of qubic-miner-helpers in future
- like restart workers, observe cpu temperatures...

### Configuration Hint for ngrok / lan users
- you can use * instead of a ip address in server field. 
so instead of 127.0.0.1:6363 or 192.168.1.115:6363 you can use **\*:6363** to bind all local adresses.
This makes also ngrok work and you can connect remote machines to you ngrok url.  
You need ngrok only if you want to connect public machines to your local run server.


![image](https://user-images.githubusercontent.com/1867828/149851220-eed9be9d-9d71-48ef-80c8-0ca51380af40.png)
