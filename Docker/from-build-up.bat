@echo off
setlocal

rem start db and web with values from repo-root .env
docker compose -f from-build-compose.yml --env-file ..\Docker\.env up -d --build

endlocal