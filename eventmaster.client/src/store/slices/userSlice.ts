import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { User } from '../../services/interfaces/User';
import { EventDetails } from '../../services/interfaces/EventDetails';

const initialState: User = {
    id: 0,
    firstName: localStorage.getItem('firstName') || '',
    lastName: localStorage.getItem('lastName') || '',
    birthDate: '',
    email: localStorage.getItem('email') || '',
    role: 'User',
    registeredEvents: [] as EventDetails[],  
};

const userSlice = createSlice({
    name: 'user',
    initialState,
    reducers: {
        setUser(state, action: PayloadAction<User>) {
            return { ...state, ...action.payload };
        },
        registerEvent(state, action: PayloadAction<EventDetails>) {
            const isAlreadyRegistered = state.registeredEvents.some(event => event.id === action.payload.id);

            if (!isAlreadyRegistered) {
                state.registeredEvents.push(action.payload);
            } else {
                console.warn(`User is already registered for event: ${action.payload.name}`);
            }
        }
    },
});

export const { setUser, registerEvent } = userSlice.actions;
export default userSlice.reducer;
