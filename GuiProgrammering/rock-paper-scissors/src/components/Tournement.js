"use client";

import { useState } from "react";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription, DialogFooter } from "./ui/dialog";
import { Input } from "./ui/input";
import { Button } from "./ui/button";

export default function Tournament({ onAddResult }) {
  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [step, setStep] = useState(1); // Track the step in the dialog (1: number of players, 2: enter names)
  const [playersCount, setPlayersCount] = useState("");
  const [playerNames, setPlayerNames] = useState([]); // Array of player names
  const [matches, setMatches] = useState([]); // List of current matches
  const [results, setResults] = useState([]); // List of results
  const [error, setError] = useState("");

  // Handle first step: setting the number of players
  const handleSetPlayersCount = () => {
    const count = parseInt(playersCount);

    if (isNaN(count) || count < 2 || count > 10 || count % 2 !== 0) {
      setError("Please enter an even number of players between 4 and 10.");
    } else {
      setPlayerNames(Array(count).fill("")); // Create empty inputs for each player name
      setError("");
      setStep(2); // Move to the next step
    }
  };

  // Handle starting the tournament with the provided player names
  const handleStartTournament = () => {
    const randomizedPlayers = [...playerNames].sort(() => Math.random() - 0.5);
    const initialMatches = [];

    for (let i = 0; i < randomizedPlayers.length; i += 2) {
      initialMatches.push({
        players: [randomizedPlayers[i], randomizedPlayers[i + 1]],
        scores: [0, 0],
      });
    }
    setMatches(initialMatches);
    setIsCreateOpen(false); // Close the dialog
    setStep(1); // Reset the dialog step for the next time
  };

  const handleScoreChange = (matchIndex, playerIndex, value) => {
    const updatedMatches = [...matches];
    updatedMatches[matchIndex].scores[playerIndex] = parseInt(value) || 0;
    setMatches(updatedMatches);
  };

  const handleNextRound = () => {
    const updatedResults = [...results];
    const winners = [];

    matches.forEach((match, index) => {
      const [score1, score2] = match.scores;
      const winner = score1 > score2 ? match.players[0] : match.players[1];
      winners.push(winner);

      const matchResult = {
        players: match.players,
        scores: match.scores,
        winner,
      };

      updatedResults.push(matchResult);
      onAddResult(matchResult); // Send the match result to the Career component
    });

    const nextRoundMatches = [];
    for (let i = 0; i < winners.length; i += 2) {
      if (winners[i + 1]) {
        nextRoundMatches.push({
          players: [winners[i], winners[i + 1]],
          scores: [0, 0],
        });
      }
    }

    setMatches(nextRoundMatches);
    setResults(updatedResults);
  };

  const handlePlayerNameChange = (index, name) => {
    const updatedNames = [...playerNames];
    updatedNames[index] = name;
    setPlayerNames(updatedNames);
  };

  return (
    <div className="w-[75%] flex flex-col items-center justify-center p-8 space-y-4">
      <h1 className="text-2xl font-bold mb-4">Tournament</h1>
      <Button className="w-[20%]" onClick={() => setIsCreateOpen(true)}>
        Create Tournament
      </Button>

      {/* Display Matches */}
      {matches.length > 0 && (
        <div className="w-full mt-4">
          <h2 className="text-lg font-bold">Current Matches</h2>
          {matches.map((match, index) => (
            <div key={index} className="flex items-center justify-between p-2 bg-neutral-800 mt-2">
              <span>{match.players[0]} vs {match.players[1]}</span>
              <div className="flex items-center space-x-2">
                <Input
                  type="number"
                  placeholder="Score"
                  value={match.scores[0]}
                  onChange={(e) => handleScoreChange(index, 0, e.target.value)}
                  className="w-16 text-center"
                />
                <span>:</span>
                <Input
                  type="number"
                  placeholder="Score"
                  value={match.scores[1]}
                  onChange={(e) => handleScoreChange(index, 1, e.target.value)}
                  className="w-16 text-center"
                />
              </div>
            </div>
          ))}
          <Button onClick={handleNextRound} className="mt-4">
            Next Round
          </Button>
        </div>
      )}

      {/* Dialog for Creating Tournament */}
      <Dialog open={isCreateOpen} onOpenChange={(open) => {setIsCreateOpen(open); setStep(1);}}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Create Tournament</DialogTitle>
            <DialogDescription>
              {step === 1 ? "Enter number of players." : "Enter names for each player."}
            </DialogDescription>
          </DialogHeader>
          <form className="space-y-4" onSubmit={(e) => e.preventDefault()}>
            {step === 1 && (
              <>
                <Input
                  type="number"
                  placeholder="Number of players (2, 4, 6, 8, 10)"
                  value={playersCount}
                  onChange={(e) => setPlayersCount(e.target.value)}
                />
                {error && <p className="text-red-500">{error}</p>}
              </>
            )}
            {step === 2 && playerNames.map((name, index) => (
              <Input
                key={index}
                placeholder={`Player ${index + 1}`}
                value={name}
                onChange={(e) => handlePlayerNameChange(index, e.target.value)}
                className="w-full"
              />
            ))}
          </form>
          <DialogFooter>
            {step === 1 ? (
              <Button onClick={handleSetPlayersCount} className="">
                Next
              </Button>
            ) : (
              <Button onClick={handleStartTournament} className="">
                Start Tournament
              </Button>
            )}
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
