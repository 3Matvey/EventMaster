import { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { useDispatch } from 'react-redux';
import { setUser } from '../store/slices/userSlice';
import { Server_HostAddress } from '../services/constants/Server_HostAddress';
import './Auth.css';

export function Login() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [message, setMessage] = useState<{ text: string, type: string } | null>(null);

    const navigate = useNavigate(); // Для перенаправления
    const dispatch = useDispatch(); // Используем для изменения состояния Redux

    const handleLogin = async () => {
        try {
            const response = await axios.post(`${Server_HostAddress}/api/auth/login`, {
                email,
                password
            });

            if (response.status === 200) {
                const data = response.data;
                // Сохраняем данные в localStorage
                localStorage.setItem('token', data.token);
                localStorage.setItem('firstName', data.firstName);
                localStorage.setItem('lastName', data.lastName);
                localStorage.setItem('email', data.email);
                localStorage.setItem('role', data.role);

                // Обновляем Redux store
                dispatch(setUser({
                    id: data.id, // если есть ID пользователя
                    firstName: data.firstName,
                    lastName: data.lastName,
                    email: data.email,
                    role: data.role,
                    birthDate: data.birthDate || '', // добавьте, если необходимо
                    registeredEvents: [] // пустой массив или значение, если данные уже есть
                }));

                navigate('/events'); // Перенаправляем на страницу событий после успешного логина
            }
        } catch (error) {
            setMessage({ text: 'Login failed. Please check your credentials and try again.', type: 'error' });
            console.error('Error logging in:', error);
        }
    };

    return (
        <div className="auth-container">
            <div className="auth-card">
                <h2 className="auth-title">Login</h2>

                {/* Вывод сообщения, если оно есть */}
                {message && (
                    <div className={`message ${message.type}`}>
                        {message.text}
                    </div>
                )}

                <input
                    type="email"
                    placeholder="Email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    className="auth-input"
                />
                <input
                    type="password"
                    placeholder="Password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    className="auth-input"
                />
                <button className="auth-button" onClick={handleLogin}>Login</button>
            </div>
        </div>
    );
}
