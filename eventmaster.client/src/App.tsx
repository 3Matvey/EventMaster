import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { Provider } from 'react-redux';
import { store } from './store';
import { Welcome } from './Components/Welcome';
import { Login } from './Components/Login';
import { Register } from './Components/Register';
import { EventList } from './Components/EventList';
import { EventDetails } from './Components/EventDetails';
import { UserEvents } from './Components/UserEvents';

export function App() {
    return (
        <Provider store={store}>
            <Router>
                <Routes>
                    <Route path="/" element={<Welcome />} />
                    <Route path="/login" element={<Login />} />
                    <Route path="/register" element={<Register />} />
                    <Route path="/events" element={<EventList />} />
                    <Route path="/events/:eventName" element={<EventDetails />} />
                    <Route path="/my-events" element={<UserEvents />} />
                </Routes>
            </Router>
        </Provider>
    );
}

export default App;
