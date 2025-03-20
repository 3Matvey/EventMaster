# Используем официальный образ Microsoft SQL Server
FROM mcr.microsoft.com/mssql/server:2022-latest

# Устанавливаем переменные окружения
ENV SA_PASSWORD=YourStrong!Passw0rd
ENV ACCEPT_EULA=Y

# Переключаемся на root для установки утилит
USER root
RUN apt-get update && \
    apt-get install -y curl apt-transport-https && \
    curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - && \
    curl https://packages.microsoft.com/config/ubuntu/20.04/prod.list > /etc/apt/sources.list.d/mssql-tools.list && \
    apt-get update && \
    apt-get install -y mssql-tools unixodbc-dev && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

# Переключаемся обратно на пользователя mssql
USER mssql

# Копируем скрипт инициализации в контейнер
COPY ./init-db.sql /init-db.sql

# Запускаем SQL Server и ждем его готовности, затем выполняем скрипт инициализации
CMD /bin/bash -c "/opt/mssql/bin/sqlservr & \
while ! /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -Q 'SELECT 1' 2>/dev/null; do \
  echo 'Ожидание запуска SQL Server...'; sleep 5; \
done; \
echo 'SQL Server запущен. Выполнение скрипта инициализации...'; \
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d master -i /init-db.sql; \
wait"
