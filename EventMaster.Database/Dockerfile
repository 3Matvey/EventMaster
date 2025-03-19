# Используем официальный образ Microsoft SQL Server
FROM mcr.microsoft.com/mssql/server:2022-latest

# Устанавливаем переменные окружения
ENV SA_PASSWORD=YourStrong!Passw0rd
ENV ACCEPT_EULA=Y

# Копируем скрипт инициализации в контейнер
COPY ./init-db.sql /init-db.sql

# Указываем команду для запуска SQL Server и выполнения скрипта
CMD /bin/bash -c "/opt/mssql/bin/sqlservr & /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d master -i /init-db.sql"
