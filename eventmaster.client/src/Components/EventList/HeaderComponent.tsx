import { useNavigate } from 'react-router-dom';
import { useSelector } from 'react-redux';
import { RootState } from '../../store';

export function HeaderComponent() {
    const navigate = useNavigate();
    const user = useSelector((state: RootState) => state.user);

    const handleViewMyEvents = () => {
        navigate('/my-events');
    };

    return (
        <header>
            <p>Welcome, {user.firstName} {user.lastName}</p>
            {user.role === 'Admin' && <p>You have admin privileges</p>}
            <button onClick={handleViewMyEvents}>My Registered Events</button>
        </header>
    );
}
