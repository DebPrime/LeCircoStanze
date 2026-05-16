const mongoose = require('mongoose');
const bcrypt = require('bcryptjs');

// Must match your server's database layout
const UserSchema = new mongoose.Schema({
    username: { type: String, unique: true, required: true },
    password: { type: String, required: true }
});
const User = mongoose.model('User', UserSchema);

async function createPredefinedUsers() {
    await mongoose.connect('mongodb://localhost:27017/unity_game_db');
    
    // Define the exact credentials you want to exist in your database
    const allowedPlayers = [
        { user: "admin", pass: "ProfMaffeis" },
        { user: "player1", pass: "" },
        { user: "player2", pass: "" },
        { user: "player3", pass: "" },
        { user: "player4", pass: "" },
        { user: "player5", pass: "" },
        { user: "player6", pass: "" }
    ];

    for (let player of allowedPlayers) {
        const hashedPassword = await bcrypt.hash(player.pass, 10);
        
        // Update if exists, insert if it doesn't (upsert)
        await User.findOneAndUpdate(
            { username: player.user },
            { password: hashedPassword },
            { upsert: true, new: true }
        );
        console.log(`Preloaded user into database: ${player.user}`);
    }

    console.log("Database seeding complete. Closing connection...");
    mongoose.connection.close();
}

createPredefinedUsers();
