import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { App } from './App'
import './index.css'
import { Provider } from 'react-redux'
import { store } from './store'
import { updateServerHostAddress } from './services/constants/Server_HostAddress'

async function initApp() {
    try {
        const response = await fetch('/config.json')
        if (!response.ok) {
            throw new Error('Failed to load config.json')
        }
        const config = await response.json()
        // Обновляем нашу переменную Server_HostAddress
        updateServerHostAddress(config.SERVER_HOST_ADDRESS)
    } catch (error) {
        console.error('Error loading config.json:', error)
        // Можно установить значение по умолчанию, если конфигурация не загрузилась
        updateServerHostAddress('http://localhost:5242')
    }

    createRoot(document.getElementById('root')!).render(
        <Provider store={store}>
            <StrictMode>
                <App />
            </StrictMode>
        </Provider>,
    )
}

initApp()
