#!/bin/bash

# Function to check and create database
create_db_if_not_exists() {
  DB_NAME=$1
  if ! psql -U "$POSTGRES_USER" -d "postgres" -tAc "SELECT 1 FROM pg_database WHERE datname='$DB_NAME'" | grep -q 1; then
    psql -U "$POSTGRES_USER" -d "postgres" -c "CREATE DATABASE $DB_NAME"
  fi
}

create_db_if_not_exists "post_service_db"
create_db_if_not_exists "post_in_series_service_db"
create_db_if_not_exists "post_in_tag_service_db"