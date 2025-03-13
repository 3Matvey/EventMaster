// Изначальное значение (для разработки)
export let Server_HostAddress: string = 'http://localhost:5242';

// Функция для обновления переменной
export function updateServerHostAddress(newAddress: string) {
    Server_HostAddress = newAddress;
}
