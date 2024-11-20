import { useState, useEffect } from 'react';
import axios from 'axios';
import { useParams, useNavigate } from 'react-router-dom';
import { useDispatch } from 'react-redux';
import { Server_HostAddress } from '../../services/constants/Server_HostAddress';
import { updateEvent } from '../../store/slices/eventsSlice';
import { EventDetails } from '../../services/interfaces/EventDetails';
import './EditEvent.css';

export function EditEvent() {
    const { eventId } = useParams<{ eventId: string }>();
    const navigate = useNavigate();
    const dispatch = useDispatch();

    const [name, setName] = useState('');
    const [description, setDescription] = useState('');
    const [date, setDate] = useState('');
    const [place, setPlace] = useState('');
    const [type, setType] = useState('');
    const [maxMemberCount, setMaxMemberCount] = useState<number | ''>('');
    const [imageFile, setImageFile] = useState<File | null>(null);
    const [imageUrl, setImageUrl] = useState('');
    const [message, setMessage] = useState<{ text: string; type: string } | null>(null);

    useEffect(() => {
        const fetchEvent = async () => {
            try {
                const token = localStorage.getItem('token');
                const response = await axios.get(
                    `${Server_HostAddress}/api/events/${eventId}`,
                    {
                        headers: {
                            Authorization: `Bearer ${token}`,
                        },
                    }
                );

                if (response.status === 200) {
                    const event: EventDetails = response.data;
                    setName(event.name);
                    setDescription(event.description);
                    setDate(event.date.slice(0, 10));
                    setPlace(event.place);
                    setType(event.type);
                    setMaxMemberCount(event.maxMemberCount || '');
                }
            } catch (error) {
                console.error('Failed to fetch event details', error);
                setMessage({ text: 'Failed to fetch event details. Please try again.', type: 'error' });
            }
        };

        fetchEvent();
    }, [eventId]);

    const handleEditEvent = async () => {
        try {
            const token = localStorage.getItem('token');
            const updatedEvent = {
                name,
                description,
                date,
                place,
                type,
                maxMemberCount: Number(maxMemberCount),
            };

            const response = await axios.patch(
                
                `${Server_HostAddress}/api/events/${eventId}`,
                updatedEvent,
                {
                    headers: {
                        Authorization: `Bearer ${token}`,
                        'Content-Type': 'application/json',
                    },
                }
            );

            if (response.status === 200) {
                setMessage({ text: 'Event successfully updated!', type: 'success' });
                dispatch(updateEvent(response.data as EventDetails));

                if (imageFile) {
                    await uploadImage(eventId!);
                } else if (imageUrl) {
                    await uploadImageUrl(eventId!, imageUrl);
                }

                navigate('/events');
            }
        } catch (error) {
            console.error('Failed to update event', error);
            setMessage({ text: 'Failed to update event. Please try again.', type: 'error' });
        }
    };

    const uploadImage = async (eventId: string) => {
        const formData = new FormData();
        formData.append('imageFile', imageFile!);

        try {
            const token = localStorage.getItem('token');
            const response = await axios.post(
                `${Server_HostAddress}/api/events/${eventId}/upload-image`,
                formData,
                {
                    headers: {
                        Authorization: `Bearer ${token}`,
                        'Content-Type': 'multipart/form-data',
                    },
                }
            );

            if (response.status === 200) {
                setMessage({ text: 'Image successfully uploaded!', type: 'success' });
            } else {
                console.error('Failed to upload image:', response);
                setMessage({ text: 'Failed to upload image. Please try again.', type: 'error' });
            }
        } catch (error) {
            console.error('Failed to upload image', error);
            setMessage({ text: 'An unexpected error occurred while uploading the image. Please try again.', type: 'error' });
        }
    };

    const uploadImageUrl = async (eventId: string, imageUrl: string) => {
        try {
            const token = localStorage.getItem('token');

            const formData = new FormData();
            formData.append('ImageUrl', imageUrl);
            
            const response = await axios.post(
                `${Server_HostAddress}/api/events/${eventId}/upload-image`,
                formData,
                {
                    headers: {
                        'Authorization': `Bearer ${token}`,
                    },
                }
            );

            if (response.status === 200) {
                setMessage({ text: 'Image URL successfully added to the event!', type: 'success' });
            }
        } catch (error) {
            console.error('Failed to upload image URL', error);
            setMessage({ text: 'Failed to upload image URL. Please try again.', type: 'error' });
        }
    };

    const handleImageFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setImageUrl(''); 
        setImageFile(e.target.files ? e.target.files[0] : null);
    };

    const handleImageUrlChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setImageFile(null); 
        setImageUrl(e.target.value);
    };

    return (
        <div className="edit-event-container">
            {message && (
                <div className={`message ${message.type}`}>
                    {message.text}
                </div>
            )}
            <h2>Edit Event</h2>
            <form className="edit-event-form" onSubmit={(e) => e.preventDefault()}>
                <input
                    type="text"
                    placeholder="Event Name"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    disabled
                />
                <input
                    type="text"
                    placeholder="Description"
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                />
                <input
                    type="date"
                    value={date}
                    onChange={(e) => setDate(e.target.value)}
                />
                <input
                    type="text"
                    placeholder="Place"
                    value={place}
                    onChange={(e) => setPlace(e.target.value)}
                />
                <input
                    type="text"
                    placeholder="Type"
                    value={type}
                    onChange={(e) => setType(e.target.value)}
                />
                <input
                    type="number"
                    placeholder="Max Member Count"
                    value={maxMemberCount}
                    onChange={(e) =>
                        setMaxMemberCount(e.target.value ? Number(e.target.value) : '')
                    }
                />
                <div className="upload-options">
                    <h3>Upload Image or Enter Image URL</h3>
                    <input
                        type="file"
                        accept="image/*"
                        onChange={handleImageFileChange}
                    />
                    <input
                        type="text"
                        placeholder="Or enter image URL"
                        value={imageUrl}
                        onChange={handleImageUrlChange}
                    />
                </div>
                <button type="button" onClick={handleEditEvent}>
                    Update Event
                </button>
            </form>
        </div>
    );
}
