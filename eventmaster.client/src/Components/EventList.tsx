import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from '../store';
import { setEvents } from '../store/slices/eventsSlice';
import { setUser, registerEvent } from '../store/slices/userSlice';
import { Server_HostAddress } from '../services/constants/Server_HostAddress';
import { EventDetails } from '../services/interfaces/Event';

export function EventList() {
    const events = useSelector((state: RootState) => state.events.events);
    const user = useSelector((state: RootState) => state.user);
    const dispatch = useDispatch();
    const navigate = useNavigate();

    useEffect(() => {
        const fetchEvents = async () => {
            try {
                const token = localStorage.getItem('token');  // Получаем токен для запроса

                const response = await axios.get(`${Server_HostAddress}/api/events`, {
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                });

                console.log('Fetched events:', response.data); // Лог для проверки данных от API

                if (response.status === 200 && Array.isArray(response.data)) {
                    dispatch(setEvents(response.data));
                } else {
                    console.error('Unexpected data format:', response.data);
                }
            } catch (error) {
                console.error('Failed to fetch events', error);
            }
        };

        if (events.length === 0) {
            fetchEvents();
        } else {
            console.log('Events already in state:', events); // Лог текущих событий в state
        }
    }, [events.length, dispatch]);

    const handleRegister = async (eventName: string) => {
        if (!user.email) {
            alert('User email is not found. Please log in again.');
            return;
        }

        try {
            console.log(`Registering for event: ${eventName}`);
            console.log(`User email: ${user.email}`);

            const response = await axios.post(`${Server_HostAddress}/api/events/${eventName}/register_to_event`,
                { Email: user.email }, {
                headers: {
                    'Content-Type': 'application/json',
                }
            });

            if (response.status === 200) {
                alert('You have successfully registered for the event.');

                // Обновляем события после успешной регистрации
                const updatedEvents = events.map(event =>
                    event.name === eventName
                        ? { ...event, users: [...event.users, user] }
                        : event
                );

                console.log('Updated events:', updatedEvents);

                dispatch(setEvents(updatedEvents));

                // Обновляем зарегистрированные события пользователя
                const registeredEvent = updatedEvents.find(event => event.name === eventName);
                if (registeredEvent) {
                    dispatch(registerEvent(registeredEvent));
                }
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

    const handleViewDetails = (eventName: string) => {
        navigate(`/events/${eventName}`);
    };

    const handleViewMyEvents = () => {
        navigate('/my-events');
    };

    return (
        <div>
            <header>
                <p>Welcome, {user.firstName} {user.lastName}</p>
                {user.role === 'Admin' && <p>You have admin privileges</p>}
                <button onClick={handleViewMyEvents}>My Registered Events</button>
            </header>

            <h2>Event List</h2>
            <ul>
                {events.length > 0 ? (
                    events.map(event => (
                        <li key={event.name}>
                            {event.name} - {new Date(event.date).toLocaleDateString()}
                            <button onClick={() => handleRegister(event.name)}>Register</button>
                            <button onClick={() => handleViewDetails(event.name)}>View Details</button>
                        </li>
                    ))
                ) : (
                    <p>No events available.</p>
                )}
            </ul>
        </div>
    );
}
