#!/bin/bash
set -e

echo "=== Starting SQL Server ==="
/opt/mssql/bin/sqlservr &
SQLSERVER_PID=$!

echo "=== Waiting for SQL Server to start... ==="
RETRIES=30
until /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -Q "SELECT 1" > /dev/null 2>&1 || [ $RETRIES -eq 0 ]; do
  echo "Waiting for SQL Server... ($RETRIES retries left)"
  RETRIES=$((RETRIES - 1))
  sleep 2
done

if [ $RETRIES -eq 0 ]; then
  echo "ERROR: SQL Server failed to start!"
  exit 1
fi

echo "=== SQL Server is ready! Running init script... ==="
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -i /docker-entrypoint-initdb/setup.sql

echo "=== Database initialization complete! ==="

wait $SQLSERVER_PID
