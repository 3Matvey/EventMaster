import { useCallback, useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import axios from 'axios';
import { Server_HostAddress } from '../services/constants/Server_HostAddress';
import { RootState } from '../store';
import { setEvents } from '../store/slices/eventsSlice';
import { EventItem } from '../Components/EventList/EventItem';
import { HeaderComponent } from '../Components/EventList/HeaderComponent';
import { EventDetails } from '../services/interfaces/EventDetails';
import './EventList.css'; 

export function EventList() {
    const events = useSelector((state: RootState) => state.events.events);
    const dispatch = useDispatch();

    const [searchName, setSearchName] = useState('');
    const [searchDate, setSearchDate] = useState('');
    const [type, setType] = useState('');
    const [place, setPlace] = useState('');
    const [pageNumber, setPageNumber] = useState(1);
    const pageSize = 3; 

    
    const handleSetEvents = useCallback((data: EventDetails[]) => {
        dispatch(setEvents(data));
    }, [dispatch]);

    useEffect(() => {
        const fetchEvents = async () => {
            try {
                const token = localStorage.getItem('token');

                const response = await axios.get(`${Server_HostAddress}/api/events/filter`, {
                    headers: {
                        'Authorization': `Bearer ${token}`
                    },
                    params: {
                        name: searchName,
                        date: searchDate,
                        type,
                        place,
                        pageNumber,
                        pageSize
                    }
                });

                console.log("Response:", response);

                if (response.status === 200 && Array.isArray(response.data)) {
                    handleSetEvents(response.data);
                    console.log("Events set in state:", response.data);
                } else {
                    console.warn("Unexpected response structure:", response);
                }
            } catch (error) {
                console.error('Failed to fetch events', error);
            }
        };

        fetchEvents();
    }, [handleSetEvents, searchName, searchDate, type, place, pageNumber, dispatch]);

    return (
        <div className="event-list-container">
            <HeaderComponent />

            <h2 className="event-list-title">Event List</h2>

            <div className="filters">
                <input
                    type="text"
                    placeholder="Search by name"
                    value={searchName}
                    onChange={(e) => setSearchName(e.target.value)}
                />
                <input
                    type="date"
                    value={searchDate}
                    onChange={(e) => setSearchDate(e.target.value)}
                />
                <input
                    type="text"
                    placeholder="Type"
                    value={type}
                    onChange={(e) => setType(e.target.value)}
                />
                <input
                    type="text"
                    placeholder="Place"
                    value={place}
                    onChange={(e) => setPlace(e.target.value)}
                />
            </div>

            <ul className="event-list">
                {events.length > 0 ? (
                    events.map(event => (
                        <EventItem key={event.id} event={event} />
                    ))
                ) : (
                    <p>No events available.</p>
                )}
            </ul>

            <div className="pagination">
                <button
                    disabled={pageNumber === 1}
                    onClick={() => setPageNumber(prev => prev - 1) }
                >
                    Previous
                </button>
                <span>Page {pageNumber}</span>
                <button
                    disabled={events.length < pageSize}
                    onClick={() => {
                        console.log(events.length);
                        setPageNumber(prev => prev + 1);
                    }}
                >
                    Next
                </button>
            </div>
        </div>
    );
}