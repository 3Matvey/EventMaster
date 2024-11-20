import { EventDetails } from "./EventDetails";

interface User {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
    role: 'User' | 'Admin';
    birthDate: string;
    registeredEvents: EventDetails[];
}

export type { User };