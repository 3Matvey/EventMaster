# Используем официальный образ Microsoft SQL Server
FROM mcr.microsoft.com/mssql/server:2022-latest

# Устанавливаем переменные окружения
ENV SA_PASSWORD=YourStrong!Passw0rd
ENV ACCEPT_EULA=Y

# Копируем скрипт инициализации в контейнер
COPY ./init-db.sql /init-db.sql

# Копируем скрипт запуска
COPY ./entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

# Указываем команду для запуска SQL Server и выполнения скрипта
CMD /bin/bash /entrypoint.sh
