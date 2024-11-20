import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import axios from 'axios';
import { Server_HostAddress } from '../services/constants/Server_HostAddress';
import { EventDetails } from '../services/interfaces/EventDetails';
import { User } from '../services/interfaces/User';
import './ShowEvent.css'; 

export function ShowEvent() {
    const { eventId } = useParams<{ eventId: string }>(); 
    const [event, setEvent] = useState<EventDetails | null>(null);
    const [participants, setParticipants] = useState<User[]>([]); 
    const userRole = localStorage.getItem('role'); 

    useEffect(() => {
        const fetchEventDetails = async () => {
            try {
                const response = await axios.get(`${Server_HostAddress}/api/events/${eventId}`);
                setEvent(response.data);
            } catch (error) {
                console.error('Failed to fetch event details', error);
            }
        };

        if (eventId) {
            fetchEventDetails();
        }
    }, [eventId]);

    useEffect(() => {
        const fetchParticipants = async () => {
            if (userRole === 'Admin' && eventId) {
                try {
                    const token = localStorage.getItem('token');
                    const response = await axios.get(`${Server_HostAddress}/api/events/${eventId}/users`, {
                        headers: {
                            'Authorization': `Bearer ${token}`,
                        },
                    });
                    setParticipants(response.data);
                } catch (error) {
                    console.error('Failed to fetch participants', error);
                }
            }
        };

        fetchParticipants();
    }, [eventId, userRole]);

    if (!event) {
        return <p>Loading event details...</p>;
    }

    return (
        <div className="show-event-container">
            <h2>Event Details</h2>
            <div className="event-details">
                <p><strong>Name:</strong> {event.name}</p>
                <p><strong>Description:</strong> {event.description}</p>
                <p><strong>Date:</strong> {new Date(event.date).toLocaleDateString()}</p>
                <p><strong>Place:</strong> {event.place}</p>
                <p><strong>Type:</strong> {event.type}</p>
                <p><strong>Max Members:</strong> {event.maxMemberCount}</p>
            </div>

            {userRole === 'Admin' && participants.length > 0 && (
                <div className="participants-container">
                    <h3>Participants</h3>
                    <ul className="participants-list">
                        {participants.map((participant) => (
                            <li key={participant.email}>
                                <span>{participant.firstName} {participant.lastName}</span>
                                <span>{participant.email}</span>
                            </li>
                        ))}
                    </ul>
                </div>
            )}
        </div>
    );
}
