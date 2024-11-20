import { useNavigate } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import axios from 'axios';
import { Server_HostAddress } from '../../services/constants/Server_HostAddress';
import { RootState } from '../../store';
import { removeEvent } from '../../store/slices/eventsSlice';
import { EventDetails } from '../../services/interfaces/EventDetails';
import './EventItem.css'; 
import defaultImage from '@/assets/images.png';
import { useState } from 'react';

interface EventItemProps {
    event: EventDetails;
}

export function EventItem({ event }: EventItemProps) {
    const dispatch = useDispatch();
    const user = useSelector((state: RootState) => state.user);
    const navigate = useNavigate();
    const [message, setMessage] = useState<{ text: string, type: string } | null>(null);

    const handleRegister = async () => {
        if (!user.email) {
            setMessage({ text: 'User email is not found. Please log in again.', type: 'error' });
            return;
        }

        if (user.registeredEvents.some((e) => e.id === event.id)) {
            setMessage({ text: 'You are already registered for this event.', type: 'info' });
            return;
        }

        try {
            const token = localStorage.getItem('token');
            const response = await axios.post(
                `${Server_HostAddress}/api/events/${event.id}/register`,
                { email: user.email },
                {
                    headers: {
                        'Content-Type': 'application/json',
                        Authorization: `Bearer ${token}`,
                    },
                }
            );

            if (response.status === 200) {
                setMessage({ text: 'You have successfully registered for the event.', type: 'success' });
            }
        } catch (error) {
            console.error('Failed to register:', error);
            setMessage({ text: 'Registration failed. Please try again.', type: 'error' });
        }
    };

    const handleViewDetails = () => {
        navigate(`/events/${event.id}`);
    };

    const handleEditEvent = () => {
        navigate(`/edit-event/${event.id}`);
    };

    const handleDeleteEvent = async () => {
        if (window.confirm(`Are you sure you want to delete the event "${event.name}"?`)) {
            try {
                const token = localStorage.getItem('token');
                const response = await axios.delete(`${Server_HostAddress}/api/events/${event.id}`, {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                });

                if (response.status === 200) {
                    setMessage({ text: 'Event deleted successfully.', type: 'success' });
                    dispatch(removeEvent(event));
                }
            } catch (error) {
                console.error('Failed to delete event:', error);
                setMessage({ text: 'Failed to delete event. Please try again.', type: 'error' });
            }
        }
    };

    return (
        <li className="event-item">
            {message && (
                <div className={`message ${message.type}`}>
                    {message.text}
                </div>
            )}
            <div className="event-item__image">
                <img
                    src={event.imagePath ? `${Server_HostAddress}${event.imagePath}` : defaultImage}
                    alt={event.name}
                />
            </div>
            <div className="event-item__details">
                <h3>{event.name}</h3>
                <p><strong>Date:</strong> {new Date(event.date).toLocaleDateString()}</p>
                <p><strong>Place:</strong> {event.place}</p>
                <p>{event.description}</p>
                <div className="event-item__actions">
                    <button onClick={handleRegister}>Register</button>
                    <button onClick={handleViewDetails}>View Details</button>
                    {user.role === 'Admin' && (
                        <>
                            <button onClick={handleEditEvent}>Edit</button>
                            <button onClick={handleDeleteEvent}>Delete</button>
                        </>
                    )}
                </div>
            </div>
        </li>
    );
}
