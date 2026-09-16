@echo off

REM ==== API ====
cd .\WebGenQRCode
docker build -t qrcode31kn-api .
docker tag qrcode31kn-api:latest novakvova/qrcode31kn-api:latest
docker push novakvova/qrcode31kn-api:latest

echo DONE
pause
