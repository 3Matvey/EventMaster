import { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import axios from 'axios';
import { Server_HostAddress } from '../../services/constants/Server_HostAddress';
import { setEvents } from '../../store/slices/eventsSlice';
import { RootState } from '../../store';
import { HeaderComponent } from '.././EventList/HeaderComponent';
import { EventItem } from '.././EventList/EventItem';

export function EventList() {
    const events = useSelector((state: RootState) => state.events.events);
    const dispatch = useDispatch();

    useEffect(() => {
        const fetchEvents = async () => {
            try {
                const token = localStorage.getItem('token');  // Получаем токен для запроса

                const response = await axios.get(`${Server_HostAddress}/api/events`, {
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                });

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
        }
    }, [events.length, dispatch]);

    return (
        <div>
            <HeaderComponent />
            <h2>Event List</h2>
            <ul>
                {events.length > 0 ? (
                    events.map(event => (
                        <EventItem key={event.name} event={event} />
                    ))
                ) : (
                    <p>No events available.</p>
                )}
            </ul>
        </div>
    );
}
