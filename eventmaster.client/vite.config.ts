import { fileURLToPath, URL } from 'node:url';
import { defineConfig, UserConfig } from 'vite';
import react from '@vitejs/plugin-react';
import fs from 'fs';
import path from 'path';
import child_process from 'child_process';
import { env } from 'process';

// ������������ ������������ ����� defineConfig
export default defineConfig(({ mode }) => {
    // ������� ������ ������������, ���������� ��� UserConfig
    const config: UserConfig = {
        plugins: [react()],
        resolve: {
            alias: {
                '@': fileURLToPath(new URL('./src', import.meta.url))
            }
        }
        // ���� server ������� ���� ��� �������������
    };

    // ���� ������� dev-����� (npm run dev => mode === 'development'),
    // ����� �������� HTTPS � dev-�������������.
    if (mode === 'development') {
        const baseFolder =
            env.APPDATA && env.APPDATA !== ''
                ? path.join(env.APPDATA, 'ASP.NET/https')
                : path.join(env.HOME || '', '.aspnet/https');

        const certificateName = 'eventmaster.client';
        const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
        const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

        // ���� ����������� �� ������� � �������� ������������� �� ����� dotnet dev-certs
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

        // ����������� ������ �� ��� ASP.NET Core ������ (���� �����).
        // ���� ����� �� ASPNETCORE_HTTPS_PORT ��� ASPNETCORE_URLS.
        const target = env.ASPNETCORE_HTTPS_PORT
            ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
            : env.ASPNETCORE_URLS
                ? env.ASPNETCORE_URLS.split(';')[0]
                : 'https://localhost:7196';

        // ��������� ������������ dev-������� (https, proxy, port � �.�.)
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

    // ���������� �������� ������
    return config;
});
