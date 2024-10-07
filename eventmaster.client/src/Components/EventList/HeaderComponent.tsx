import { useNavigate } from 'react-router-dom';
import { useSelector } from 'react-redux';
import { RootState } from '../../store';
import './HeaderComponent.css';

export function HeaderComponent() {
    const navigate = useNavigate();
    const user = useSelector((state: RootState) => state.user);

    const handleViewMyEvents = () => {
        navigate('/my-events');
    };

    const handleAddEvent = () => {
        navigate('/add-event');
    };

    return (
        <header className="header-container">
            <div className="user-info">
                <p className="user-greeting">Welcome, {user.firstName} {user.lastName}</p>
                {user.role === 'Admin' && (
                    <p className="admin-privilege">You have admin privileges</p>
                )}
            </div>
            <div className="header-buttons">
                {user.role === 'Admin' && (
                    <button className="add-event-btn" onClick={handleAddEvent}>Add New Event</button>
                )}
                <button className="my-events-btn" onClick={handleViewMyEvents}>My Registered Events</button>
            </div>
        </header>
    );
}
