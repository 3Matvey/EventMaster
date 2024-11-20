import { useEffect, useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { Server_HostAddress } from '../services/constants/Server_HostAddress';
import { EventDetails } from '../services/interfaces/EventDetails';
import './UserEvents.css'; 

export function UserEvents() {
    const [registeredEvents, setRegisteredEvents] = useState<EventDetails[]>([]);
    const navigate = useNavigate();
    const userEmail = localStorage.getItem('email');
    const [message, setMessage] = useState<{ text: string, type: 'success' | 'error' | '' }>({ text: '', type: '' });


    useEffect(() => {
        const fetchUserEvents = async () => {
            if (!userEmail) {
                setMessage({ text: 'User email is not found. Please log in again.', type: 'error' });
                return;
            }

            try {
                const token = localStorage.getItem('token');

                console.log(`Fetching registered events for user: ${userEmail}`); 

                const response = await axios.get(`${Server_HostAddress}/api/user/${userEmail}/events`, {
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    }
                });

                console.log('Fetched registered events:', response.data);

                if (response.status === 200) {
                    setRegisteredEvents(response.data);
                    setMessage({ text: '', type: '' }); 
                } else {
                    console.error('Unexpected response status:', response.status);
                }
            } catch (error) {
                console.error('Failed to fetch registered events', error);
                if (axios.isAxiosError(error) && error.response) {
                    setMessage({ text: `Failed to fetch registered events: ${error.response.data.Message}`, type: 'error' });
                } else {
                    setMessage({ text: 'An unexpected error occurred while fetching registered events.', type: 'error' });
                }
            }
        };

        fetchUserEvents();
    }, [userEmail]);

    const handleViewDetails = (eventId: number) => {
        navigate(`/events/${eventId}`);
    };

    const handleUnregister = async (eventId: number) => {
        if (!userEmail) {
            console.log('User email is not found');
            return;
        }

        setMessage({ text: `Are you sure you want to unregister from the event "${eventId}"?`, type: '' });

        if (!window.confirm(`Are you sure you want to unregister from the event "${eventId}"?`)) {
            return;
        }

        try {
            const token = localStorage.getItem('token');

            console.log(`Unregistering user: ${userEmail} from event: ${eventId}`);

            const response = await axios.delete(`${Server_HostAddress}/api/user/${eventId}/unregister`, {
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json',
                },
                data: {
                    email: userEmail,
                },
            });

            if (response.status === 200) {
                setMessage({ text: 'Successfully unregistered from the event.', type: 'success' });

                setRegisteredEvents((prevEvents) =>
                    prevEvents.filter((event) => event.id !== eventId)
                );
            } else {
                console.error('Unexpected response status:', response.status);
            }
        } catch (error) {
            console.error('Failed to unregister from event', error);
            if (axios.isAxiosError(error) && error.response) {
                setMessage({ text: `Failed to unregister from event: ${error.response.data.Message}`, type: 'error' });
            } else {
                setMessage({ text: 'An unexpected error occurred while unregistering from the event.', type: 'error' });
            }
        }
    };

    return (
        <div className="user-events-container">
            <h2>Registered Events</h2>

            {message.text && (
                <div className={`message ${message.type}`}>
                    {message.text}
                </div>
            )}

            <ul className="user-events-list">
                {registeredEvents.length > 0 ? (
                    registeredEvents.map(event => (
                        <li key={event.id}>
                            <div className="event-details">
                                <span>{event.name}</span> - {new Date(event.date).toLocaleDateString()}
                            </div>
                            <div className="event-actions">
                                <button className="view-details-btn" onClick={() => handleViewDetails(event.id)}>View Details</button>
                                <button className="unregister-btn" onClick={() => handleUnregister(event.id)}>Unregister</button>
                            </div>
                        </li>
                    ))
                ) : (
                    <p className="no-events-message">You are not registered for any events yet.</p>
                )}
            </ul>
        </div>
    );
}
