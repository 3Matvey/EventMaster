import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import axios from 'axios';
import { Server_HostAddress } from '../services/constants/Server_HostAddress';
import { Event } from '../services/interfaces/Event';

export function EventDetails() {
    const { eventName } = useParams<{ eventName: string }>(); // Получаем имя события из URL
    const [event, setEvent] = useState<Event | null>(null);

    useEffect(() => {
        const fetchEventDetails = async () => {
            try {
                const response = await axios.get(`${Server_HostAddress}/api/events/${eventName}`);
                setEvent(response.data);
            } catch (error) {
                console.error('Failed to fetch event details', error);
            }
        };

        if (eventName) {
            fetchEventDetails();
        }
    }, [eventName]);

    if (!event) {
        return <p>Loading event details...</p>;
    }

    return (
        <div>
            <h2>Event Details</h2>
            <p><strong>Name:</strong> {event.name}</p>
            <p><strong>Description:</strong> {event.description}</p>
            <p><strong>Date:</strong> {new Date(event.date).toLocaleDateString()}</p>
            <p><strong>Place:</strong> {event.place}</p>
            <p><strong>Type:</strong> {event.type}</p>
            <p><strong>Max Members:</strong> {event.maxMemberCount}</p>
        </div>
    );
}
