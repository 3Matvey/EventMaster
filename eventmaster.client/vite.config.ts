import { fileURLToPath, URL } from 'node:url';
import { defineConfig, UserConfig } from 'vite';
import react from '@vitejs/plugin-react';
import fs from 'fs';
import path from 'path';
import child_process from 'child_process';
import { env } from 'process';

// Экспортируем конфигурацию через defineConfig
export default defineConfig(({ mode }) => {
    // Базовый объект конфигурации, типизируем как UserConfig
    const config: UserConfig = {
        plugins: [react()],
        resolve: {
            alias: {
                '@': fileURLToPath(new URL('./src', import.meta.url))
            }
        }
        // Поле server добавим ниже при необходимости
    };

    // Если запущен dev-режим (npm run dev => mode === 'development'),
    // тогда включаем HTTPS с dev-сертификатами.
    if (mode === 'development') {
        const baseFolder =
            env.APPDATA && env.APPDATA !== ''
                ? path.join(env.APPDATA, 'ASP.NET/https')
                : path.join(env.HOME || '', '.aspnet/https');

        const certificateName = 'eventmaster.client';
        const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
        const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

        // Если сертификаты не найдены — пытаемся сгенерировать их через dotnet dev-certs
        if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
            const result = child_process.spawnSync(
                'dotnet',
                [
                    'dev-certs',
                    'https',
                    '--export-path',
                    certFilePath,
                    '--format',
                    'Pem',
                    '--no-password'
                ],
                { stdio: 'inherit' }
            );

            if (result.status !== 0) {
                throw new Error('Could not create HTTPS dev certificate via dotnet dev-certs.');
            }
        }

        // Настраиваем прокси на ваш ASP.NET Core бэкенд (если нужно).
        // Порт берем из ASPNETCORE_HTTPS_PORT или ASPNETCORE_URLS.
        const target = env.ASPNETCORE_HTTPS_PORT
            ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
            : env.ASPNETCORE_URLS
                ? env.ASPNETCORE_URLS.split(';')[0]
                : 'https://localhost:7196';

        // Добавляем конфигурацию dev-сервера (https, proxy, port и т.д.)
        config.server = {
            port: 5173,
            https: {
                key: fs.readFileSync(keyFilePath),
                cert: fs.readFileSync(certFilePath)
            },
            proxy: {
                '^/weatherforecast': {
                    target,
                    secure: false
                }
            }
        };
    }

    // Возвращаем итоговый конфиг
    return config;
});
