import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { EventDetails } from '../../services/interfaces/EventDetails';

interface EventsState {
    events: EventDetails[];
}

const initialState: EventsState = {
    events: [],
};

const eventsSlice = createSlice({
    name: 'events',
    initialState,
    reducers: {
        setEvents(state, action: PayloadAction<EventDetails[]>) {
            state.events = action.payload.sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime());
        },
        removeEvent(state, action: PayloadAction<EventDetails>) {
            state.events = state.events.filter(event => event.id !== action.payload.id);
        },
        updateEvent(state, action: PayloadAction<EventDetails>) {
            const index = state.events.findIndex(event => event.id === action.payload.id);
            if (index !== -1) {
                state.events[index] = action.payload;
                state.events.sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime());
            }
        },
    },
});

export const { setEvents, removeEvent, updateEvent } = eventsSlice.actions;
export default eventsSlice.reducer;
