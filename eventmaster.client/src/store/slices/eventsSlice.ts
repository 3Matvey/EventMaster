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
            state.events = action.payload; 
        },
    },
});

export const { setEvents } = eventsSlice.actions;
export default eventsSlice.reducer;
