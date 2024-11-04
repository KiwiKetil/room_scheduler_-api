DROP DATABASE IF EXISTS DATABASE_NAME;
CREATE DATABASE DATABASE_NAME;
USE DATABASE_NAME;

# create user
CREATE USER IF NOT EXISTS 'YOUR_USERNAME'@'localhost' IDENTIFIED BY 'YOUR_PASSWORD';
CREATE USER IF NOT EXISTS 'YOUR_USERNAME'@'%' IDENTIFIED BY 'YOUR_PASSWORD';
 
 # grant access
GRANT ALL privileges ON DATABASE_NAME.* TO 'YOUR_USERNAME'@'%';
GRANT ALL privileges ON DATABASE_NAME.* TO 'YOUR_USERNAME'@'localhost';
FLUSH PRIVILEGES