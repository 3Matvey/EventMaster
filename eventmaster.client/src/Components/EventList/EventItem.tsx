import { useNavigate } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import axios from 'axios';
import { Server_HostAddress } from '../../services/constants/Server_HostAddress';
import { RootState } from '../../store';
import { registerEvent } from '../../store/slices/userSlice';
import { EventDetails } from '../../services/interfaces/EventDetails';

interface EventItemProps {
    event: EventDetails;
}

export function EventItem({ event }: EventItemProps) {
    const dispatch = useDispatch();
    const user = useSelector((state: RootState) => state.user);
    const navigate = useNavigate();

    const handleRegister = async () => {
        if (!user.email) {
            alert('User email is not found. Please log in again.');
            return;
        }

        // Проверяем, зарегистрирован ли пользователь на это событие
        if (user.registeredEvents.some(e => e.name === event.name)) {
            alert('You are already registered for this event.');
            return;
        }

        try {
            console.log(`Registering for event: ${event.name}`);
            console.log(`User email: ${user.email}`);

            const response = await axios.post(`${Server_HostAddress}/api/events/${event.name}/register_to_event`,
                { email: user.email }, { // Используем объект DTO для отправки email
                headers: {
                    'Content-Type': 'application/json',
                }
            });

            if (response.status === 200) {
                alert('You have successfully registered for the event.');

                // Обновляем зарегистрированные события пользователя
                dispatch(registerEvent(event));
            }
        } catch (error) {
            console.error('Failed to register:', error);
            if (axios.isAxiosError(error) && error.response) {
                alert(`Registration failed: ${error.response.data.Message}`);
            } else {
                alert('Registration failed due to an unexpected error.');
            }
        }
    };

    const handleViewDetails = () => {
        navigate(`/events/${event.name}`);
    };

    return (
        <li key={event.name}>
            {event.name} - {new Date(event.date).toLocaleDateString()}
            <button onClick={handleRegister}>Register</button>
            <button onClick={handleViewDetails}>View Details</button>
        </li>
    );
}
