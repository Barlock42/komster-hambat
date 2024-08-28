const express = require('express');
const app = express();
const port = 4242;

app.use(express.json());

// Static variables for clickerUser data
let id = "1234567890";
let balanceCoins = 110716598.49949667;
let availableTaps = 2359;
let lastSyncUpdate = 1719588467;
let maxTaps = 7000;
let earnPerTap = 10;
let tapsRecoverPerSec = 3;

// Mock API endpoint to get clickerUser data
app.get('/api/clickerUser', (req, res) => {
  res.status(200).json({
    "clickerUser": {
      "id": id,
      "balanceCoins": balanceCoins,
      "availableTaps": availableTaps,
      "lastSyncUpdate": lastSyncUpdate,
      "maxTaps": maxTaps,
      "earnPerTap": earnPerTap,
      "tapsRecoverPerSec": tapsRecoverPerSec
    }
  });
});

// Endpoint to update clickerUser data
app.post('/api/clickerUser/tap', (req, res) => {
  const { availableTaps: clientAvailableTaps, count, timestamp } = req.body;
  
  // Calculate the expected availableTaps on the server
  const expectedAvailableTaps = availableTaps - count;
  const deviation = clientAvailableTaps - expectedAvailableTaps;

  // Check for large deviations
  if (deviation > 50) {
    // Client sent more taps than expected, respond with a 400 error
    return res.status(400).json({ message: 'Invalid availableTaps value. Too high compared to expected value.' });
  } else if (deviation < -50) {
    // Client sent fewer taps than expected, update server-side availableTaps to match client's value
    availableTaps = clientAvailableTaps;
  }

  // Update balanceCoins and last sync timestamp
  balanceCoins += count * earnPerTap;
  lastSyncUpdate = timestamp;

  // Log the current values of clickerUser data
  console.log('Updated clickerUser data:', {
    id: id,
    balanceCoins: balanceCoins,
    availableTaps: availableTaps,
    lastSyncUpdate: lastSyncUpdate,
    maxTaps: maxTaps,
    earnPerTap: earnPerTap,
    tapsRecoverPerSec: tapsRecoverPerSec
  });

  res.status(200).json({
    message: 'User data updated successfully',
    clickerUser: {
      id: id,
      balanceCoins: balanceCoins,
      availableTaps: availableTaps,
      lastSyncUpdate: lastSyncUpdate,
      maxTaps: maxTaps,
      earnPerTap: earnPerTap,
      tapsRecoverPerSec: tapsRecoverPerSec
    }
  });
});

// Start the server
app.listen(port, () => {
  console.log(`Mock API server is running at http://localhost:${port}`);
});
