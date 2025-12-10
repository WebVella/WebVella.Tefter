@echo off
REM Load environment variables from .env file
for /f "usebackq tokens=1,2 delims==" %%a in (".env") do (
    set %%a=%%b
)

docker login

docker tag %TEFTER_IMAGE_NAME%:%TEFTER_IMAGE_VERSION%  webvella/%TEFTER_IMAGE_NAME%:%TEFTER_IMAGE_VERSION%

docker push webvella/%TEFTER_IMAGE_NAME%:%TEFTER_IMAGE_VERSION%
