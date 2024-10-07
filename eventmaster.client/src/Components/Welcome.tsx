import { useNavigate } from 'react-router-dom';
import './Welcome.css';

export function Welcome() {
    const navigate = useNavigate();

    const goToLogin = () => {
        navigate('/login');
    };

    const goToRegister = () => {
        navigate('/register');
    };

    return (
        <div className="welcome-container">
            <div className="welcome-card">
                <h1 className="welcome-title">Welcome to EventMaster</h1>
                <p className="welcome-description">Manage and participate in amazing events.</p>
                <div className="welcome-buttons">
                    <button className="btn login-btn" onClick={goToLogin}>Login</button>
                    <button className="btn register-btn" onClick={goToRegister}>Register</button>
                </div>
            </div>
        </div>
    );
}
