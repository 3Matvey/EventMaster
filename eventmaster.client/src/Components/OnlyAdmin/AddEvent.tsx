import { useState } from 'react';
import axios from 'axios';
import { useSelector } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import { RootState } from '../../store';
import { Server_HostAddress } from '../../services/constants/Server_HostAddress';
import './AddEvent.css';

export function AddEvent() {
    const [name, setName] = useState('');
    const [description, setDescription] = useState('');
    const [date, setDate] = useState('');
    const [place, setPlace] = useState('');
    const [type, setType] = useState('');
    const [maxMemberCount, setMaxMemberCount] = useState<number | ''>('');
    const [imageFile, setImageFile] = useState<File | null>(null);
    const [imageUrl, setImageUrl] = useState('');
    const [message, setMessage] = useState<{ text: string; type: string } | null>(null);

    const navigate = useNavigate();
    const user = useSelector((state: RootState) => state.user);

    const handleAddEvent = async () => {
        if (!user.role || user.role !== 'Admin') {
            setMessage({ text: 'Access Denied', type: 'error' });
            return;
        }

        const token = localStorage.getItem('token');
        if (!token) {
            setMessage({ text: 'You need to login first', type: 'error' });
            navigate('/login');
            return;
        }

        try {
            const formattedDate = new Date(date).toISOString();

            const newEvent = {
                name,
                description,
                date: formattedDate,
                place,
                type,
                maxMemberCount: maxMemberCount !== '' ? Number(maxMemberCount) : 0,
            };

            const response = await axios.post(`${Server_HostAddress}/api/events/create`, newEvent, {
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json',
                },
            });

            setMessage({ text: 'Event successfully added!', type: 'success' });

            const createdEvent = await response.data;
            const eventId = createdEvent.id;

            if (imageFile) {
                await uploadImage(eventId);
            } else if (imageUrl) {
                await uploadImageUrl(eventId, imageUrl);
            }

            navigate('/events');
        } catch (error) {
            console.error('Failed to add event', error);
            if (axios.isAxiosError(error) && error.response) {
                setMessage({ text: `Failed to add event: ${error.response.data.Message}`, type: 'error' });
            } else {
                setMessage({ text: 'Failed to add event. Please try again.', type: 'error' });
            }
        }
    };

    const uploadImage = async (eventId: number) => {
        const formData = new FormData();
        formData.append('imageFile', imageFile!);

        try {
            const token = localStorage.getItem('token');
            const response = await axios.post(`${Server_HostAddress}/api/events/${eventId}/upload-image`, formData, {
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'multipart/form-data',
                },
            });

            if (response.status === 200) {
                setMessage({ text: 'Image successfully uploaded!', type: 'success' });
            } else {
                console.error('Failed to upload image:', response);
                setMessage({ text: 'Failed to upload image. Please try again.', type: 'error' });
            }
        } catch (error) {
            console.error('Failed to upload image', error);
            if (axios.isAxiosError(error) && error.response) {
                setMessage({ text: `Failed to upload image: ${error.response.data.Message}`, type: 'error' });
            } else {
                setMessage({ text: 'An unexpected error occurred while uploading the image. Please try again.', type: 'error' });
            }
        }
    };

    const uploadImageUrl = async (eventId: number, imageUrl: string) => {
        try {
            const token = localStorage.getItem('token');

            const formData = new FormData();
            formData.append('ImageUrl', imageUrl);
            
            await axios.post(
                `${Server_HostAddress}/api/events/${eventId}/upload-image`,
                formData,
                {
                    headers: {
                        'Authorization': `Bearer ${token}`,
                    },
                }
            );

            setMessage({ text: 'Image URL successfully added to the event!', type: 'success' });
            
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
        <div className="add-event-container">
            {message && (
                <div className={`message ${message.type}`}>
                    {message.text}
                </div>
            )}
            <h2>Add New Event</h2>
            <form className="add-event-form" onSubmit={(e) => e.preventDefault()}>
                <input
                    type="text"
                    placeholder="Event Name"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
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
                    onChange={(e) => setMaxMemberCount(Number(e.target.value))}
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

                <button type="button" onClick={handleAddEvent}>Add Event</button>
            </form>
        </div>
    );
}
