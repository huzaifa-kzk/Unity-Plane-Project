# Use official Node.js LTS image
FROM node:18

# Set working directory inside container
WORKDIR /usr/src/app

# Copy package.json and package-lock.json first
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy the rest of your server code
COPY . .

# Expose WebSocket port
EXPOSE 8080

# Start the server
CMD ["node", "server.js"]
