import { User } from './User';

interface EventDetails {
    id: number,
    name: string;
    description: string;
    date: string;
    place: string;
    type: string;
    maxMemberCount: number;
    users: User[];
    imagePath?: string;
}
export type { EventDetails };