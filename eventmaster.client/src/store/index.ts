import { configureStore } from '@reduxjs/toolkit';
import eventsReducer from './slices/eventsSlice';
import userReducer from './slices/userSlice';

export const store = configureStore({
    reducer: {
        events: eventsReducer,
        user: userReducer,
    },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
