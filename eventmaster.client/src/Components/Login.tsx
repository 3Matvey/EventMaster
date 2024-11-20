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

    const navigate = useNavigate(); 
    const dispatch = useDispatch(); 

    const handleLogin = async () => {
        try {
            const response = await axios.post(`${Server_HostAddress}/api/auth/login`, {
                email,
                password
            });

            if (response.status === 200) {
                const data = response.data;
                localStorage.setItem('token', data.token);
                localStorage.setItem('firstName', data.firstName);
                localStorage.setItem('lastName', data.lastName);
                localStorage.setItem('email', data.email);
                localStorage.setItem('role', data.role);

                dispatch(setUser({
                    id: data.id, 
                    firstName: data.firstName,
                    lastName: data.lastName,
                    email: data.email,
                    role: data.role,
                    birthDate: data.birthDate || '', 
                    registeredEvents: [] 
                }));

                navigate('/events');
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
