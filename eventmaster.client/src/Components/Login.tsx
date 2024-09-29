import { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { Server_HostAddress } from '../services/constants/Server_HostAddress';

export function Login() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const navigate = useNavigate(); // Для перенаправления

    const handleLogin = async () => {

        try {
            const response = await axios.post(`${Server_HostAddress}/api/auth/login`, {
                email,
                password
            });

            if (response.status === 200) {
                const data = response.data;
                localStorage.setItem('token', data.token); // Сохраняем токен
                localStorage.setItem('firstName', data.firstName); // Сохраняем имя пользователя
                localStorage.setItem('lastName', data.lastName);   // Сохраняем фамилию пользователя
                localStorage.setItem('email', data.email);
                localStorage.setItem('role', data.role); // Сохраняем роль пользователя

                navigate('/events'); // Перенаправляем на страницу событий после успешного логина
            }
        } catch (error) {
            alert('Login failed');
            console.error('Error logging in:', error);
        }
    };

    return (
        <div>
            <h2>Login</h2>
            <input
                type="email"
                placeholder="Email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
            />
            <input
                type="password"
                placeholder="Password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
            />
            <button onClick={handleLogin}>Login</button>
        </div>
    );
}
