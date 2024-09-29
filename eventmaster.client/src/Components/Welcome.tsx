import { useNavigate } from 'react-router-dom';

export function Welcome() {
    const navigate = useNavigate();

    const goToLogin = () => {
        navigate('/login');
    };

    const goToRegister = () => {
        navigate('/register');
    };

    return (
        <div>
            <h1>Welcome to EventMaster</h1>
            <p>Please log in or register to continue.</p>
            <button onClick={goToLogin}>Login</button>
            <button onClick={goToRegister}>Register</button>
        </div>
    );
}
