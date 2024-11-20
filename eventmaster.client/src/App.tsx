import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { Welcome } from './Components/Welcome';
import { Login } from './Components/Login';
import { Register } from './Components/Register';
import { EventList } from './Components/EventList';
import { ShowEvent } from './Components/ShowEvent';
import { UserEvents } from './Components/UserEvents';
import { AddEvent } from './Components/OnlyAdmin/AddEvent';
import { EditEvent } from './Components/OnlyAdmin/EditEvent';
import { useEffect } from 'react';
import { useDispatch } from 'react-redux';
import { setUser } from './store/slices/userSlice';
import { User } from './services/interfaces/User';

export function App() {
    const dispatch = useDispatch();

    useEffect(() => {
        const storedUser: User = {
            id: parseInt(localStorage.getItem('id') || '0'),
            firstName: localStorage.getItem('firstName') || '',
            lastName: localStorage.getItem('lastName') || '',
            email: localStorage.getItem('email') || '',
            role: (localStorage.getItem('role') as "User" | "Admin") || "User",
            registeredEvents: JSON.parse(localStorage.getItem('registeredEvents') || '[]'),
            birthDate: localStorage.getItem('birthDate') || ''
        };

        if (storedUser.email) {
            dispatch(setUser(storedUser));
        }
    }, [dispatch]);
    return (
        <Router>
            <Routes>
                <Route path="/" element={<Welcome />} />
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />
                <Route path="/events" element={<EventList />} />
                <Route path="/events/:eventId" element={<ShowEvent />} />
                <Route path="/my-events" element={<UserEvents />} />
                <Route path="/add-event" element={<AddEvent />} />
                <Route path="/edit-event/:eventId" element={<EditEvent />} />
            </Routes>
        </Router>
    );
}

export default App;
