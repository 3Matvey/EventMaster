import { useEffect, useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { Server_HostAddress } from '../services/constants/Server_HostAddress';
import { EventDetails } from '../services/interfaces/EventDetails';

export function UserEvents() {
    const [registeredEvents, setRegisteredEvents] = useState<EventDetails[]>([]);
    const navigate = useNavigate();
    const userEmail = localStorage.getItem('email');

    useEffect(() => {
        const fetchUserEvents = async () => {
            if (!userEmail) {
                alert('User email is not found. Please log in again.');
                return;
            }

            try {
                const token = localStorage.getItem('token');

                console.log(`Fetching registered events for user: ${userEmail}`); // Логирование для проверки

                const response = await axios.get(`${Server_HostAddress}/api/events/user/${userEmail}/events`, {
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json' // Убедитесь, что указываете тип содержимого
                    }
                });

                console.log('Fetched registered events:', response.data);

                if (response.status === 200) {
                    setRegisteredEvents(response.data);
                } else {
                    console.error('Unexpected response status:', response.status);
                }
            } catch (error) {
                console.error('Failed to fetch registered events', error);
                if (axios.isAxiosError(error) && error.response) {
                    alert(`Failed to fetch registered events: ${error.response.data.Message}`);
                } else {
                    alert('An unexpected error occurred while fetching registered events.');
                }
            }
        };

        fetchUserEvents();
    }, [userEmail]);

    const handleViewDetails = (eventName: string) => {
        navigate(`/events/${eventName}`);
    };

    return (
        <div>
            <h2>Registered Events</h2>
            <ul>
                {registeredEvents.length > 0 ? (
                    registeredEvents.map(event => (
                        <li key={event.name}>
                            {event.name} - {new Date(event.date).toLocaleDateString()}
                            <button onClick={() => handleViewDetails(event.name)}>View Details</button>
                        </li>
                    ))
                ) : (
                    <p>You are not registered for any events yet.</p>
                )}
            </ul>
        </div>
    );
}
