1. Create file named .env in Docker directory with the following content:

POSTGRES_USER=your_username
POSTGRES_PASSWORD=your_password
POSTGRES_DB=your_database

TEFTER_IMAGE_NAME=webvella.tefter.site
TEFTER_IMAGE_VERSION=0.0.1 #version of the tefter image to use

# Connection string consumed by the web container (use Host=db to reach the postgres service)
TEFTER__CONNECTIONSTRING=Host=db;Port=5432;Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD};Database=${POSTGRES_DB};Pooling=true;MinPoolSize=1;MaxPoolSize=100;CommandTimeout=600
ASPNETCORE_ENVIRONMENT=Production

2. Change needed values in the .env file.

3. You need to have Docker installed on your machine.
If you don't have it, download and install it from 
https://www.docker.com/get-started

4. Run the following command to build images and start the containers: (current folder is Docker)

.\from-build-up.bat

This command will build the tefter image from the source code and create 2 images and 2 containers - one for postgres and another for the tefter web application.

5. After the command finishes you should be able to access the tefter application at http://localhost:8080

If you want to stop and remove the containers run:

.\from-build-down.bat

6. To push the built image to docker hub run:

.\push-image.bat

All scripts and bat files are configured to use the .env file for configuration.