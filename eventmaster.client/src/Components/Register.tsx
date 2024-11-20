import { useState } from 'react';
import axios from 'axios';
import { Server_HostAddress } from '../services/constants/Server_HostAddress';
import './Auth.css';

export function Register() {
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [birthDate, setBirthDate] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [message, setMessage] = useState<{ text: string, type: string } | null>(null);

    const handleRegister = async () => {
        try {
            const response = await axios.post(`${Server_HostAddress}/api/auth/register`, {
                firstName,
                lastName,
                birthDate,
                email,
                password
            });

            if (response.status === 200) {
                setMessage({ text: 'Registration successful', type: 'success' });
            }
        } catch (error) {
            console.error('Registration failed', error);
            setMessage({ text: 'Registration failed. Please try again.', type: 'error' });
        }
    };

    return (
        <div className="auth-container">
            <div className="auth-card">
                <h2 className="auth-title">Register</h2>

                {message && (
                    <div className={`message ${message.type}`}>
                        {message.text}
                    </div>
                )}

                <input
                    type="text"
                    placeholder="First Name"
                    value={firstName}
                    onChange={(e) => setFirstName(e.target.value)}
                    className="auth-input"
                />
                <input
                    type="text"
                    placeholder="Last Name"
                    value={lastName}
                    onChange={(e) => setLastName(e.target.value)}
                    className="auth-input"
                />
                <input
                    type="date"
                    value={birthDate}
                    onChange={(e) => setBirthDate(e.target.value)}
                    className="auth-input"
                />
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
                <button className="auth-button" onClick={handleRegister}>Register</button>
            </div>
        </div>
    );
}
