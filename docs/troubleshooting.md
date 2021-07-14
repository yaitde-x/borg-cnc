sudo netstat -ltnp | grep -w '6040'
sudo kill -9 18099
/usr/local/bin/dotnet /home/pi/borg-cnc/borg-api.dll --urls http://0.0.0.0:5000