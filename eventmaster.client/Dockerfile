# ==========================
# 1. Этап сборки (Vite)
# ==========================
FROM node:18 AS build

WORKDIR /app

# Копируем package*.json и устанавливаем зависимости
COPY package*.json ./
RUN npm install

# Копируем весь проект и запускаем сборку
COPY . .
RUN npm run build  # Vite создаст папку /app/dist

# ==========================
# 2. Этап размещения (Nginx)
# ==========================
FROM nginx:alpine

# Устанавливаем gettext для envsubst
RUN apk add --no-cache gettext

# Копируем собранную статику из первого этапа
COPY --from=build /app/dist /usr/share/nginx/html

# Копируем шаблон конфигурации
COPY public/config.template.json /usr/share/nginx/html/config.template.json

# Копируем скрипт entrypoint и делаем его исполняемым
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

# Пробрасываем порт 80
EXPOSE 80

# Задаём entrypoint
ENTRYPOINT ["/entrypoint.sh"]
