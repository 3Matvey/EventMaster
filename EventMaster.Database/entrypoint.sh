#!/bin/bash

# ��������� SQL Server � ����
/opt/mssql/bin/sqlservr &

echo "Waiting 20 seconds for SQL Server to start..."
sleep 20

echo "Running initialization script..."
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d master -i /scripts/StarterKit.sql

# �� ��� ���������� �����������, ���� ����� ������� sqlservr
wait
