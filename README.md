# RealTime Chat Application

A real-time chat application built with React and .NET, featuring instant messaging capabilities and room-based chat functionality.

## Features

- Real-time messaging using SignalR
- Room-based chat system
- Modern UI with Chakra UI and Tailwind CSS
- Automatic reconnection handling
- Responsive design

## Tech Stack

### Frontend
- React 18
- Chakra UI
- Tailwind CSS
- SignalR Client

### Backend
- .NET 8.0
- SignalR
- Redis (optional, for scaling)

## Getting Started

### Prerequisites
- Node.js 16+
- .NET 8.0 SDK
- Redis (optional)

### Running the Application

1. Clone the repository
```bash
git clone [your-repo-url]
```

2. Start the backend
```bash
cd backend
dotnet run
```

3. Start the frontend
```bash
cd frontend
npm install
npm start
```

4. Open http://localhost:3000 in your browser

## Development

- Frontend runs on port 3000
- Backend runs on port 5022
- WebSocket connection is established at ws://localhost:5022/chat

## License

MIT 