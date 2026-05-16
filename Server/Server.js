const express = require('express');
const cors = require('cors');
const mongoose = require('mongoose');

const bcrypt = require('bcryptjs');
const jwt = require('jsonwebtoken');

const app = express();
const port = 3000;

const UserSchema = new mongoose.Schema({
    username: { type: String, unique: true, required: true },
    password: { type: String, required: true }
});
const User = mongoose.model('User', UserSchema);

const JWT_SECRET = "your_local_secret_key_here"; // Keep this safe

// 1. Browser Security Configuration (CORS)
// This permits your Unity browser game to communicate with this server locally
app.use(cors());

// This enables the server to automatically read incoming JSON data payloads
app.use(express.json());

app.post('/login', async (req, res) => {
    const { username, password } = req.body;
    const user = await User.findOne({ username });
    
    if (!user) return res.status(400).send({ status: "Error", message: "User not found" });

    // Compare typed password with the scrambled database password
    const isPasswordValid = await bcrypt.compare(password, user.password);
    if (!isPasswordValid) return res.status(400).send({ status: "Error", message: "Invalid password" });

    // Generate an identity token valid for 24 hours
    const token = jwt.sign({ userId: user._id, username: user.username }, JWT_SECRET, { expiresIn: '24h' });
    
    // Return the token back to Unity
    res.send({ status: "Success", token: token });
});

// 2. Connect to Local MongoDB Database
// 'unity_game_db' is the name of the database that will automatically be created
mongoose.connect('mongodb://localhost:27017/unity_game_db')
  .then(() => console.log('Successfully connected to MongoDB!'))
  .catch(err => console.error('MongoDB database connection error:', err));

// 3. Define the Blueprint (Schema) for Your Game Data
const ScoreSchema = new mongoose.Schema({
    playerName: String,
    score: String,
    date: { type: Date, default: Date.now }
});

// Create a database model based on the layout above
const Score = mongoose.model('Score', ScoreSchema);

// 4. Data Processing Endpoint
// This listens for HTTP POST requests coming from Unity at: http://localhost:3000/data
app.post('/data', async (req, res) => {
    try {
        console.log("Data packet received from Unity Client:", req.body);
        
        // Map the incoming Unity JSON fields to our MongoDB database layout
        const newScore = new Score({
            playerName: req.body.playerName,
            score: req.body.score
        });

        // Write the data directly to your hard drive via MongoDB
        await newScore.save();
        
        // Reply back to Unity to confirm data is safe
        res.send({ status: "Success", message: "Data successfully committed to MongoDB!" });
    } catch (error) {
        console.error("Database save operation failed:", error);
        res.status(500).send({ status: "Error", message: "Internal server error saving data" });
    }
});

// 5. Start the Listener Engine
app.listen(port, () => {
    console.log(`Server framework online. Listening at http://localhost:${port}`);
});
